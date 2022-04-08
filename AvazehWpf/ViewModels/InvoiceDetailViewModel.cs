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
    public class InvoiceDetailViewModel : ViewAware
    {
        public InvoiceDetailViewModel(IInvoiceCollectionManager iManager, IInvoiceDetailManager dManager, InvoiceDetailSingleton singleton, int? InvoiceId, Func<Task> callBack)
        {
            InvoiceCollectionManager = iManager;
            InvoiceDetailManager = dManager;
            CallBackFunc = callBack;
            Singleton = singleton;
            if (InvoiceId is not null)
            {
                ReloadInvoice(InvoiceId).ConfigureAwait(true);
            }
            GetComboboxItems();
        }

        private readonly IInvoiceCollectionManager InvoiceCollectionManager;
        private readonly IInvoiceDetailManager InvoiceDetailManager;
        private InvoiceDetailSingleton Singleton;
        private InvoiceModel _Invoice;
        private readonly Func<Task> CallBackFunc;
        private ObservableCollection<ProductNamesForComboBox> productItems;
        private ObservableCollection<ProductUnitModel> productUnits;
        private ObservableCollection<RecentSellPriceModel> recentSellPrices;
        private InvoiceItemModel _workItem = new();
        private bool CanUpdateRowFromDB = true; //False when user DoubleClicks on a row.
        private bool EdittingItem = false;
        public bool CanSaveInvoiceChanges { get; set; } = true;
        public InvoiceItemModel SelectedItem { get; set; }
        public InvoiceItemModel WorkItem { get => _workItem; set { _workItem = value; NotifyOfPropertyChange(() => WorkItem); } }
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


        public ProductUnitModel SelectedProductUnit
        {
            get => WorkItem.Unit;
            set { WorkItem.Unit = value; NotifyOfPropertyChange(() => SelectedProductUnit); }
        }

        public ProductNamesForComboBox SelectedProductItem
        {
            get => _selectedProductItem;
            set { _selectedProductItem = value; NotifyOfPropertyChange(() => SelectedProductItem); }
        }

        public InvoiceModel Invoice
        {
            get => _Invoice;
            set { _Invoice = value; NotifyOfPropertyChange(() => Invoice); }
        }

        private async Task ReloadInvoice(int? InvoiceId)
        {
            if (InvoiceId is null) return;
            Invoice = await InvoiceCollectionManager.GetItemById((int)InvoiceId);
            await ReloadCustomerPreviousBalance();
        }

        private async Task ReloadCustomerPreviousBalance()
        {
            if (Invoice is null) return;
            CustomerPreviousTotalBalance = await InvoiceCollectionManager.GetCustomerTotalBalanceById(Invoice.Customer.Id, Invoice.Id);
            ReloadCustomerTotalBalance();
        }

        private void ReloadCustomerTotalBalance()
        {
            if (Invoice is null) return;
            CustomerTotalBalancePlusThis = CustomerPreviousTotalBalance + (Invoice == null ? 0 : Invoice.TotalBalance);
        }

        public void EditItem() //DataGrid doubleClick event
        {
            if (Invoice == null || SelectedItem == null) return;
            CanUpdateRowFromDB = false;
            EdittingItem = true;
            SelectedItem.Clone(WorkItem);
            SelectedProductItem = ProductItemsForComboBox.SingleOrDefault(x => x.Id == SelectedItem.Product.Id);
            SelectedProductUnit = SelectedItem.Unit == null || SelectedItem.Unit.Id == 0 ? null : ProductUnits.SingleOrDefault(x => x.Id == SelectedItem.Unit.Id);
            CanUpdateRowFromDB = true;
            NotifyOfPropertyChange(() => WorkItem);
            NotifyOfPropertyChange(() => SelectedProductUnit);
        }

        public async Task AddOrUpdateItem()
        {
            if (Invoice == null) return;
            ICollectionManager<ProductModel> productManager = new ProductCollectionManagerAsync<ProductModel, ProductModel_DTO_Create_Update, ProductValidator>(InvoiceCollectionManager.ApiProcessor);
            if (SelectedProductItem == null && ProductInput != null && ProductInput.Length > 0 && EdittingItem == false) //Search barcode
            {
                var product = await productManager.GetItemByBarCodeAsync(ProductInput);
                ProductInput = "";
                if (product == null)
                {
                    //Show some red color as "not found" error.
                    return;
                }
                if (Invoice.Items == null) Invoice.Items = new();
                var item = Invoice.Items.FirstOrDefault(x => x.Product.Id == product.Id);
                if (item == null) //If doesnt exsist in list, add new
                {
                    WorkItem = new();
                    WorkItem.InvoiceId = Invoice.Id;
                    WorkItem.Product = product;
                    WorkItem.SellPrice = product.SellPrice;
                    WorkItem.BuyPrice = product.BuyPrice;
                    WorkItem.CountString = (1).ToString();
                    var addedItem = await InvoiceDetailManager.CreateItemAsync(WorkItem);
                    if (addedItem is not null)
                    {
                        //Validate here
                        Invoice.Items.Add(addedItem);
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
                WorkItem.InvoiceId = Invoice.Id;
                WorkItem.Product = await productManager.GetItemById(SelectedProductItem.Id);
                var validate = InvoiceDetailManager.ValidateItem(WorkItem);
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
                    if (Invoice.Items == null) Invoice.Items = new();
                    var addedItem = await InvoiceDetailManager.CreateItemAsync(WorkItem);
                    if (addedItem is not null)
                        Invoice.Items.Add(addedItem);
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
            NotifyOfPropertyChange(() => Invoice.Items);
        }

        private async Task UpdateItemInDatabase(InvoiceItemModel item)
        {
            var ResultItem = await InvoiceDetailManager.UpdateItemAsync(WorkItem);
            var EdittedItem = Invoice.Items.FirstOrDefault(x => x.Id == WorkItem.Id);
            if (ResultItem != null) ResultItem.Clone(EdittedItem);
            RefreshDataGrid();
        }

        public async Task DeleteItem()
        {
            if (Invoice == null || Invoice.Items == null || !Invoice.Items.Any() || SelectedItem == null) return;
            var result = MessageBox.Show("Are you sure you want to delete this row ?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (await InvoiceDetailManager.DeleteItemAsync(SelectedItem.Id))
                Invoice.Items.Remove(SelectedItem);
            RefreshDataGrid();
            NotifyOfPropertyChange(() => Invoice);
        }

        public void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {
                DeleteItem().ConfigureAwait(true);
                e.Handled = true;
            }
        }

        public async Task DeleteInvoiceAndClose()
        {
            if (Invoice == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete Invoice for {Invoice.Customer.FullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (await InvoiceCollectionManager.DeleteItemAsync(Invoice.Id) == false) MessageBox.Show($"Invoice with ID: {Invoice.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CloseWindow();
        }

        private void RefreshDataGrid()
        {
            InvoiceModel temp;
            temp = Invoice;
            Invoice = null;
            Invoice = temp;
        }

        public async Task ViewPayments()
        {
            WindowManager wm = new();
            await wm.ShowWindowAsync(new InvoicePaymentsViewModel(InvoiceCollectionManager, InvoiceDetailManager, Invoice, RefreshDataGrid));
        }

        public async Task SaveInvoiceChanges()
        {
            var result = await InvoiceCollectionManager.UpdateItemAsync(Invoice);
            Invoice.DateUpdated = result.DateUpdated;
            Invoice.TimeUpdated = result.TimeUpdated;
            RefreshDataGrid();
            CanSaveInvoiceChanges = false;
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
            if (Invoice == null || SelectedProductItem == null) return;
            if (CanUpdateRowFromDB is false) return;
            ICollectionManager<ProductModel> productManager = new ProductCollectionManagerAsync<ProductModel, ProductModel_DTO_Create_Update, ProductValidator>(InvoiceCollectionManager.ApiProcessor);
            WorkItem.Product = await productManager.GetItemById(SelectedProductItem.Id);
            WorkItem.BuyPrice = WorkItem.Product.BuyPrice;
            WorkItem.SellPrice = WorkItem.Product.SellPrice;
            RecentSellPrices?.Clear();
            var recents = await InvoiceDetailManager.GetRecentSellPrices(1, Invoice.Customer.Id, WorkItem.Product.Id);
            if (recents != null && recents.Count > 0) RecentSellPrices = recents;
            if (RecentSellPrices == null) RecentSellPrices = new();
            RecentSellPrices.Add(new RecentSellPriceModel { SellPrice = WorkItem.Product.SellPrice, DateSold = "اکنون" });
            NotifyOfPropertyChange(() => WorkItem.Product);
            NotifyOfPropertyChange(() => Invoice.Items);
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

    #region DataGrid Row Number
    public class DataGridBehavior
    {
        #region DisplayRowNumber

        public static DependencyProperty DisplayRowNumberProperty =
            DependencyProperty.RegisterAttached("DisplayRowNumber",
                                                typeof(bool),
                                                typeof(DataGridBehavior),
                                                new FrameworkPropertyMetadata(false, OnDisplayRowNumberChanged));
        public static bool GetDisplayRowNumber(DependencyObject target)
        {
            return (bool)target.GetValue(DisplayRowNumberProperty);
        }
        public static void SetDisplayRowNumber(DependencyObject target, bool value)
        {
            target.SetValue(DisplayRowNumberProperty, value);
        }

        private static void OnDisplayRowNumberChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = target as DataGrid;
            if ((bool)e.NewValue)
            {
                EventHandler<DataGridRowEventArgs> loadedRowHandler = null;
                loadedRowHandler = (object sender, DataGridRowEventArgs ea) =>
                {
                    if (GetDisplayRowNumber(dataGrid) == false)
                    {
                        dataGrid.LoadingRow -= loadedRowHandler;
                        return;
                    }
                    ea.Row.Header = ea.Row.GetIndex() + 1;
                };
                dataGrid.LoadingRow += loadedRowHandler;

                ItemsChangedEventHandler itemsChangedHandler = null;
                itemsChangedHandler = (object sender, ItemsChangedEventArgs ea) =>
                {
                    if (GetDisplayRowNumber(dataGrid) == false)
                    {
                        dataGrid.ItemContainerGenerator.ItemsChanged -= itemsChangedHandler;
                        return;
                    }
                    GetVisualChildCollection<DataGridRow>(dataGrid).
                        ForEach(d => d.Header = d.GetIndex() + 1);
                };
                dataGrid.ItemContainerGenerator.ItemsChanged += itemsChangedHandler;
            }
        }

        #endregion // DisplayRowNumber

        #region Get Visuals

        private static List<T> GetVisualChildCollection<T>(object parent) where T : Visual
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                if (child != null)
                {
                    GetVisualChildCollection(child, visualCollection);
                }
            }
        }

        #endregion // Get Visuals
    }
    #endregion
}