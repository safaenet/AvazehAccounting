using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.SecurityAndSettingsModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AvazehWpf.ViewModels;

public class NewInvoiceViewModel : Screen
{
    public NewInvoiceViewModel(SingletonClass singleton, int? InvoiceId, IInvoiceCollectionManager icManager, ICollectionManager<CustomerModel> ccManager, Func<Task> callBack, LoggedInUser_DTO user, SimpleContainer sc)
    {
        ICM = icManager;
        CCM = ccManager;
        User = user;
        SC = sc;
        Singleton = singleton;
        InvoiceID = InvoiceId;
        _ = GetComboboxItemsAsync().ConfigureAwait(true);
        CallBack = callBack;
        if (InvoiceID == null || InvoiceID == 0)
        {
            ButtonTitle = "ذخیره";
            WindowTitle = "فاکتور جدید";
        }
        else
        {
            ButtonTitle = "بروزرسانی";
            _ = LoadSelectedItemAsync().ConfigureAwait(true);
        }
    }
    public IInvoiceCollectionManager ICM { get; set; }
    private LoggedInUser_DTO User;
    private SimpleContainer SC;
    public ICollectionManager<CustomerModel> CCM { get; set; }
    private SingletonClass Singleton;
    private readonly int? InvoiceID;
    private int OldCustomerId;
    private string OldInvoiceAbout;
    private ObservableCollection<ItemsForComboBox> customerNames;
    private ObservableCollection<ItemsForComboBox> invoiceAbouts;
    public ObservableCollection<ItemsForComboBox> CustomerNamesForComboBox { get => customerNames; set { customerNames = value; NotifyOfPropertyChange(() => CustomerNamesForComboBox); } }
    public ObservableCollection<ItemsForComboBox> InvoiceAboutsForComboBox { get => invoiceAbouts; set { invoiceAbouts = value; NotifyOfPropertyChange(() => InvoiceAboutsForComboBox); } }
    private bool isCustomerInputDropDownOpen;
    private bool isInvoiceAboutInputDropDownOpen;
    private ItemsForComboBox _selectedCustomer;
    private ItemsForComboBox _selectedInvoiceAbout;
    private string customerInput;
    private string invoiceAboutInput;
    private string buttonTitle;
    private Func<Task> CallBack;
    private string windowTitle;

    public string WindowTitle
    {
        get { return windowTitle; }
        set { windowTitle = value; NotifyOfPropertyChange(() => WindowTitle); }
    }

    public string ButtonTitle
    {
        get { return buttonTitle; }
        set { buttonTitle = value; NotifyOfPropertyChange(() => ButtonTitle); }
    }

    public bool IsCustomerInputDropDownOpen { get => isCustomerInputDropDownOpen; set { isCustomerInputDropDownOpen = value; NotifyOfPropertyChange(() => IsCustomerInputDropDownOpen); } }
    public bool IsInvoiceAboutInputDropDownOpen { get => isInvoiceAboutInputDropDownOpen; set { isInvoiceAboutInputDropDownOpen = value; NotifyOfPropertyChange(() => IsInvoiceAboutInputDropDownOpen); } }

    public ItemsForComboBox SelectedCustomer
    {
        get => _selectedCustomer;
        set { _selectedCustomer = value; NotifyOfPropertyChange(() => SelectedCustomer); }
    }
    public ItemsForComboBox SelectedInvoiceAbout
    {
        get => _selectedInvoiceAbout;
        set { _selectedInvoiceAbout = value; NotifyOfPropertyChange(() => SelectedInvoiceAbout); }
    }

    public string CustomerInput
    {
        get => customerInput;
        set { customerInput = value; NotifyOfPropertyChange(() => CustomerInput); }
    }

    public string InvoiceAboutInput
    {
        get => invoiceAboutInput;
        set { invoiceAboutInput = value; NotifyOfPropertyChange(() => InvoiceAboutInput); }
    }

    private async Task GetComboboxItemsAsync()
    {
        CustomerNamesForComboBox = await Singleton.ReloadCustomerNames();
        InvoiceAboutsForComboBox = await Singleton.ReloadInvoiceAbouts();
    }

