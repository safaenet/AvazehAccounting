using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.Contracts;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels;

public class PrevInvoiceListViewModel : Screen
{
    public PrevInvoiceListViewModel(IInvoiceCollectionManager manager, int InvoiceId, LoggedInUser_DTO user, string CustomerFullName = "Customer")
    {
        ICM = manager;
        User = user;
        //_SelectedInvoice = new();
        WindowTitle = "فاکتورهای " + CustomerFullName;
        this.InvoiceId = InvoiceId;
        QueryDate = "____/__/__";
        CurrentPersianDate = PersianCalendarHelper.GetCurrentPersianDate();
        _ = SearchAsync().ConfigureAwait(true);
    }

    private IInvoiceCollectionManager _ICM;
    private InvoiceListDTO _SelectedInvoice;
    public LoggedInUser_DTO User { get; init; }
    public int? ReturnId = null;
    public string CurrentPersianDate { get; set; }

    public InvoiceListDTO SelectedInvoice
    {
        get { return _SelectedInvoice; }
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

    private ObservableCollection<InvoiceListDTO> invoices;
    public ObservableCollection<InvoiceListDTO> Invoices
    {
        get => invoices;
        set
        {
            invoices = value;
            NotifyOfPropertyChange(() => ICM);
            NotifyOfPropertyChange(() => Invoices);
        }
    }

    private bool uiEnabled = true;

    public bool UIEnabled
    {
        get { return uiEnabled; }
        set { uiEnabled = value; NotifyOfPropertyChange(() => UIEnabled); }
    }

    private string queryDate;

    public string QueryDate
    {
        get { return queryDate; }
        set { queryDate = value; NotifyOfPropertyChange(() => QueryDate); }
    }

    private string searchText;

    public string SearchText
    {
        get { return searchText; }
        set { searchText = value; NotifyOfPropertyChange(() => SearchText); }
    }


    public int InvoiceId { get; set; }

    public string WindowTitle { get; set; }

    public async Task SearchAsync()
    {
        UIEnabled = false;
        var items = await ICM.LoadPrevInvoices(InvoiceId, QueryDate, SearchText, OrderType.DESC);
        Invoices = items;
        UIEnabled = true;
        NotifyOfPropertyChange(() => Invoices);
    }

    public void SearchSync()
    {
        _ = Task.Run(SearchAsync);
    }

    public async Task SelectInvoiceAndCloseAsync()
    {
        if (SelectedInvoice != null)
        {
            ReturnId = SelectedInvoice.Id;
            await TryCloseAsync();
        }
    }

    public async Task Dg_PreviewKeyDownAsync(object sender, KeyEventArgs e)
    {
        if (Key.Enter == e.Key)
        {
            await SelectInvoiceAndCloseAsync();
            e.Handled = true;
        }
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