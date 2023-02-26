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
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using SharedLibrary.SecurityAndSettingsModels;
using System.Globalization;
using SharedLibrary.Enums;

namespace AvazehWpf.ViewModels;

public class InvoiceDetailViewModel : ViewAware
{
    public InvoiceDetailViewModel(IInvoiceCollectionManager iManager, IInvoiceDetailManager dManager, LoggedInUser_DTO user, SingletonClass singleton, int? InvoiceId, Func<Task> callBack, SimpleContainer sc)
    {
        ICM = iManager;
        IDM = dManager;
        User = user;
        CurrentPersianDate = new PersianCalendar().GetPersianDate();
        SC = sc;
        CallBackFunc = callBack;
        Singleton = singleton;
        LoadSettings();
        _ = LoadInvoiceAsync(InvoiceId).ConfigureAwait(true);
    }

    private void LoadSettings()
    {
        CanEditInvoice = ICM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanEditInvoice));
        CanDeleteInvoice = ICM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanDeleteInvoice));
        CanDeleteInvoiceItem = ICM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanDeleteInvoiceItem));
        CanPrintInvoice = ICM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanPrintInvoice));
        ShowNetProfits = ICM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanViewNetProfits));
    }

    private readonly IInvoiceCollectionManager ICM;
    private readonly IInvoiceDetailManager IDM;
    readonly SimpleContainer SC;
    private readonly SingletonClass Singleton;
    private InvoiceModel _Invoice;
    private readonly Func<Task> CallBackFunc;
    private ObservableCollection<ItemsForComboBox> productItems;
    private ObservableCollection<ProductUnitModel> productUnits;
    private ObservableCollection<RecentSellPriceModel> recentSellPrices;
    private InvoiceItemModel _workItem = new();
    private bool CanUpdateRowFromDB = true; //False when user DoubleClicks on a row.
    private bool EdittingItem = false;
    public LoggedInUser_DTO User { get; init; }
    public int SelectedDiscountType
    {
        get => selectedDiscountType; set
        {
            selectedDiscountType = value;
            NotifyOfPropertyChange(() => SelectedDiscountType);
        }
    }
    public string CurrentPersianDate { get; init; }
    public bool CanSaveInvoiceChanges { get; set; } = true;
    public InvoiceItemModel SelectedItem { get; set; }
    public InvoiceItemModel WorkItem { get => _workItem; set { _workItem = value; NotifyOfPropertyChange(() => WorkItem); } }
    public double CustomerPreviousTotalBalance { get => customerPreviousTotalBalance; private set { customerPreviousTotalBalance = value; NotifyOfPropertyChange(() => CustomerPreviousTotalBalance); } }
    public double CustomerTotalBalancePlusThis { get => customerTotalBalancePlusThis; set { customerTotalBalancePlusThis = value; NotifyOfPropertyChange(() => CustomerTotalBalancePlusThis); } }
    public ObservableCollection<ItemsForComboBox> ProductItemsForComboBox { get => productItems; set { productItems = value; NotifyOfPropertyChange(() => ProductItemsForComboBox); } }
    public ObservableCollection<ProductUnitModel> ProductUnits { get => productUnits; set { productUnits = value; NotifyOfPropertyChange(() => ProductUnits); } }
    public ObservableCollection<RecentSellPriceModel> RecentSellPrices { get => recentSellPrices; set { recentSellPrices = value; NotifyOfPropertyChange(() => RecentSellPrices); } }

    private bool showNetProfits;
    public bool ShowNetProfits
    {
        get => showNetProfits;
        set
        {
            showNetProfits = value;
            NotifyOfPropertyChange(() => ShowNetProfits);
        }
    }

    private ItemsForComboBox _selectedProductItem;
    private bool isSellPriceDropDownOpen;
    private string productInput;
    private double customerPreviousTotalBalance;
    private double customerTotalBalancePlusThis;
    private bool isProductInputDropDownOpen;
    private string windowTitle;
    private string phoneNumberText;
    private int selectedDiscountType;

    public string PhoneNumberText
    {
        get { return phoneNumberText; }
        set { phoneNumberText = value; }
    }

    private bool canEditInvoice;
    public bool CanEditInvoice
    {
        get { return canEditInvoice; }
        set { canEditInvoice = value; NotifyOfPropertyChange(() => CanEditInvoice); }
    }

    private bool canDeleteInvoice;
    public bool CanDeleteInvoice
    {
        get { return canDeleteInvoice; }
        set { canDeleteInvoice = value; NotifyOfPropertyChange(() => CanDeleteInvoice); }
    }

    private bool canDeleteInvoiceItem;
    public bool CanDeleteInvoiceItem
    {
        get { return canDeleteInvoiceItem; }
        set { canDeleteInvoiceItem = value; NotifyOfPropertyChange(() => CanDeleteInvoiceItem); }
    }

    private bool canPrintInvoice;

    public bool CanPrintInvoice
    {
        get { return canPrintInvoice; }
        set { canPrintInvoice = value; NotifyOfPropertyChange(() => CanPrintInvoice); }
    }

    private async Task LoadInvoiceAsync(int? InvoiceId)
    {
        if (InvoiceId is not null)
        {
            await ReloadInvoiceAsync(InvoiceId);
        }
        await GetComboboxItemsAsync();
    }

    public string WindowTitle
    {
        get { return windowTitle; }
        set { windowTitle = value; NotifyOfPropertyChange(() => WindowTitle); }
    }

    public bool IsProductInputDropDownOpen { get => isProductInputDropDownOpen; set { isProductInputDropDownOpen = value; NotifyOfPropertyChange(() => IsProductInputDropDownOpen); } }

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

    public ItemsForComboBox SelectedProductItem
    {
        get => _selectedProductItem;
        set { _selectedProductItem = value; NotifyOfPropertyChange(() => SelectedProductItem); }
    }

    public InvoiceModel Invoice
    {
        get => _Invoice;
        set { _Invoice = value; NotifyOfPropertyChange(() => Invoice); }
    }

    private async Task ReloadInvoiceAsync(int? InvoiceId)
    {
        if (InvoiceId is null || (int)InvoiceId == 0) return;
        Invoice = await ICM.GetItemById((int)InvoiceId);
        SelectedDiscountType = (int)Invoice.DiscountType;
        WindowTitle = Invoice.Customer.FullName + " - فاکتور";
        await ReloadCustomerPreviousBalanceAsync();
    }

    public async Task ReloadCustomerPreviousBalanceAsync()
    {
        if (Invoice is null) return;
        CustomerPreviousTotalBalance = await ICM.GetCustomerTotalBalanceById(Invoice.Customer.Id, Invoice.Id);
        ReloadCustomerTotalBalance();
    }

    private void ReloadCustomerTotalBalance()
    {
        if (Invoice is null) return;
        CustomerTotalBalancePlusThis = CustomerPreviousTotalBalance + (Invoice == null ? 0 : Invoice.TotalBalance);
    }

    public void EditItem() //DataGrid doubleClick event
    {
        if (!CanEditInvoice || Invoice == null || SelectedItem == null) return;
        CanUpdateRowFromDB = false;
        EdittingItem = true;
        SelectedItem.Clone(WorkItem);
        SelectedProductItem = ProductItemsForComboBox.SingleOrDefault(x => x.Id == SelectedItem.Product.Id);
        SelectedProductUnit = SelectedItem.Unit == null || SelectedItem.Unit.Id == 0 ? null : ProductUnits.SingleOrDefault(x => x.Id == SelectedItem.Unit.Id);
        CanUpdateRowFromDB = true;
        NotifyOfPropertyChange(() => WorkItem);
        NotifyOfPropertyChange(() => SelectedProductUnit);
        FocusOnProductsCombobox();
    }

    public async Task AddOrUpdateItemAsync()
    {
        if (!CanEditInvoice || Invoice == null) return;
        CanEditInvoice = false;
        var enableBarcodeReader = ICM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanUseBarcodeReader));
        var pcm = SC.GetInstance<ICollectionManager<ProductModel>>();
        if (SelectedProductItem == null && ProductInput != null && ProductInput.Length > 0 && EdittingItem == false) //Search by Entered text
        {
            if (enableBarcodeReader) //Search Barcode
            {
                var product = await pcm.GetItemByBarCodeAsync(ProductInput);
                if (product != null) //Found by barcode.
                {
                    if (Invoice.Items == null) Invoice.Items = new();
                    var item = Invoice.Items.FirstOrDefault(x => x.Product.Id == product.Id);
                    if (item == null) //If doesnt exsist in list, add new
                    {
                        WorkItem = new();
                        WorkItem.InvoiceId = Invoice.Id;
                        WorkItem.Product = product;
                        WorkItem.SellPrice = product.SellPrice;
                        WorkItem.BuyPrice = product.BuyPrice;
                        //WorkItem.CountString = User.Settings.
                        var addedItem = await IDM.CreateItemAsync(WorkItem);
                        if (addedItem is not null)
                        {
                            //Validate here
                            Invoice.Items.Insert(0, addedItem);
                        }
                    }
                    else //if exists in list, update it to "BarcodeAddItemCount" more
                    {
                        WorkItem = item;
                        WorkItem.CountString = (WorkItem.CountValue + User.GeneralSettings.BarcodeAddItemCount).ToString();
                        await UpdateItemInDatabaseAsync(WorkItem);
                    }
                }
                else if (User.UserSettings.AskToAddNotExistingProduct) //Not found by barcode, Try to create new product.
                {
                    if (MessageBox.Show("نام کالای وارد شده موجود نیست. آیا به لیست کالاها اضافه شود ؟", "اضافه کردن کالا", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                    {
                        ProductModel newProduct = new();
                        newProduct.SellPrice = WorkItem.SellPrice;
                        newProduct.ProductName = ProductInput;
                        if (long.TryParse(ProductInput, out _)) newProduct.Barcode = ProductInput;
                        newProduct.BuyPrice = WorkItem.BuyPrice;
                        var p = await pcm.CreateItemAsync(newProduct);
                        if (p is not null)
                        {
                            ItemsForComboBox item = new() { Id = p.Id, ItemName = p.ProductName };
                            ProductItemsForComboBox.Add(item);
                            SelectedProductItem = item;
                            WorkItem.Id = p.Id;
                            WorkItem.InvoiceId = Invoice.Id;
                            WorkItem.Product = p;
                            //Add newly created product to invoice:
                            if (Invoice.Items == null) Invoice.Items = new();
                            var addedItem = await IDM.CreateItemAsync(WorkItem);
                            if (addedItem is not null)
                                Invoice.Items.Insert(0, addedItem);
                        }
                        else MessageBox.Show("خطا هنگام اضافه کردن کالای جدید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else MessageBox.Show("نام کالای وارد شده موجود نیست", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            if (WorkItem != null && SelectedProductItem != null && SelectedProductItem.Id != 0)
            {
                WorkItem.InvoiceId = Invoice.Id;
                WorkItem.Product = await pcm.GetItemById(SelectedProductItem.Id);
                var validate = IDM.ValidateItem(WorkItem);
                if (validate.IsValid)
                {
                    if (EdittingItem == false) //New Item
                    {
                        if (Invoice.Items == null) Invoice.Items = new();
                        var addedItem = await IDM.CreateItemAsync(WorkItem);
                        if (addedItem is not null)
                            Invoice.Items.Insert(0, addedItem);
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
                    return;
                }
            }
        }
        ProductUnitModel temp = WorkItem.Unit;
        WorkItem = new();
        WorkItem.Unit = temp;
        SelectedProductItem = null;
        ProductInput = "";
        ReloadCustomerTotalBalance();
        CanEditInvoice = true;
        NotifyOfPropertyChange(() => Invoice.Items);
        NotifyOfPropertyChange(() => Invoice);
        FocusOnProductsCombobox();
    }

    public void FocusOnProductsCombobox()
    {
        ((GetView() as Window).FindName("ProductsCombobox") as ComboBox).Focus();
    }

    private async Task UpdateItemInDatabaseAsync(InvoiceItemModel item)
    {
        var ResultItem = await IDM.UpdateItemAsync(item);
        var EdittedItem = Invoice.Items.FirstOrDefault(x => x.Id == item.Id);
        if (ResultItem != null) ResultItem.Clone(EdittedItem);
        RefreshDataGrid();
    }

    public async Task DeleteItemAsync()
    {
        if (!CanDeleteInvoiceItem || Invoice == null || Invoice.Items == null || !Invoice.Items.Any() || SelectedItem == null) return;
        var result = MessageBox.Show("Are you sure you want to delete this row ?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        if (result == MessageBoxResult.No) return;
        if (await IDM.DeleteItemAsync(SelectedItem.Id))
        {
            if (SelectedItem.Id == WorkItem.Id)
            {
                ProductUnitModel temp = WorkItem.Unit;
                WorkItem = new();
                WorkItem.Unit = temp;
                SelectedProductItem = null;
                ProductInput = "";
            }
            Invoice.Items.Remove(SelectedItem);
        }
        RefreshDataGrid();
        ReloadCustomerTotalBalance();
        NotifyOfPropertyChange(() => Invoice);
    }

    public void dg_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Key.Delete == e.Key)
        {
            _ = DeleteItemAsync().ConfigureAwait(true);
            e.Handled = true;
        }
    }

    public async Task DeleteInvoiceAndCloseAsync()
    {
        if (!CanDeleteInvoice || Invoice == null) return;
        var result = MessageBox.Show("Are you sure ?", $"Delete Invoice for {Invoice.Customer.FullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        if (result == MessageBoxResult.No) return;
        if (await ICM.DeleteItemAsync(Invoice.Id) == false) MessageBox.Show($"Invoice with ID: {Invoice.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        CloseWindow();
    }

    private void RefreshDataGrid()
    {
        InvoiceModel temp;
        temp = Invoice;
        Invoice = null;
        Invoice = temp;
    }

    private void RefreshAndReloadCustomerTotalBalance()
    {
        _ = ReloadInvoiceAsync(Invoice.Id).ConfigureAwait(true);
    }

    private async Task RefreshAndReloadCustomerTotalBalanceAsync()
    {
        await ReloadInvoiceAsync(Invoice.Id);
    }

    public async Task ViewPaymentsAsync()
    {
        if (!CanEditInvoice) return;
        WindowManager wm = new();
        await wm.ShowWindowAsync(new InvoicePaymentsViewModel(ICM, IDM, User, Invoice, RefreshAndReloadCustomerTotalBalance, SC, true));
    }

    public async Task SaveInvoiceChangesAsync()
    {
        if (!CanEditInvoice) return;
        Invoice.DiscountType = (DiscountTypes)SelectedDiscountType;
        var result = await ICM.UpdateItemAsync(Invoice);
        if (result == null)
        {
            MessageBox.Show("خطا در ذخیره تغییرات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        Invoice.DateUpdated = result.DateUpdated;
        RefreshDataGrid();
        ReloadCustomerTotalBalance();
        CanSaveInvoiceChanges = false;
    }

    public void PrintInvoiceMenu(object sender, object window)
    {
        if (!CanPrintInvoice) return;
        ContextMenu cm = (window as Window).FindResource("PrintInvoiceCM") as ContextMenu;
        cm.PlacementTarget = sender as Button;
        cm.IsOpen = true;
    }

    public async Task PrintInvoiceAsync(int t)
    {
        if (!CanPrintInvoice || Invoice == null) return;
        await ReloadInvoiceAsync(Invoice.Id);
        PrintInvoiceModel pim = new();
        pim.PrintSettings = User.PrintSettings;
        Invoice.AsPrintModel(pim);
        /* 11: فاکتور فروش بدون سربرگ
         * 12: فاکتور فروش با سربرگ غیررسمی
         * 13: فاکتور فروش با سربرگ غیررسمی
         * 21: پیش فاکتور با سربرگ غیررسمی
         * 22: پیش فاکتور با سربرگ رسمی
         */
        if (t == 12) pim.PrintSettings.MainHeaderText = "فاکتور فروش";
        else if (t == 13) pim.PrintSettings.MainHeaderText = "فروشگاه آوازه";
        else if (t == 21) pim.PrintSettings.MainHeaderText = "پیش فاکتور";
        else if (t == 22) pim.PrintSettings.MainHeaderText = "فروشگاه آوازه";
        pim.InvoiceType = t;
        if (!string.IsNullOrEmpty(PhoneNumberText)) pim.CustomerPhoneNumber = PhoneNumberText;
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

    public async Task EditOwnerAsync()
    {
        if (!CanEditInvoice || Invoice is null) return;
        WindowManager wm = new();
        var ccm = SC.GetInstance<ICollectionManager<CustomerModel>>();
        await wm.ShowDialogAsync(new NewInvoiceViewModel(Singleton, Invoice.Id, ICM, ccm, RefreshAndReloadCustomerTotalBalanceAsync, User, SC));
    }

    public async Task EditCustomerAsync()
    {
        if (!CanEditInvoice || Invoice is null) return;
        WindowManager wm = new();
        var ccm = SC.GetInstance<ICollectionManager<CustomerModel>>();
        await wm.ShowWindowAsync(new CustomerDetailViewModel(ccm, Invoice.Customer, User, null));
    }

    public void CloseWindow()
    {
        (GetView() as Window).Close();
    }

    public async Task ClosingWindowAsync()
    {
        if (CallBackFunc != null)
            await CallBackFunc?.Invoke();
    }

    public void SellPrice_GotFocus()
    {
        if (RecentSellPrices != null && RecentSellPrices.Count > 1)
            IsSellPriceDropDownOpen = true;
    }

    public void SellPrice_LostFocus()
    {

    }

    public void ProductNames_PreviewTextInput()
    {
        IsProductInputDropDownOpen = true;
    }

    public async Task ProductNames_SelectionChangedAsync(object sender, EventArgs e)
    {
        if (Invoice == null || SelectedProductItem == null) return;
        if (CanUpdateRowFromDB is false) return;
        var pcm = SC.GetInstance<ICollectionManager<ProductModel>>();
        WorkItem.Product = await pcm.GetItemById(SelectedProductItem.Id);
        WorkItem.BuyPrice = WorkItem.Product.BuyPrice;
        WorkItem.SellPrice = WorkItem.Product.SellPrice;
        RecentSellPrices?.Clear();
        var recents = await IDM.GetRecentSellPrices(1, Invoice.Customer.Id, WorkItem.Product.Id);
        if (recents != null && recents.Count > 0) RecentSellPrices = recents;
        if (RecentSellPrices == null) RecentSellPrices = new();
        RecentSellPrices.Add(new RecentSellPriceModel { SellPrice = WorkItem.Product.SellPrice, DateSold = DateTime.Now });
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

    public async Task GetComboboxItemsAsync()
    {
        ProductItemsForComboBox = await Singleton.ReloadProductNames();
        ProductUnits = await Singleton.ReloadProductUnits();
    }

    public void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
    }

    public void CountStringTextbox_GotFocus(object sender)
    {
        (sender as TextBox).SelectAll();
    }

    public void CalculateTotalAmountOfWorkItem(object sender, TextChangedEventArgs e)
    {
        NotifyOfPropertyChange(() => WorkItem);
    }

    public void SetKeyboardLayout()
    {
        if (User.UserSettings.AutoSelectPersianLanguage)
            ExtensionsAndStatics.ChangeLanguageToPersian();
    }
}
public static class InvoiceDiscountTypeItems //For ComboBoxes
{
    public static Dictionary<int, string> GetDiscountTypeItems()
    {
        Dictionary<int, string> choices = new();
        for (int i = 0; i < Enum.GetNames(typeof(DiscountTypes)).Length; i++)
        {
            if (Enum.GetName(typeof(DiscountTypes), i) == DiscountTypes.Amount.ToString())
                choices.Add((int)DiscountTypes.Amount, "مبلغ");
            else if (Enum.GetName(typeof(DiscountTypes), i) == DiscountTypes.Percent.ToString())
                choices.Add((int)DiscountTypes.Percent, "درصد");
        }
        return choices;
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