using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;
using SharedLibrary.SecurityAndSettingsModels;
using SharedLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace AvazehWpf.ViewModels;

public class InvoiceListViewModel : Screen
{
    public InvoiceListViewModel(IInvoiceCollectionManager manager, SingletonClass singleton, LoggedInUser_DTO user, SimpleContainer sc)
    {
        ICM = manager;
        User = user;
        CurrentPersianDate = PersianCalendarHelper.GetCurrentPersianDate();
        SC = sc;
        _SelectedInvoice = new();
        Singleton = singleton;
        LoadSettings();
        ICM.PageSize = User.UserSettings.InvoicePageSize;
        ICM.orderType = User.UserSettings.InvoiceListQueryOrderType;
        ICM.LifeStatus = InvoiceLifeStatus.Active;
        ICM.SearchMode = SqlQuerySearchMode.Backward;
        ICM.FinStatus = null;
        
        QueryDate = new System.Globalization.PersianCalendar().GetYear(DateTime.Now).ToString() + "/__/__";

        _ = SearchAsync().ConfigureAwait(true);
    }

    private void LoadSettings()
    {
        CanAddNewInvoice = ICM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanAddNewInvoice));
        CanEditInvoice = ICM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanEditInvoice));
        CanViewInvoiceDetails = ICM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanViewInvoiceDetails));
        CanDeleteInvoice = ICM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanDeleteInvoice));
        CanPrintInvoice = ICM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanPrintInvoice));
    }

    readonly SimpleContainer SC;
    private IInvoiceCollectionManager _ICM;
    public LoggedInUser_DTO User { get => user; init => user = value; }
    public string CurrentPersianDate { get; init; }
    private InvoiceListModel _SelectedInvoice;
    private string searchText;
    private LoggedInUser_DTO user;
    private readonly SingletonClass Singleton;
    private string invoiceIdToSearch;

    public string InvoiceIdToSearch
    {
        get { return invoiceIdToSearch; }
        set { invoiceIdToSearch = value;  NotifyOfPropertyChange(() => InvoiceIdToSearch); }
    }

    private string queryDate;

    public string QueryDate
    {
        get => queryDate;
        set { queryDate = value; NotifyOfPropertyChange(() => QueryDate); }
    }

    public InvoiceListModel SelectedInvoice
    {
        get => _SelectedInvoice;
        set { _SelectedInvoice = value; NotifyOfPropertyChange(() => SelectedInvoice); }
    }

    public IInvoiceCollectionManager ICM
    {
        get { return _ICM; }
        set
        {
            _ICM = value;
            NotifyOfPropertyChange(() => ICM);
            NotifyOfPropertyChange(() => Invoices);
        }
    }

    public ObservableCollection<InvoiceListModel> Invoices
    {
        get => ICM.Items;
        set
        {
            ICM.Items = value;
            NotifyOfPropertyChange(() => ICM);
            NotifyOfPropertyChange(() => Invoices);
        }
    }

    public string SearchText
    {
        get => searchText; set
        {
            searchText = value;
            NotifyOfPropertyChange(() => SearchText);
        }
    }
    public int SelectedFinStatus { get; set; } = 1;
    public int SelectedLifeStatus { get; set; }

    private bool canAddNewInvoice;
    public bool CanAddNewInvoice
    {
        get { return canAddNewInvoice; }
        set { canAddNewInvoice = value; NotifyOfPropertyChange(() => CanAddNewInvoice); }
    }

    private bool canEditInvoice;
    public bool CanEditInvoice
    {
        get { return canEditInvoice; }
        set { canEditInvoice = value; NotifyOfPropertyChange(() => CanEditInvoice); }
    }

    private bool canViewInvoiceDetails;
    public bool CanViewInvoiceDetails
    {
        get { return canViewInvoiceDetails; }
        set { canViewInvoiceDetails = value; NotifyOfPropertyChange(() => CanViewInvoiceDetails); }
    }

    private bool canDeleteInvoice;
    public bool CanDeleteInvoice
    {
        get { return canDeleteInvoice; }
        set { canDeleteInvoice = value; NotifyOfPropertyChange(() => CanDeleteInvoice); }
    }

    private bool canPrintInvoice;
    public bool CanPrintInvoice
    {
        get { return canPrintInvoice; }
        set { canPrintInvoice = value; NotifyOfPropertyChange(() => CanPrintInvoice); }
    }

    public async Task PreviousPageAsync()
    {
        await ICM.LoadPreviousPageAsync();
        NotifyOfPropertyChange(() => Invoices);
    }

    public async Task NextPageAsync()
    {
        await ICM.LoadNextPageAsync();
        NotifyOfPropertyChange(() => Invoices);
    }

    public async Task RefreshPageAsync()
    {
        await ICM.RefreshPage();
        NotifyOfPropertyChange(() => Invoices);
    }

    public async Task AddNewInvoiceAsync()
    {
        if (!CanAddNewInvoice) return;
        WindowManager wm = new();
        ICollectionManager<CustomerModel> cManager = new CustomerCollectionManagerAsync<CustomerModel, CustomerModel_DTO_Create_Update, CustomerValidator>(ICM.ApiProcessor);
        await wm.ShowDialogAsync(new NewInvoiceViewModel(Singleton, null, ICM, cManager, SearchAsync, User, SC));
    }

    public async Task SearchAsync()
    {
        InvoiceFinancialStatus? FinStatus = SelectedFinStatus >= Enum.GetNames(typeof(InvoiceFinancialStatus)).Length ? null : (InvoiceFinancialStatus)SelectedFinStatus;
        InvoiceLifeStatus? LifeStatus = SelectedLifeStatus >= Enum.GetNames(typeof(InvoiceLifeStatus)).Length ? null : (InvoiceLifeStatus)SelectedLifeStatus;
        ICM.SearchValue = SearchText;
        ICM.FinStatus = FinStatus;
        ICM.LifeStatus = LifeStatus;
        int.TryParse(InvoiceIdToSearch, out var invId);
        ICM.InvoiceIdToSearch = invId <= 0 ? -1 : invId;
        ICM.InvoiceDateToSearch = QueryDate;
        await ICM.LoadFirstPageAsync();
        NotifyOfPropertyChange(() => Invoices);
    }

    public void SearchSync()
    {
        _ = Task.Run(SearchAsync);
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

    public async Task EditInvoiceAsync()
    {
        if (!CanViewInvoiceDetails || Invoices == null || Invoices.Count == 0 || SelectedInvoice == null || SelectedInvoice.Id == 0) return;
        var idm = SC.GetInstance<IInvoiceDetailManager>();
        WindowManager wm = new();
        await wm.ShowWindowAsync(new InvoiceDetailViewModel(ICM, idm, User, Singleton, SelectedInvoice.Id, RefreshPageAsync, SC));
    }

    public async Task DeleteInvoiceAsync()
    {
        if (!CanDeleteInvoice || Invoices == null || Invoices.Count == 0 || SelectedInvoice == null || SelectedInvoice.Id == 0) return;
        var result = MessageBox.Show("Are you sure ?", $"Delete invoice of {SelectedInvoice.CustomerFullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        if (result == MessageBoxResult.No) return;
        var output = await ICM.DeleteItemAsync(SelectedInvoice.Id);
        if (output) Invoices.Remove(SelectedInvoice);
        else MessageBox.Show($"Invoice with ID: {SelectedInvoice.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public async Task ViewPaymentsAsync()
    {
        if (!CanViewInvoiceDetails || SelectedInvoice is null) return;
        WindowManager wm = new();
        var invoice = await ICM.GetItemById(SelectedInvoice.Id);
        var idm = SC.GetInstance<IInvoiceDetailManager>();
        await wm.ShowWindowAsync(new InvoicePaymentsViewModel(ICM, idm, User, invoice, SearchSync, SC, true));
    }

    public async Task ShowCustomerInvoicesAsync()
    {
        if (SelectedInvoice is null) return;
        SearchText = SelectedInvoice.CustomerFullName;
        await SearchAsync();
    }

    public async Task ShowAllInvoicesAsync()
    {
        SearchText = "";
        await SearchAsync();
    }

    public void dg_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Key.Delete == e.Key)
        {
            _ = DeleteInvoiceAsync().ConfigureAwait(true);
            e.Handled = true;
        }
    }

    public async Task PrintInvoiceAsync(int t)
    {
        if (!CanPrintInvoice || Invoices == null || Invoices.Count == 0 || SelectedInvoice == null || SelectedInvoice.Id == 0) return;
        var Invoice = await ICM.GetItemById(SelectedInvoice.Id);
        if (Invoice == null) return;
        PrintInvoiceModel pim = new();
        pim.PrintSettings = User.PrintSettings;
        Invoice.AsPrintModel(pim);
        if (t == 12) pim.PrintSettings.MainHeaderText = "فاکتور فروش";
        else if (t == 13) pim.PrintSettings.MainHeaderText = "فروشگاه آوازه";
        else if (t == 21) pim.PrintSettings.MainHeaderText = "پیش فاکتور";
        else if (t == 22) pim.PrintSettings.MainHeaderText = "فروشگاه آوازه";
        pim.InvoiceType = t;

        XmlSerializer xmlSerializer = new(pim.GetType());
        StringWriter stringWriter = new();
        xmlSerializer.Serialize(stringWriter, pim);
        var UniqueFileName = $@"{DateTime.Now.Ticks}.xml";
        string TempFolderName = "Temp";
        Directory.CreateDirectory(TempFolderName);
        var FilePath = AppDomain.CurrentDomain.BaseDirectory + TempFolderName + @"\" + UniqueFileName;
        File.WriteAllText(FilePath, stringWriter.ToString());
        var PrintInterfacePath = AppDomain.CurrentDomain.BaseDirectory + @"Print\PrintInterface.exe";
        var arguments = "invoice \"" + FilePath + "\"";
        Process p = new Process
        {
            StartInfo = new ProcessStartInfo(PrintInterfacePath, arguments)
        };
        p.Start();
    }

    public async Task PrintInvoiceByShortcutAsync()
    {
        await PrintInvoiceAsync(11);
    }

    public void SetKeyboardLayout()
    {
        if (User.UserSettings.AutoSelectPersianLanguage)
            ExtensionsAndStatics.ChangeLanguageToPersian();
    }

    public void Window_PreviewKeyDown(object window, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) (GetView() as Window).Close();
    }
}