    public void CustomerNames_KeyUp(KeyEventArgs e)
    {
        if (CustomerNamesForComboBox == null || CustomerNamesForComboBox.ToList().Find(x => x.ItemName.StartsWith(CustomerInput ?? string.Empty)) == null) IsCustomerInputDropDownOpen = false;
        else IsCustomerInputDropDownOpen = true;

        if (e.Key == Key.Enter) IsCustomerInputDropDownOpen = false;
        e.Handled = true;
    }

    public void InvoiceAbout_KeyUp(KeyEventArgs e)
    {
        if (InvoiceAboutsForComboBox == null || InvoiceAboutsForComboBox.ToList().Find(x => x.ItemName.StartsWith(InvoiceAboutInput ?? string.Empty)) == null) IsInvoiceAboutInputDropDownOpen = false;
        else IsInvoiceAboutInputDropDownOpen = true;

        if (e.Key == Key.Enter) IsInvoiceAboutInputDropDownOpen = false;
        e.Handled = true;
    }

    public async Task AddOrUpdateInvoiceAsync()
    {
        var c = await CheckCustomerAsync();
        if (c == null)
        {
            MessageBox.Show("خطا هنگام ایجاد مشتری جدید", CustomerInput, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (InvoiceID is null) //Add new
        {
            InvoiceModel i = new();
            i.Customer = c;
            i.About = string.IsNullOrWhiteSpace(InvoiceAboutInput) ? null : InvoiceAboutInput;
            var newInvoice = await ICM.CreateItemAsync(i);
            if (newInvoice != null)
            {
                WindowManager wm = new();
                var idm = SC.GetInstance<IInvoiceDetailManager>();
                await wm.ShowWindowAsync(new InvoiceDetailViewModel(ICM, idm, User, Singleton, newInvoice.Id, null, SC));
                await TryCloseAsync();
            }
            else MessageBox.Show("خطا هنگام ایجاد فاکتور جدید", CustomerInput, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else //Update Owner
        {
            if (c.Id != OldCustomerId || InvoiceAboutInput != OldInvoiceAbout)
            {
                int id = (int)InvoiceID;
                var invoice = await ICM.GetItemById(id);
                if (invoice == null)
                {
                    MessageBox.Show("Cannot find such invoice.");
                    return;
                }
                if (MessageBox.Show("آیا مالک فاکتور و/یا فیلد بابت فاکتور تغییر کند؟", "تغییر مالک", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No) return;
                invoice.Customer = c;
                invoice.About = string.IsNullOrWhiteSpace(InvoiceAboutInput) ? null : InvoiceAboutInput;
                await ICM.UpdateItemAsync(invoice);
            }
            await TryCloseAsync();
        }
    }

    private async Task<CustomerModel> CheckCustomerAsync()
    {
        if (SelectedCustomer != null) return await CCM.GetItemById(SelectedCustomer.Id);
        var res = MessageBox.Show("مشتری وارد شده وجود ندارد. آیا ایجاد شود؟", "ایجاد مشتری جدید", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
        if (res == MessageBoxResult.Yes)
        {
            CustomerModel TempCustomer = new();
            TempCustomer.FullName = customerInput;
            var newCustomer = await CCM.CreateItemAsync(TempCustomer);
            return newCustomer;
        }
        return null;
    }

    private async Task LoadSelectedItemAsync()
    {
        var invoice = await ICM.GetItemById((int)InvoiceID);
        SelectedCustomer = CustomerNamesForComboBox.SingleOrDefault(c => c.Id == invoice.Customer.Id);
        OldCustomerId = invoice.Customer.Id;
        OldInvoiceAbout = invoice.About;
        CustomerInput = invoice.Customer.FullName;
        InvoiceAboutInput = invoice.About;
        WindowTitle = invoice.Customer.FullName + " - تغییر نام";
    }

    public void WindowLoaded()
    {
        ((GetView() as Window).FindName("NewInvoiceOwnerInput") as ComboBox).Focus();
        IsCustomerInputDropDownOpen = true;
    }

    public async Task ClosingWindowAsync()
    {
        if (CallBack != null) await CallBack?.Invoke();
    }

    public void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
            (GetView() as Window).Close();
    }

    public void SetKeyboardLayout()
    {
        if (User.UserSettings.AutoSelectPersianLanguage)
            ExtensionsAndStatics.ChangeLanguageToPersian();
    }
}