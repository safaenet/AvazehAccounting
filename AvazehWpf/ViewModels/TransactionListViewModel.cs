using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using SharedLibrary.SecurityAndSettingsModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels;

public class TransactionListViewModel : Screen
{
    public TransactionListViewModel(ITransactionCollectionManager manager, SingletonClass singleton, LoggedInUser_DTO user, SimpleContainer sc)
    {
        TCM = manager;
        User = user;
        CurrentPersianDate = new PersianCalendar().GetPersianDate();
        SC = sc;
        _SelectedTransaction = new();
        Singleton = singleton;
        QueryDate = new PersianCalendar().GetYear(DateTime.Now).ToString() + "/__/__";
        _ = LoadSettingsAsync().ConfigureAwait(true);
    }

    SimpleContainer SC;
    private ITransactionCollectionManager _TCM;
    private TransactionListModel _SelectedTransaction;
    private readonly SingletonClass Singleton;
    public LoggedInUser_DTO User { get; init; }
    public string CurrentPersianDate { get; init; }

    private string queryDate;

    public string QueryDate
    {
        get => queryDate;
        set { queryDate = value; NotifyOfPropertyChange(() => QueryDate); }
    }

    private string transactionIdToSearch;

    public string TransactionIdToSearch
    {
        get { return transactionIdToSearch; }
        set { transactionIdToSearch = value; NotifyOfPropertyChange(() => TransactionIdToSearch); }
    }

    public TransactionListModel SelectedTransaction
    {
        get { return _SelectedTransaction; }
        set { _SelectedTransaction = value; NotifyOfPropertyChange(() => SelectedTransaction); }
    }

    public ITransactionCollectionManager TCM
    {
        get { return _TCM; }
        set
        {
            _TCM = value;
            NotifyOfPropertyChange(() => TCM);
            NotifyOfPropertyChange(() => Transactions);
        }
    }

    public ObservableCollection<TransactionListModel> Transactions
    {
        get => TCM.Items;
        set
        {
            TCM.Items = value;
            NotifyOfPropertyChange(() => TCM);
            NotifyOfPropertyChange(() => Transactions);
        }
    }

    public string SearchText { get; set; }
    public int SelectedFinStatus { get; set; } = 3;

    private bool canViewTransactionDetails;
    public bool CanViewTransactionDetails
    {
        get { return canViewTransactionDetails; }
        set { canViewTransactionDetails = value; NotifyOfPropertyChange(() => CanViewTransactionDetails); }
    }

