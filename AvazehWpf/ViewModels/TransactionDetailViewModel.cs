using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using System.Threading.Tasks;
using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using SharedLibrary.Validators;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace AvazehWpf.ViewModels
{
    public class TransactionDetailViewModel : ViewAware
    {
        public TransactionDetailViewModel(ITransactionCollectionManager iManager, ITransactionDetailManager dManager, InvoiceDetailSingleton singleton, int? TransactionId, Func<Task> callBack)
        {
            TransactionCollectionManager = iManager;
            TransactionDetailManager = dManager;
            CallBackFunc = callBack;
            Singleton = singleton;
            if (TransactionId is not null)
            {
                ReloadTransaction(TransactionId).ConfigureAwait(true);
            }
            GetComboboxItems();
        }

        private readonly ITransactionCollectionManager TransactionCollectionManager;
        private readonly ITransactionDetailManager TransactionDetailManager;
        private InvoiceDetailSingleton Singleton;
        private TransactionModel _Transaction;
        private readonly Func<Task> CallBackFunc;
        private ObservableCollection<ProductNamesForComboBox> productItems;
        private ObservableCollection<ProductUnitModel> productUnits;
        private ObservableCollection<RecentSellPriceModel> recentSellPrices;
        private TransactionItemModel _workItem = new();
        private bool CanUpdateRowFromDB = true; //False when user DoubleClicks on a row.
        private bool EdittingItem = false;
        public bool CanSaveTransactionChanges { get; set; } = true;
        public TransactionItemModel SelectedItem { get; set; }
        public TransactionItemModel WorkItem { get => _workItem; set { _workItem = value; NotifyOfPropertyChange(() => WorkItem); } }
        public double CustomerPreviousTotalBalance { get => customerPreviousTotalBalance; private set { customerPreviousTotalBalance = value; NotifyOfPropertyChange(() => CustomerPreviousTotalBalance); } }
        public double CustomerTotalBalancePlusThis { get => customerTotalBalancePlusThis; set { customerTotalBalancePlusThis = value; NotifyOfPropertyChange(() => CustomerTotalBalancePlusThis); } }
        public ObservableCollection<ProductNamesForComboBox> ProductItemsForComboBox { get => productItems; set { productItems = value; NotifyOfPropertyChange(() => ProductItemsForComboBox); } }
        public ObservableCollection<ProductUnitModel> ProductUnits { get => productUnits; set { productUnits = value; NotifyOfPropertyChange(() => ProductUnits); } }
        public ObservableCollection<RecentSellPriceModel> RecentSellPrices { get => recentSellPrices; set { recentSellPrices = value; NotifyOfPropertyChange(() => RecentSellPrices); } }
        private ProductNamesForComboBox _selectedProductItem;
        private bool isSellPriceDropDownOpen;
        private string productInput;
        private double customerPreviousTotalBalance;
        private double customerTotalBalancePlusThis;

        public string ProductInput
        {
            get => productInput;
            set { productInput = value; NotifyOfPropertyChange(() => ProductInput); }
        }

        public bool IsSellPriceDropDownOpen
        {
            get => isSellPriceDropDownOpen;
            set { isSellPriceDropDownOpen = value; NotifyOfPropertyChange(() => IsSellPriceDropDownOpen); }
        }

        public ProductNamesForComboBox SelectedProductItem
        {
            get => _selectedProductItem;
            set { _selectedProductItem = value; NotifyOfPropertyChange(() => SelectedProductItem); }
        }

        public TransactionModel Transaction
        {
            get => _Transaction;
            set { _Transaction = value; NotifyOfPropertyChange(() => Transaction); }
        }

        private async Task ReloadTransaction(int? TransactionId)
        {
            if (TransactionId is null) return;
            Transaction = await TransactionCollectionManager.GetItemById((int)TransactionId);
        }

        public void EditItem() //DataGrid doubleClick event
        {
            if (Transaction == null || SelectedItem == null) return;
            CanUpdateRowFromDB = false;
            EdittingItem = true;
            SelectedItem.Clone(WorkItem);
            SelectedProductItem = ProductItemsForComboBox.SingleOrDefault(x => x.Id == SelectedItem.Product.Id);
            CanUpdateRowFromDB = true;
            NotifyOfPropertyChange(() => WorkItem);
        }

        public async Task AddOrUpdateItem()
        {
            if (Transaction == null) return;
            ICollectionManager<ProductModel> productManager = new ProductCollectionManagerAsync<ProductModel, ProductModel_DTO_Create_Update, ProductValidator>(TransactionCollectionManager.ApiProcessor);
            if (SelectedProductItem == null && ProductInput != null && ProductInput.Length > 0 && EdittingItem == false) //Search barcode
            {
                var product = await productManager.GetItemByBarCodeAsync(ProductInput);
                ProductInput = "";
                if (product == null)
                {
                    //Show some red color as "not found" error.
                    return;
                }
                if (Transaction.Items == null) Transaction.Items = new();
                var item = Transaction.Items.FirstOrDefault(x => x.Product.Id == product.Id);
                if (item == null) //If doesnt exsist in list, add new
                {
                    WorkItem = new();
                    WorkItem.TransactionId = Transaction.Id;
                    WorkItem.Product = product;
                    WorkItem.SellPrice = product.SellPrice;
                    WorkItem.BuyPrice = product.BuyPrice;
                    WorkItem.CountString = (1).ToString();
                    var addedItem = await TransactionDetailManager.CreateItemAsync(WorkItem);
                    if (addedItem is not null)
                    {
                        //Validate here
                        Transaction.Items.Add(addedItem);
                    }
                }
                else //if exists in list, update it to one more
                {
                    WorkItem = item;
                    WorkItem.CountString = (WorkItem.CountValue + 1).ToString();
                    await UpdateItemInDatabase(WorkItem);
                }
            }
            else
            {
                if (WorkItem == null || SelectedProductItem == null || SelectedProductItem.Id == 0) return;
                WorkItem.TransactionId = Transaction.Id;
                WorkItem.Product = await productManager.GetItemById(SelectedProductItem.Id);
                var validate = TransactionDetailManager.ValidateItem(WorkItem);
                if (!validate.IsValid)
                {
                    var str = "";
                    foreach (var error in validate.Errors)
                        str += error.ErrorMessage + "\n";
                    MessageBox.Show(str, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (EdittingItem == false) //New Item
                {
                    if (Transaction.Items == null) Transaction.Items = new();
                    var addedItem = await TransactionDetailManager.CreateItemAsync(WorkItem);
                    if (addedItem is not null)
                        Transaction.Items.Add(addedItem);
                }
                else //Edit Item
                {
                    await UpdateItemInDatabase(WorkItem);
                    EdittingItem = false;
                }
            }
            ProductUnitModel temp = WorkItem.Unit;
            WorkItem = new();
            WorkItem.Unit = temp;
            SelectedProductItem = null;
            ProductInput = "";
            ReloadCustomerTotalBalance();
            NotifyOfPropertyChange(() => Transaction.Items);
            NotifyOfPropertyChange(() => Transaction);
        }

        private async Task UpdateItemInDatabase(TransactionItemModel item)
        {
            var ResultItem = await TransactionDetailManager.UpdateItemAsync(item);
            var EdittedItem = Transaction.Items.FirstOrDefault(x => x.Id == item.Id);
            if (ResultItem != null) ResultItem.Clone(EdittedItem);
            RefreshDataGrid();
        }

        public async Task DeleteItem()
        {
            if (Transaction == null || Transaction.Items == null || !Transaction.Items.Any() || SelectedItem == null) return;
            var result = MessageBox.Show("Are you sure you want to delete this row ?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (await TransactionDetailManager.DeleteItemAsync(SelectedItem.Id))
                Transaction.Items.Remove(SelectedItem);
            RefreshDataGrid();
            NotifyOfPropertyChange(() => Transaction);
        }

        public void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {
                DeleteItem().ConfigureAwait(true);
                e.Handled = true;
            }
        }

        public async Task DeleteTransactionAndClose()
        {
            if (Transaction == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete Transaction for {Transaction.Customer.FullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (await TransactionCollectionManager.DeleteItemAsync(Transaction.Id) == false) MessageBox.Show($"Transaction with ID: {Transaction.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CloseWindow();
        }

        private void RefreshDataGrid()
        {
            TransactionModel temp;
            temp = Transaction;
            Transaction = null;
            Transaction = temp;
        }

        public async Task SaveTransactionChanges()
        {
            var result = await TransactionCollectionManager.UpdateItemAsync(Transaction);
            Transaction.DateUpdated = result.DateUpdated;
            Transaction.TimeUpdated = result.TimeUpdated;
            RefreshDataGrid();
            CanSaveTransactionChanges = false;
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        public async Task ClosingWindow()
        {
            await CallBackFunc?.Invoke();
        }

        public void SellPrice_GotFocus()
        {
            if (RecentSellPrices != null && RecentSellPrices.Count > 1)
                IsSellPriceDropDownOpen = true;
        }

        public void ProductNames_PreviewTextInput(object sender, EventArgs e)
        {
            var combo = sender as ComboBox;
            combo.IsDropDownOpen = true;
        }

        public async Task ProductNames_SelectionChanged(object sender, EventArgs e)
        {
            if (Transaction == null || SelectedProductItem == null) return;
            if (CanUpdateRowFromDB is false) return;
            ICollectionManager<ProductModel> productManager = new ProductCollectionManagerAsync<ProductModel, ProductModel_DTO_Create_Update, ProductValidator>(TransactionCollectionManager.ApiProcessor);
            WorkItem.Product = await productManager.GetItemById(SelectedProductItem.Id);
            WorkItem.BuyPrice = WorkItem.Product.BuyPrice;
            WorkItem.SellPrice = WorkItem.Product.SellPrice;
            RecentSellPrices?.Clear();
            var recents = await TransactionDetailManager.GetRecentSellPrices(1, Transaction.Customer.Id, WorkItem.Product.Id);
            if (recents != null && recents.Count > 0) RecentSellPrices = recents;
            if (RecentSellPrices == null) RecentSellPrices = new();
            RecentSellPrices.Add(new RecentSellPriceModel { SellPrice = WorkItem.Product.SellPrice, DateSold = "اکنون" });
            NotifyOfPropertyChange(() => WorkItem.Product);
            NotifyOfPropertyChange(() => Transaction.Items);
            NotifyOfPropertyChange(() => WorkItem);
        }

        public void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (EdittingItem == false) (GetView() as Window).Close();
                else
                {
                    EdittingItem = false;
                    WorkItem = new();
                    SelectedProductItem = new();
                }
            }
        }

        private void GetComboboxItems()
        {
            ProductItemsForComboBox = Singleton.ProductItemsForCombobox;
            ProductUnits = Singleton.ProductUnits;
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}