public static class InvoiceFinStatusAndLifeStatusItems //For ComboBoxes
{
    public static Dictionary<int, string> GetInvoiceFinStatusItems()
    {
        Dictionary<int, string> choices = new();
        choices.Add((int)InvoiceFinancialStatus.Balanced, "تسویه");
        choices.Add((int)InvoiceFinancialStatus.Deptor, "بدهکار");
        choices.Add((int)InvoiceFinancialStatus.Creditor, "بستانکار");
        choices.Add(3, "معوق"); //Outstanding
        choices.Add(4, "همه");
        return choices;
    }

    public static Dictionary<int, string> GetInvoiceLifeStatusItems()
    {
        Dictionary<int, string> choices = new();
        for (int i = 0; i < Enum.GetNames(typeof(InvoiceLifeStatus)).Length; i++)
        {
            if (Enum.GetName(typeof(InvoiceLifeStatus), i) == InvoiceLifeStatus.Active.ToString())
                choices.Add((int)InvoiceLifeStatus.Active, "فعال");
            else if (Enum.GetName(typeof(InvoiceLifeStatus), i) == InvoiceLifeStatus.Inactive.ToString())
                choices.Add((int)InvoiceLifeStatus.Inactive, "غیرفعال");
            else if (Enum.GetName(typeof(InvoiceLifeStatus), i) == InvoiceLifeStatus.Archive.ToString())
                choices.Add((int)InvoiceLifeStatus.Archive, "بایگانی");
            else if (Enum.GetName(typeof(InvoiceLifeStatus), i) == InvoiceLifeStatus.Deleted.ToString())
                choices.Add((int)InvoiceLifeStatus.Deleted, "حذف شده");
        }
        choices.Add(Enum.GetNames(typeof(InvoiceLifeStatus)).Length, "همه");
        return choices;
    }
}