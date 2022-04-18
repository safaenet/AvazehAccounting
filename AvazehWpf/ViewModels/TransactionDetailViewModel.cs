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
                TransactionDetailManager.TransactionId = (int)TransactionId;
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
        private TransactionItemModel _workItem = new();
        private bool EdittingItem = false;
        public bool CanSaveTransactionChanges { get; set; } = true;
        public TransactionItemModel SelectedItem { get; set; }
        public TransactionItemModel WorkItem { get => _workItem; set { _workItem = value; NotifyOfPropertyChange(() => WorkItem); } }
        public ObservableCollection<ProductNamesForComboBox> ProductItemsForComboBox { get => productItems; set { productItems = value; NotifyOfPropertyChange(() => ProductItemsForComboBox); } }
        private bool isTitleInputDropDownOpen;

        public bool IsTitleInputDropDownOpen { get => isTitleInputDropDownOpen; set { isTitleInputDropDownOpen = value; NotifyOfPropertyChange(() => IsTitleInputDropDownOpen); } }

        public TransactionModel Transaction
        {
            get => _Transaction;
            set { _Transaction = value; NotifyOfPropertyChange(() => Transaction); }
        }

        private async Task ReloadTransaction(int? TransactionId)
        {
            if (TransactionId is null) return;
            Transaction = await TransactionCollectionManager.GetItemById((int)TransactionId);
            await TransactionDetailManager.LoadFirstPageAsync();
        }

        public void EditItem() //DataGrid doubleClick event
        {
            if (Transaction == null || SelectedItem == null) return;
            EdittingItem = true;
            SelectedItem.Clone(WorkItem);
            NotifyOfPropertyChange(() => WorkItem);
        }

        public async Task AddOrUpdateItem()
        {
            if (Transaction == null || WorkItem == null) return;
            WorkItem.TransactionId = Transaction.Id;
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
            WorkItem = new();
            NotifyOfPropertyChange(() => Transaction.Items);
            NotifyOfPropertyChange(() => Transaction);
        }

        private async Task UpdateItemInDatabase(TransactionItemModel item)
        {
            var ResultItem = await TransactionDetailManager.UpdateItemAsync(item);
            var EdittedItem = Transaction.Items.FirstOrDefault(x => x.Id == item.Id);
            if (ResultItem != null) ResultItem.Clone(EdittedItem);
            Transaction.DateUpdated = EdittedItem.DateUpdated;
            Transaction.TimeUpdated = EdittedItem.TimeUpdated;
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
            var result = MessageBox.Show("Are you sure ?", $"Delete Transaction file {Transaction.FileName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
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

        public void ProductNames_PreviewTextInput()
        {
            IsTitleInputDropDownOpen = true;
        }

        public void ProductNames_PreviewTextInput(object sender, EventArgs e)
        {
            var combo = sender as ComboBox;
            combo.IsDropDownOpen = true;
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
                }
            }
        }

        private void GetComboboxItems()
        {
            ProductItemsForComboBox = Singleton.ProductItemsForCombobox;
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}