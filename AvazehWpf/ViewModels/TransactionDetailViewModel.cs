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
using SharedLibrary.Enums;

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
            GetComboboxItems().ConfigureAwait(true);
        }
        private readonly ITransactionCollectionManager TransactionCollectionManager;
        private readonly ITransactionDetailManager TransactionDetailManager;
        private InvoiceDetailSingleton Singleton;
        private TransactionModel _Transaction;
        private readonly Func<Task> CallBackFunc;
        private ObservableCollection<ItemsForComboBox> productItems;
        private ObservableCollection<ItemsForComboBox> transactionsForComboBox;
        private TransactionItemModel _workItem = new();
        private bool _EdittingItem;
        public bool EdittingItem
        {
            get => _EdittingItem;
            set { _EdittingItem = value; NotifyOfPropertyChange(() => EdittingItem); }
        }
        private string windowTitle;

        public string WindowTitle
        {
            get { return windowTitle; }
            set { windowTitle = value; NotifyOfPropertyChange(() => WindowTitle); }
        }

        public bool CanSaveTransactionChanges { get => canSaveTransactionChanges; set { canSaveTransactionChanges = value; NotifyOfPropertyChange(() => CanSaveTransactionChanges); } }
        public TransactionItemModel SelectedItem { get; set; }
        private TransactionItemModel selectedItem_Backup { get; set; } = new();
        public TransactionItemModel WorkItem { get => _workItem; set { _workItem = value; NotifyOfPropertyChange(() => WorkItem); } }
        public ObservableCollection<ItemsForComboBox> ProductItemsForComboBox { get => productItems; set { productItems = value; NotifyOfPropertyChange(() => ProductItemsForComboBox); } }
        public ObservableCollection<ItemsForComboBox> TransactionsForComboBox { get => transactionsForComboBox; set { transactionsForComboBox = value; NotifyOfPropertyChange(() => TransactionsForComboBox); } }
        private bool isTitleInputDropDownOpen;
        private bool canSaveTransactionChanges;

        public string SearchText { get; set; }
        public int SelectedFinStatus { get; set; } = 3;

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
            WindowTitle = Transaction.FileName + " - فایل";
            await Search();
        }

        public void EditItem() //DataGrid doubleClick event
        {
            if (Transaction == null || SelectedItem == null) return;
            EdittingItem = true;
            SelectedItem.Clone(WorkItem);
            SelectedItem.Clone(selectedItem_Backup);
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
                {
                    Transaction.Items.Add(addedItem);
                    if (addedItem.TotalValue > 0) Transaction.TotalPositiveItemsSum += addedItem.TotalValue;
                    else if (addedItem.TotalValue < 0) Transaction.TotalNegativeItemsSum += addedItem.TotalValue;
                    if (TransactionsForComboBox.Where(x => x.IsChecked).Any())
                    {
                        if (MessageBox.Show("آیا در فایل یا فایل های دیگری که انتخاب کرده اید نیز ثبت شود؟ ", "ثبت در فایل های دیگر", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                        {
                            foreach (var item in TransactionsForComboBox)
                            {
                                if (item.IsChecked)
                                {
                                    WorkItem.TransactionId = item.Id;
                                    WorkItem.Descriptions += $" -ثبت شده از فایل {Transaction.FileName}- ";
                                    await TransactionDetailManager.CreateItemAsync(WorkItem);
                                    item.IsChecked = false;
                                }
                            }
                            var temp = TransactionsForComboBox;
                            TransactionsForComboBox = null;
                            TransactionsForComboBox = temp;
                            WorkItem.TransactionId = Transaction.Id;
                        }
                    }
                }
            }
            else //Edit Item
            {
                await UpdateItemInDatabase(WorkItem);
                EdittingItem = false;
            }
            WorkItem = new();
            selectedItem_Backup = new();
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
            if (selectedItem_Backup.TotalValue > 0)
            {
                Transaction.TotalPositiveItemsSum -= selectedItem_Backup.TotalValue;
                if (EdittedItem.TotalValue > 0) Transaction.TotalPositiveItemsSum += item.TotalValue;
                else if (EdittedItem.TotalValue < 0) Transaction.TotalNegativeItemsSum += item.TotalValue;
                //Transaction.TotalPositiveItemsSum += item.TotalValue;
            }
            else if (selectedItem_Backup.TotalValue < 0)
            {
                Transaction.TotalNegativeItemsSum -= selectedItem_Backup.TotalValue;
                if (EdittedItem.TotalValue > 0) Transaction.TotalPositiveItemsSum += item.TotalValue;
                else if (EdittedItem.TotalValue < 0) Transaction.TotalNegativeItemsSum += item.TotalValue;
                //Transaction.TotalNegativeItemsSum += item.TotalValue;
            }
            RefreshDataGrid();
        }

        public async Task DeleteItem()
        {
            if (Transaction == null || Transaction.Items == null || !Transaction.Items.Any() || SelectedItem == null) return;
            var result = MessageBox.Show("Are you sure you want to delete this row ?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (await TransactionDetailManager.DeleteItemAsync(SelectedItem.Id))
            {
                if (SelectedItem.TotalValue > 0) Transaction.TotalPositiveItemsSum -= SelectedItem.TotalValue;
                else if (SelectedItem.TotalValue < 0) Transaction.TotalNegativeItemsSum -= SelectedItem.TotalValue;
                Transaction.Items.Remove(SelectedItem);
            }
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

        public async Task Search()
        {
            TransactionFinancialStatus? FinStatus = SelectedFinStatus >= Enum.GetNames(typeof(TransactionFinancialStatus)).Length ? null : (TransactionFinancialStatus)SelectedFinStatus;
            TransactionDetailManager.SearchValue = SearchText;
            EdittingItem = false;
            WorkItem = new();
            TransactionDetailManager.FinStatus = FinStatus;
            await TransactionDetailManager.LoadFirstPageAsync();
            Transaction.Items = TransactionDetailManager.Items;
            NotifyOfPropertyChange(() => Transaction);
        }

        public async Task PreviousPage()
        {
            await TransactionDetailManager.LoadPreviousPageAsync();
            Transaction.Items= TransactionDetailManager.Items;
            NotifyOfPropertyChange(() => Transaction);
        }

        public async Task NextPage()
        {
            await TransactionDetailManager.LoadNextPageAsync();
            Transaction.Items= TransactionDetailManager.Items;
            NotifyOfPropertyChange(() => Transaction);
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

        public async Task SearchBoxKeyDownHandler(ActionExecutionContext context)
        {
            if (context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
            {
                await Search();
            }
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

        public void FileDescriptions_TextChanged()
        {
            CanSaveTransactionChanges = true;
        }

        public void CountAndAmount_TextChanged()
        {
            NotifyOfPropertyChange(() => WorkItem);
        }

        private async Task GetComboboxItems()
        {
            ProductItemsForComboBox = await Singleton.ReloadProductNames();
            TransactionsForComboBox = await Singleton.ReloadTransactionNames(TransactionDetailManager == null ? 0 : TransactionDetailManager.TransactionId);
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }

    public class BooleanToReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
         => !(bool?)value ?? true;

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
         => !(value as bool?);
    }
}