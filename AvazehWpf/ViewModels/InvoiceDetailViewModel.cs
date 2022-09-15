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
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Diagnostics;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using SharedLibrary.SettingsModels;
using System.Globalization;

namespace AvazehWpf.ViewModels
{
    public class InvoiceDetailViewModel : ViewAware
    {
        public InvoiceDetailViewModel(IInvoiceCollectionManager iManager, IInvoiceDetailManager dManager, IAppSettingsManager settingsManager, SingletonClass singleton, int? InvoiceId, Func<Task> callBack, SimpleContainer sc)
        {
            ICM = iManager;
            IDM = dManager;
            ASM = settingsManager;
            SC = sc;
            CallBackFunc = callBack;
            Singleton = singleton;
            LoadSettings(InvoiceId).ConfigureAwait(true);
        }

        private readonly IInvoiceCollectionManager ICM;
        private readonly IInvoiceDetailManager IDM;
        private readonly IAppSettingsManager ASM;
        SimpleContainer SC;
        private readonly SingletonClass Singleton;
        private InvoiceModel _Invoice;
        private readonly Func<Task> CallBackFunc;
        private InvoiceSettingsModel invoiceSettings;
        private PrintSettingsModel printSettings;
        private GeneralSettingsModel generalSettings;
        private ObservableCollection<ItemsForComboBox> productItems;
        private ObservableCollection<ProductUnitModel> productUnits;
        private ObservableCollection<RecentSellPriceModel> recentSellPrices;
        public InvoiceSettingsModel InvoiceSettings { get => invoiceSettings; private set { invoiceSettings = value; NotifyOfPropertyChange(() => InvoiceSettings); } }
        public PrintSettingsModel PrintSettings { get => printSettings; private set { printSettings = value; NotifyOfPropertyChange(() => PrintSettings); } }
        public GeneralSettingsModel GeneralSettings { get => generalSettings; private set { generalSettings = value; NotifyOfPropertyChange(() => GeneralSettings); } }
        private InvoiceItemModel _workItem = new();
        private bool CanUpdateRowFromDB = true; //False when user DoubleClicks on a row.
        private bool EdittingItem = false;
        public bool CanSaveInvoiceChanges { get; set; } = true;
        public InvoiceItemModel SelectedItem { get; set; }
        public InvoiceItemModel WorkItem { get => _workItem; set { _workItem = value; NotifyOfPropertyChange(() => WorkItem); } }
        public double CustomerPreviousTotalBalance { get => customerPreviousTotalBalance; private set { customerPreviousTotalBalance = value; NotifyOfPropertyChange(() => CustomerPreviousTotalBalance); } }
        public double CustomerTotalBalancePlusThis { get => customerTotalBalancePlusThis; set { customerTotalBalancePlusThis = value; NotifyOfPropertyChange(() => CustomerTotalBalancePlusThis); } }
        public ObservableCollection<ItemsForComboBox> ProductItemsForComboBox { get => productItems; set { productItems = value; NotifyOfPropertyChange(() => ProductItemsForComboBox); } }
        public ObservableCollection<ProductUnitModel> ProductUnits { get => productUnits; set { productUnits = value; NotifyOfPropertyChange(() => ProductUnits); } }
        public ObservableCollection<RecentSellPriceModel> RecentSellPrices { get => recentSellPrices; set { recentSellPrices = value; NotifyOfPropertyChange(() => RecentSellPrices); } }
        private ItemsForComboBox _selectedProductItem;
        private bool isSellPriceDropDownOpen;
        private string productInput;
        private double customerPreviousTotalBalance;
        private double customerTotalBalancePlusThis;
        private bool isProductInputDropDownOpen;
        private string windowTitle;
        private string phoneNumberText;

        public string PhoneNumberText
        {
            get { return phoneNumberText; }
            set { phoneNumberText = value; }
        }

