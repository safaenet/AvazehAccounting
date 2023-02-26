using Caliburn.Micro;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using System.Threading.Tasks;
using AvazehApiClient.DataAccess;
using System.Windows.Input;
using System.Collections.ObjectModel;
using SharedLibrary.Enums;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using SharedLibrary.SecurityAndSettingsModels;
using SharedLibrary.Helpers;

namespace AvazehWpf.ViewModels;

public class TransactionDetailViewModel : ViewAware
{
    public TransactionDetailViewModel(ITransactionCollectionManager iManager, ITransactionDetailManager dManager, LoggedInUser_DTO user, SingletonClass singleton, int? TransactionId, Func<Task> callBack)
    {
        TCM = iManager;
        TDM = dManager;
        User = user;
        LoadSettings();
        CurrentPersianDate = PersianCalendarHelper.GetCurrentPersianDate();
        CallBackFunc = callBack;
        Singleton = singleton;
        if (TransactionId is not null)
        {
            TDM.TransactionId = (int)TransactionId;
            _ = ReloadTransactionAsync(TransactionId).ConfigureAwait(true);
        }
        else _ = GetComboboxItemsAsync().ConfigureAwait(true);
    }

    private void LoadSettings()
    {
        CanEditTransaction = TCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanEditTransaction));
        CanDeleteTransaction = TCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanDeleteTransaction));
        CanDeleteTransactionItem = TCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanDeleteTransactionItem));
        CanPrintTransaction = TCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanPrintTransaction));
    }

    private readonly ITransactionCollectionManager TCM;
    private readonly ITransactionDetailManager TDM;
    public LoggedInUser_DTO User { get; init; }
    public string CurrentPersianDate { get; init; }
    private SingletonClass Singleton;
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

    private bool canEditTransaction;
    public bool CanEditTransaction
    {
        get { return canEditTransaction; }
        set { canEditTransaction = value; NotifyOfPropertyChange(() => CanEditTransaction); }
    }

    private bool canDeleteTransaction;
    public bool CanDeleteTransaction
    {
        get { return canDeleteTransaction; }
        set { canDeleteTransaction = value; NotifyOfPropertyChange(() => CanDeleteTransaction); }
    }

    private bool canDeleteTransactionItem;
    public bool CanDeleteTransactionItem
    {
        get { return canDeleteTransactionItem; }
        set { canDeleteTransactionItem = value; NotifyOfPropertyChange(() => CanDeleteTransactionItem); }
    }

    private bool canPrintTransaction;
    public bool CanPrintTransaction
    {
        get { return canPrintTransaction; }
        set { canPrintTransaction = value; NotifyOfPropertyChange(() => CanPrintTransaction); }
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

    private async Task ReloadTransactionAsync(int? TransactionId)
    {
        if (TransactionId is null || (int)TransactionId == 0) return;
        Transaction = await TCM.GetItemById((int)TransactionId);
        WindowTitle = Transaction.FileName + " - فایل";
        await GetComboboxItemsAsync();
        await SearchAsync();
    }

    public void EditItem() //DataGrid doubleClick event
    {
        if (!CanEditTransaction || Transaction == null || SelectedItem == null) return;
        EdittingItem = true;
        SelectedItem.Clone(WorkItem);
        SelectedItem.Clone(selectedItem_Backup);
        NotifyOfPropertyChange(() => WorkItem);
    }

    public async Task AddOrUpdateItemAsync()
    {
        if (!CanEditTransaction || Transaction == null || WorkItem == null) return;
        CanEditTransaction = false;
        WorkItem.TransactionId = Transaction.Id;
        var validate = TDM.ValidateItem(WorkItem);
        if (validate.IsValid)
        {
            if (EdittingItem == false) //New Item
            {
                if (Transaction.Items == null) Transaction.Items = new();
                var addedItem = await TDM.CreateItemAsync(WorkItem);
                if (addedItem is not null)
                {
                    Transaction.Items.Insert(0, addedItem);
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
                                    await TDM.CreateItemAsync(WorkItem);
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
                await UpdateItemInDatabaseAsync(WorkItem);
                EdittingItem = false;
            }
        }
        else
        {
            var str = "";
            foreach (var error in validate.Errors)
                str += error.ErrorMessage + "\n";
            MessageBox.Show(str, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        WorkItem = new();
        selectedItem_Backup = new();
        CanEditTransaction = true;
        NotifyOfPropertyChange(() => Transaction.Items);
        NotifyOfPropertyChange(() => Transaction);
        FocusOnProductsCombobox();
    }

    public void FocusOnProductsCombobox()
    {
        ((GetView() as Window).FindName("ProductsCombobox") as ComboBox).Focus();
    }

    private async Task UpdateItemInDatabaseAsync(TransactionItemModel item)
    {
        var ResultItem = await TDM.UpdateItemAsync(item);
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

    public async Task DeleteItemAsync()
    {
        if (!CanDeleteTransactionItem || Transaction == null || Transaction.Items == null || Transaction.Items.Count == 0 || SelectedItem == null || SelectedItem.Id == 0) return;
        var result = MessageBox.Show("Are you sure you want to delete this row ?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        if (result == MessageBoxResult.No) return;
        if (await TDM.DeleteItemAsync(SelectedItem.Id))
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
            _ = DeleteItemAsync().ConfigureAwait(true);
            e.Handled = true;
        }
    }

    public async Task DeleteTransactionAndCloseAsync()
    {
        if (!CanDeleteTransaction || Transaction == null) return;
        var result = MessageBox.Show("Are you sure ?", $"Delete Transaction file {Transaction.FileName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        if (result == MessageBoxResult.No) return;
        if (await TCM.DeleteItemAsync(Transaction.Id) == false) MessageBox.Show($"Transaction with ID: {Transaction.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        CloseWindow();
    }

    private void RefreshDataGrid()
    {
        TransactionModel temp;
        temp = Transaction;
        Transaction = null;
        Transaction = temp;
    }

    public async Task SaveTransactionChangesAsync()
    {
        if (!CanEditTransaction) return;
        var result = await TCM.UpdateItemAsync(Transaction);
        if (result == null)
        {
            MessageBox.Show("خطا در ذخیره تغییرات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        Transaction.DateUpdated = result.DateUpdated;
        Transaction.TimeUpdated = result.TimeUpdated;
        RefreshDataGrid();
        CanSaveTransactionChanges = false;
    }

    public void PrintTransactionMenu(object sender, object window)
    {
        if (!CanPrintTransaction) return;
        ContextMenu cm = (window as Window).FindResource("PrintTransactionCM") as ContextMenu;
        cm.PlacementTarget = sender as Button;
        cm.IsOpen = true;
    }

    public async Task PrintTransactionAsync(int t)
    {
        if (!CanPrintTransaction || Transaction == null) return;
        await ReloadTransactionAsync(Transaction.Id);
        PrintTransactionModel pim = new();
        pim.PrintSettings = User.PrintSettings;
        Transaction.AsPrintModel(pim);
        pim.TransactionType = t;
        XmlSerializer xmlSerializer = new(pim.GetType());
        StringWriter stringWriter = new();
        xmlSerializer.Serialize(stringWriter, pim);
        var UniqueFileName = $@"{DateTime.Now.Ticks}.xml";
        string TempFolderName = "Temp";
        Directory.CreateDirectory(TempFolderName);
        var FilePath = AppDomain.CurrentDomain.BaseDirectory + TempFolderName + @"\" + UniqueFileName;
        File.WriteAllText(FilePath, stringWriter.ToString());
        var PrintInterfacePath = AppDomain.CurrentDomain.BaseDirectory + @"Print\PrintInterface.exe";
        var arguments = "transaction \"" + FilePath + "\"";
        Process p = new Process
        {
            StartInfo = new ProcessStartInfo(PrintInterfacePath, arguments)
        };
        p.Start();
    }

    public async Task SearchAsync()
    {
        TransactionFinancialStatus? FinStatus = SelectedFinStatus >= Enum.GetNames(typeof(TransactionFinancialStatus)).Length ? null : (TransactionFinancialStatus)SelectedFinStatus;
        TDM.SearchValue = SearchText;
        EdittingItem = false;
        WorkItem = new();
        TDM.FinStatus = FinStatus;
        await TDM.LoadFirstPageAsync();
        Transaction.Items = TDM.Items;
        NotifyOfPropertyChange(() => Transaction);
    }

    public async Task PreviousPageAsync()
    {
        await TDM.LoadPreviousPageAsync();
        Transaction.Items = TDM.Items;
        NotifyOfPropertyChange(() => Transaction);
    }

    public async Task NextPageAsync()
    {
        await TDM.LoadNextPageAsync();
        Transaction.Items = TDM.Items;
        NotifyOfPropertyChange(() => Transaction);
    }

    public void CloseWindow()
    {
        (GetView() as Window).Close();
    }

    public async Task ClosingWindowAsync()
    {
        if (CallBackFunc != null) await CallBackFunc?.Invoke();
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

    public void Window_PreviewKeyDown(object window, KeyEventArgs e)
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

    public async Task InputArea_PreviewKeyDownAsync(object window, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (!User.UserSettings.SearchWhenTyping && ((window as Window).FindName("SearchText") as TextBox).IsFocused)
            {
                await SearchAsync();
            }
            else await AddOrUpdateItemAsync();
        }
    }

    public async Task SearchBoxTextChangedHandlerAsync()
    {
        if (User.UserSettings.SearchWhenTyping)
        {
            await SearchAsync();
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

    private async Task GetComboboxItemsAsync()
    {
        ProductItemsForComboBox = await Singleton.ReloadProductNamesAndTransactionItems(Transaction == null ? 0 : Transaction.Id);
        TransactionsForComboBox = await Singleton.ReloadTransactionNames(TDM == null ? 0 : TDM.TransactionId);
    }

    void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
    }

    public void Textbox_GotFocus(object sender)
    {
        (sender as TextBox).SelectAll();
    }

    public void SetKeyboardLayout()
    {
        if (User.UserSettings.AutoSelectPersianLanguage)
            ExtensionsAndStatics.ChangeLanguageToPersian();
    }
}