    private bool canAddNewTransaction;
    public bool CanAddNewTransaction
    {
        get { return canAddNewTransaction; }
        set { canAddNewTransaction = value; NotifyOfPropertyChange(() => CanAddNewTransaction); }
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

    private bool canPrintTransaction;
    public bool CanPrintTransaction
    {
        get { return canPrintTransaction; }
        set { canPrintTransaction = value; NotifyOfPropertyChange(() => CanPrintTransaction); }
    }

    private async Task LoadSettingsAsync()
    {
        CanViewTransactionDetails = TCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanViewTransactionDetails));
        CanAddNewTransaction = TCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanAddNewTransaction));
        CanEditTransaction = TCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanEditTransaction));
        CanDeleteTransaction = TCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanDeleteTransaction));
        CanPrintTransaction = TCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanPrintTransaction));
        TCM.PageSize = User.UserSettings.TransactionListPageSize;
        TCM.QueryOrderType = User.UserSettings.TransactionListQueryOrderType;
        await SearchAsync();
    }

    public async Task AddNewTransactionAsync()
    {
        WindowManager wm = new();
        await wm.ShowDialogAsync(new NewTransactionViewModel(Singleton, null, TCM, RefreshPageAsync, User, SC));
    }

    public async Task PreviousPageAsync()
    {
        await TCM.LoadPreviousPageAsync();
        NotifyOfPropertyChange(() => Transactions);
    }

    public async Task NextPageAsync()
    {
        await TCM.LoadNextPageAsync();
        NotifyOfPropertyChange(() => Transactions);
    }

    public async Task RefreshPageAsync()
    {
        await TCM.RefreshPage();
        NotifyOfPropertyChange(() => Transactions);
    }

    public async Task SearchAsync()
    {
        TransactionFinancialStatus? FinStatus = SelectedFinStatus >= Enum.GetNames(typeof(TransactionFinancialStatus)).Length ? null : (TransactionFinancialStatus)SelectedFinStatus;
        TCM.SearchValue = SearchText;
        TCM.FinStatus = FinStatus;
        int.TryParse(TransactionIdToSearch, out var tnsId);
        TCM.TransactionIdToSearch = tnsId <= 0 ? -1 : tnsId;
        TCM.TransactionDateToSearch = QueryDate;
        await TCM.LoadFirstPageAsync();
        NotifyOfPropertyChange(() => Transactions);
    }

    public async Task SearchBoxKeyDownHandlerAsync(ActionExecutionContext context)
    {
        if (!User.UserSettings.SearchWhenTyping && context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
        {
            await SearchAsync();
        }
    }

    public async Task SearchBoxTextChangedHandlerAsync()
    {
        if (User.UserSettings.SearchWhenTyping)
        {
            await SearchAsync();
        }
    }

    public async Task EditTransactionAsync()
    {
        if (!CanViewTransactionDetails || Transactions == null || Transactions.Count == 0 || SelectedTransaction == null || SelectedTransaction.Id == 0) return;
        var tdm = SC.GetInstance<ITransactionDetailManager>();
        WindowManager wm = new();
        await wm.ShowWindowAsync(new TransactionDetailViewModel(TCM, tdm, User, Singleton, SelectedTransaction.Id, RefreshPageAsync));
    }

    public async Task DeleteTransactionAsync()
    {
        if (!CanDeleteTransaction || Transactions == null || Transactions.Count == 0 || SelectedTransaction == null || SelectedTransaction.Id == 0) return;
        var result = MessageBox.Show("Are you sure ?", $"Delete Transaction file {SelectedTransaction.FileName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        if (result == MessageBoxResult.No) return;
        var output = await TCM.DeleteItemAsync(SelectedTransaction.Id);
        if (output) Transactions.Remove(SelectedTransaction);
        else MessageBox.Show($"Transaction with ID: {SelectedTransaction.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public async Task RenameTransactionAsync()
    {
        if (!CanEditTransaction || SelectedTransaction == null) return;
        WindowManager wm = new();
        await wm.ShowDialogAsync(new NewTransactionViewModel(Singleton, SelectedTransaction.Id, TCM, RefreshPageAsync, User, SC));
    }

    public void dg_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Key.Delete == e.Key)
        {
            _ = DeleteTransactionAsync().ConfigureAwait(true);
            e.Handled = true;
        }
    }

    public void Window_PreviewKeyDown(object window, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) (GetView() as Window).Close();
    }

    public void SetKeyboardLayout()
    {
        if (User.UserSettings.AutoSelectPersianLanguage)
            ExtensionsAndStatics.ChangeLanguageToPersian();
    }
}

public static class TransactionFinStatusItems //For ComboBoxes
{
    public static Dictionary<int, string> GetTransactionFinStatusItems()
    {
        Dictionary<int, string> choices = new();
        for (int i = 0; i < Enum.GetNames(typeof(TransactionFinancialStatus)).Length; i++)
        {
            if (Enum.GetName(typeof(TransactionFinancialStatus), i) == TransactionFinancialStatus.Balanced.ToString())
                choices.Add((int)TransactionFinancialStatus.Balanced, "تسویه");
            else if (Enum.GetName(typeof(TransactionFinancialStatus), i) == TransactionFinancialStatus.Positive.ToString())
                choices.Add((int)TransactionFinancialStatus.Positive, "مثبت");
            else if (Enum.GetName(typeof(TransactionFinancialStatus), i) == TransactionFinancialStatus.Negative.ToString())
                choices.Add((int)TransactionFinancialStatus.Negative, "منفی");
        }
        choices.Add(Enum.GetNames(typeof(TransactionFinancialStatus)).Length, "همه");
        return choices;
    }
}