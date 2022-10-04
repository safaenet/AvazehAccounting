using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.SecurityAndSettingsModels;
using SharedLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace AvazehWpf.ViewModels
{
    public class InvoiceListViewModel : Screen
    {
        public InvoiceListViewModel(IInvoiceCollectionManager manager, SingletonClass singleton, LoggedInUser_DTO user, SimpleContainer sc)
        {
            ICM = manager;
            User = user;
            CurrentPersianDate = new PersianCalendar().GetPersianDate();
            SC = sc;
            _SelectedInvoice = new();
            Singleton = singleton;

            ICM.PageSize = User.UserSettings.InvoicePageSize;
            ICM.QueryOrderType = User.UserSettings.InvoiceListQueryOrderType;

            _ = SearchAsync().ConfigureAwait(true);
        }

        readonly SimpleContainer SC;
        private IInvoiceCollectionManager _ICM;
        public LoggedInUser_DTO User { get => user; init => user = value; }
        public string CurrentPersianDate { get; init; }
        private InvoiceListModel _SelectedInvoice;
        private string searchText;
        private LoggedInUser_DTO user;
        private readonly SingletonClass Singleton;

        public InvoiceListModel SelectedInvoice
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
            await ICM.LoadFirstPageAsync();
            NotifyOfPropertyChange(() => Invoices);
        }

        public void SearchSync()
        {
            _ = Task.Run(SearchAsync);
        }

        public async Task SearchBoxKeyDownHandlerAsync(ActionExecutionContext context)
        {
            if (context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
            {
                await SearchAsync();
            }
        }

        public async Task EditInvoiceAsync()
        {
            if (Invoices == null || Invoices.Count == 0 || SelectedInvoice == null || SelectedInvoice.Id == 0) return;
            var idm = SC.GetInstance<IInvoiceDetailManager>();
            WindowManager wm = new();
            await wm.ShowWindowAsync(new InvoiceDetailViewModel(ICM, idm, User, Singleton, SelectedInvoice.Id, RefreshPageAsync, SC));
        }

        public async Task DeleteInvoiceAsync()
        {
            if (Invoices == null || Invoices.Count == 0 || SelectedInvoice == null || SelectedInvoice.Id == 0) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete invoice of {SelectedInvoice.CustomerFullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = await ICM.DeleteItemAsync(SelectedInvoice.Id);
            if (output) Invoices.Remove(SelectedInvoice);
            else MessageBox.Show($"Invoice with ID: {SelectedInvoice.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public async Task ViewPaymentsAsync()
        {
            if (SelectedInvoice is null) return;
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
            if (Invoices == null || Invoices.Count == 0 || SelectedInvoice == null || SelectedInvoice.Id == 0) return;
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

        public void SetKeyboardLayout()
        {
            if (User.UserSettings.AutoSelectPersianLanguage)
                ExtensionsAndStatics.ChangeLanguageToPersian();
        }

        public void Window_PreviewKeyDown(object window, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) (GetView() as Window).Close();
        }

        //public static Dictionary<string, string> GetColorSettings()
        //{
        //    Dictionary<string, string> colors = new();
        //    colors.Add(nameof(UserSettingsModel.ColorNewItem), User.UserSettings.ColorNewItem);
        //    colors.Add(nameof(UserSettingsModel.ColorSoldItem), User.UserSettings.ColorSoldItem);
        //    colors.Add(nameof(UserSettingsModel.ColorNonSufficientFundItem), User.UserSettings.ColorNonSufficientFundItem);
        //    colors.Add(nameof(UserSettingsModel.ColorCashedItem), User.UserSettings.ColorCashedItem);
        //    colors.Add(nameof(UserSettingsModel.ColorChequeNotification), User.UserSettings.ColorChequeNotification);
        //    colors.Add(nameof(UserSettingsModel.ColorUpdatedItem), User.UserSettings.ColorUpdatedItem);
        //    colors.Add(nameof(UserSettingsModel.ColorBalancedItem), User.UserSettings.ColorBalancedItem);
        //    colors.Add(nameof(UserSettingsModel.ColorDeptorItem), User.UserSettings.ColorDeptorItem);
        //    colors.Add(nameof(UserSettingsModel.ColorCreditorItem), User.UserSettings.ColorCreditorItem);
        //    colors.Add(nameof(UserSettingsModel.ColorInactiveItem), User.UserSettings.ColorInactiveItem);
        //    colors.Add(nameof(UserSettingsModel.ColorArchivedItem), User.UserSettings.ColorArchivedItem);
        //    colors.Add(nameof(UserSettingsModel.ColorDeletedItem), User.UserSettings.ColorDeletedItem);
        //    colors.Add(nameof(UserSettingsModel.ColorNegativeProfit), User.UserSettings.ColorNegativeProfit);
        //    colors.Add(nameof(UserSettingsModel.ColorPositiveItem), User.UserSettings.ColorPositiveItem);
        //    colors.Add(nameof(UserSettingsModel.ColorNegativeItem), User.UserSettings.ColorNegativeItem);
        //    var now = DateTime.Now;
        //    PersianCalendar pCal = new();
        //    var date = string.Format("{0:0000}/{1:00}/{2:00}", pCal.GetYear(now), pCal.GetMonth(now), pCal.GetDayOfMonth(now));
        //    colors.Add("CurrentPersianDate", date);
        //    return colors;
        //}
    }

    public static class InvoiceFinStatusAndLifeStatusItems //For ComboBoxes
    {
        public static Dictionary<int, string> GetInvoiceFinStatusItems()
        {
            Dictionary<int, string> choices = new();
            for (int i = 0; i < Enum.GetNames(typeof(InvoiceFinancialStatus)).Length; i++)
            {
                if (Enum.GetName(typeof(InvoiceFinancialStatus), i) == InvoiceFinancialStatus.Balanced.ToString())
                    choices.Add((int)InvoiceFinancialStatus.Balanced, "تسویه");
                else if (Enum.GetName(typeof(InvoiceFinancialStatus), i) == InvoiceFinancialStatus.Deptor.ToString())
                    choices.Add((int)InvoiceFinancialStatus.Deptor, "بدهکار");
                else if (Enum.GetName(typeof(InvoiceFinancialStatus), i) == InvoiceFinancialStatus.Creditor.ToString())
                    choices.Add((int)InvoiceFinancialStatus.Creditor, "بستانکار");
            }
            choices.Add(Enum.GetNames(typeof(InvoiceFinancialStatus)).Length, "همه");
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
}