        private async Task LoadSettings(int? InvoiceId)
        {
            var Settings = await ASM.LoadAllAppSettings();
            if (Settings == null) Settings = new();
            InvoiceSettings = Settings.InvoiceSettings;
            PrintSettings = Settings.PrintSettings;
            GeneralSettings = Settings.GeneralSettings;

            if (InvoiceId is not null)
            {
                await ReloadInvoice(InvoiceId);
            }
            await GetComboboxItems();
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

        private async Task ReloadInvoice(int? InvoiceId)
        {
            if (!GeneralSettings.CanViewInvoices) return;
            if (InvoiceId is null || (int)InvoiceId == 0) return;
            Invoice = await ICM.GetItemById((int)InvoiceId);
            WindowTitle = Invoice.Customer.FullName + " - فاکتور";
            await ReloadCustomerPreviousBalance();
        }

        public async Task ReloadCustomerPreviousBalance()
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
            if(!GeneralSettings.CanEditInvoices) return;
            if (Invoice == null || SelectedItem == null) return;
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

        public async Task AddOrUpdateItem()
        {
            if (!GeneralSettings.CanEditInvoices) return;
            if (Invoice == null) return;
            var pcm = SC.GetInstance<ICollectionManager<ProductModel>>();
            if (SelectedProductItem == null && ProductInput != null && ProductInput.Length > 0 && EdittingItem == false) //Search by Entered text
            {
                if (InvoiceSettings.EnableBarcodeReader) //Search Barcode
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
                            WorkItem.CountString = InvoiceSettings.BarcodeAddItemCount.ToString();
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
                            WorkItem.CountString = (WorkItem.CountValue + InvoiceSettings.BarcodeAddItemCount).ToString();
                            await UpdateItemInDatabase(WorkItem);
                        }
                    }
                    else if (InvoiceSettings.AutoAddNewProducts) //Not found by barcode, Try to create new product.
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
                            await UpdateItemInDatabase(WorkItem);
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
            NotifyOfPropertyChange(() => Invoice.Items);
            NotifyOfPropertyChange(() => Invoice);
            FocusOnProductsCombobox();
        }

        public void FocusOnProductsCombobox()
        {
            ((GetView() as Window).FindName("ProductsCombobox") as ComboBox).Focus();
        }

        private async Task UpdateItemInDatabase(InvoiceItemModel item)
        {
            var ResultItem = await IDM.UpdateItemAsync(item);
            var EdittedItem = Invoice.Items.FirstOrDefault(x => x.Id == item.Id);
            if (ResultItem != null) ResultItem.Clone(EdittedItem);
            RefreshDataGrid();
        }

        public async Task DeleteItem()
        {
            if (!GeneralSettings.CanEditInvoices) return;
            if (Invoice == null || Invoice.Items == null || !Invoice.Items.Any() || SelectedItem == null) return;
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
                DeleteItem().ConfigureAwait(true);
                e.Handled = true;
            }
        }

        public async Task DeleteInvoiceAndClose()
        {
            if (!GeneralSettings.CanEditInvoices) CloseWindow();
            if (Invoice == null) return;
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
            ReloadInvoice(Invoice.Id).ConfigureAwait(true);
        }

        private async Task RefreshAndReloadCustomerTotalBalanceAsync()
        {
            await ReloadInvoice(Invoice.Id);
        }

        public async Task ViewPayments()
        {
            if (!GeneralSettings.CanEditInvoices) return;
            WindowManager wm = new();
            await wm.ShowWindowAsync(new InvoicePaymentsViewModel(ICM, IDM, ASM, Invoice, RefreshAndReloadCustomerTotalBalance, SC, true));
        }

        public async Task SaveInvoiceChanges()
        {
            if (!GeneralSettings.CanEditInvoices) return;
            var result = await ICM.UpdateItemAsync(Invoice);
            if (result == null)
            {
                MessageBox.Show("خطا در ذخیره تغییرات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Invoice.DateUpdated = result.DateUpdated;
            Invoice.TimeUpdated = result.TimeUpdated;
            RefreshDataGrid();
            ReloadCustomerTotalBalance();
            CanSaveInvoiceChanges = false;
        }

        public void PrintInvoiceMenu(object sender, object window)
        {
            ContextMenu cm = (window as Window).FindResource("PrintInvoiceCM") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }

        public async Task PrintInvoice(int t)
        {
            if (Invoice == null) return;
            await ReloadInvoice(Invoice.Id);
            PrintInvoiceModel pim = new();
            pim.PrintSettings = PrintSettings;
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
            var PrintInterfacePath = AppDomain.CurrentDomain.BaseDirectory + "PrintInterface.exe";
            var arguments = "invoice \"" + FilePath + "\"";
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo(PrintInterfacePath, arguments)
            };
            p.Start();
        }

        public async Task EditOwner()
        {
            if (!GeneralSettings.CanEditInvoices) return;
            if (Invoice is null) return;
            WindowManager wm = new();
            var ccm = SC.GetInstance<ICollectionManager<CustomerModel>>();
            await wm.ShowDialogAsync(new NewInvoiceViewModel(Singleton, Invoice.Id, ICM, ccm, RefreshAndReloadCustomerTotalBalanceAsync, ASM, SC));
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        public async Task ClosingWindow()
        {
            if (CallBackFunc != null)
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

        public void ProductNames_PreviewTextInput()
        {
            IsProductInputDropDownOpen = true;
        }

        public async Task ProductNames_SelectionChanged(object sender, EventArgs e)
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

        private async Task GetComboboxItems()
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
            if (GeneralSettings != null && GeneralSettings.AutoSelectPersianLanguage)
                if (GeneralSettings.AutoSelectPersianLanguage)
                    ExtensionsAndStatics.ChangeLanguageToPersian();
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