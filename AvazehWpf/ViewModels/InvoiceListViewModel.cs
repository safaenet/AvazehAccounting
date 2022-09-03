using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.SettingsModels;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using SharedLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace AvazehWpf.ViewModels
{
    public class InvoiceListViewModel : Screen
    {
        public InvoiceListViewModel(IInvoiceCollectionManager manager, SingletonClass singleton, IAppSettingsManager settingsManager)
        {
            ICM = manager;
            ASM = settingsManager;
            _SelectedInvoice = new();
            Singleton = singleton;
            LoadSettings().ConfigureAwait(true);
            Search().ConfigureAwait(true);
        }

        private IInvoiceCollectionManager _ICM;
        private IAppSettingsManager ASM;
        private InvoiceListModel _SelectedInvoice;
        private SingletonClass Singleton;
        public InvoiceSettingsModel InvoiceSettings { get; private set; }
        public InvoicePrintSettingsModel PrintSettings { get; private set; }
        public GeneralSettingsModel GeneralSettings { get; private set; }

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

        public string SearchText { get; set; }
        public int SelectedFinStatus { get; set; } = 1;
        public int SelectedLifeStatus { get; set; }

        private async Task LoadSettings()
        {
            var Settings = await ASM.LoadAllAppSettings();
            InvoiceSettings = Settings.InvoiceSettings;
            PrintSettings = Settings.InvoicePrintSettings;
            GeneralSettings = Settings.GeneralSettings;
        }

        public async Task AddNewInvoice()
        {
            WindowManager wm = new();
            ICollectionManager<CustomerModel> cManager = new CustomerCollectionManagerAsync<CustomerModel, CustomerModel_DTO_Create_Update, CustomerValidator>(ICM.ApiProcessor);
            await wm.ShowDialogAsync(new NewInvoiceViewModel(Singleton, null, ICM, cManager, Search));
        }

        public async Task PreviousPage()
        {
            await ICM.LoadPreviousPageAsync();
            NotifyOfPropertyChange(() => Invoices);
        }

        public async Task NextPage()
        {
            await ICM.LoadNextPageAsync();
            NotifyOfPropertyChange(() => Invoices);
        }

        public async Task RefreshPage()
        {
            await ICM.RefreshPage();
            NotifyOfPropertyChange(() => Invoices);
        }

        public async Task Search()
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
            Task.Run(Search);
        }

        public async Task SearchBoxKeyDownHandler(ActionExecutionContext context)
        {
            if (context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
            {
                await Search();
            }
        }

        public async Task EditInvoice()
        {
            if (Invoices == null || Invoices.Count == 0 || SelectedInvoice == null) return;
            WindowManager wm = new();
            await wm.ShowWindowAsync(new InvoiceDetailViewModel(ICM, new InvoiceDetailManager(ICM.ApiProcessor), Singleton, SelectedInvoice.Id, RefreshPage));
        }

        public async Task DeleteInvoice()
        {
            if (Invoices == null || Invoices.Count == 0 || SelectedInvoice == null || SelectedInvoice.Id == 0) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete invoice of {SelectedInvoice.CustomerFullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = await ICM.DeleteItemAsync(SelectedInvoice.Id);
            if (output) Invoices.Remove(SelectedInvoice);
            else MessageBox.Show($"Invoice with ID: {SelectedInvoice.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public async Task ViewPayments()
        {
            if (SelectedInvoice is null) return;
            WindowManager wm = new();
            var invoice = await ICM.GetItemById(SelectedInvoice.Id);
            await wm.ShowWindowAsync(new InvoicePaymentsViewModel(ICM, new InvoiceDetailManager(ICM.ApiProcessor), invoice, SearchSync, true));
        }

        public async Task ShowCustomerInvoices()
        {
            if (SelectedInvoice is null) return;
            SearchText = SelectedInvoice.CustomerFullName;
            await Search();
        }

        public async Task ShowAllInvoices()
        {
            SearchText = "";
            await Search();
        }

        public void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {
                DeleteInvoice().ConfigureAwait(true);
                e.Handled = true;
            }
        }

        public async Task PrintInvoice(int t)
        {
            if (Invoices == null || Invoices.Count == 0 || SelectedInvoice == null || SelectedInvoice.Id == 0) return;
            var Invoice = await ICM.GetItemById(SelectedInvoice.Id);
            if (Invoice == null) return;
            PrintInvoiceModel pim = new();
            Invoice.AsPrintModel(pim);
            if (t == 12)
            {
                pim.MainHeaderText = "فاکتور فروش";
            }
            else if (t == 13)
            {
                pim.MainHeaderText = "فروشگاه آوازه";
            }
            else if (t == 21)
            {
                pim.MainHeaderText = "پیش فاکتور";
            }
            else if (t == 22)
            {
                pim.MainHeaderText = "فروشگاه آوازه";
            }
            pim.InvoiceType = t;
            pim.HeaderDescription1 = "دوربین مداربسته، کرکره برقی، جک پارکینگی";
            pim.HeaderDescription2 = "01734430827";
            pim.FooterTextLeft = "Some Text Here";
            pim.FooterTextRight = "توسعه دهنده نرم افزار: صفا دانا";
            pim.LeftImagePath = AppDomain.CurrentDomain.BaseDirectory + @"Images\LeftImage.png";
            pim.RightImagePath = AppDomain.CurrentDomain.BaseDirectory + @"Images\RightImage.png";
            pim.MainHeaderTextFontSize = 30;
            pim.HeaderDescriptionFontSize = 10;
            pim.InvoiceTypeTextFontSize = 16;
            pim.UserDescriptions = await ICM.GetUserDescriptions();

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
    }

    public static class InvoiceFinStatusAndLifeStatusItems //For ComboBoxes
    {
        public static Dictionary<int, string> GetInvoiceFinStatusItems()
        {
            Dictionary<int, string> choices = new();
            for (int i = 0; i < Enum.GetNames(typeof(InvoiceFinancialStatus)).Length; i++)
            {
                choices.Add(i, Enum.GetName(typeof(InvoiceFinancialStatus), i));
            }
            choices.Add(Enum.GetNames(typeof(InvoiceFinancialStatus)).Length, "All");
            return choices;
        }

        public static Dictionary<int, string> GetInvoiceLifeStatusItems()
        {
            Dictionary<int, string> choices = new();
            for (int i = 0; i < Enum.GetNames(typeof(InvoiceLifeStatus)).Length; i++)
            {
                choices.Add(i, Enum.GetName(typeof(InvoiceLifeStatus), i));
            }
            choices.Add(Enum.GetNames(typeof(InvoiceLifeStatus)).Length, "All");
            return choices;
        }
    }
    
    public class NameToBrushConverterForBackground : IValueConverter
    {
        private readonly string CurrentPersianDate =new PersianCalendar().GetPersianDate();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null) return DependencyProperty.UnsetValue;
            string input = value.ToString();
            if(input == CurrentPersianDate) return Brushes.LightYellow;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class NameToBrushConverterForForeground : IValueConverter
    {
        private readonly string CurrentPersianDate = new PersianCalendar().GetPersianDate();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            string input = value.ToString();
            if (input == CurrentPersianDate) return Brushes.Black;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class AmountToBrushConverterForBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            double input = (double)value;
            return input switch
            {
                < 0 => Brushes.LightBlue,
                0 => Brushes.LightGreen,
                > 0 => Brushes.LightPink,
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class AmountToBrushConverterForBackgroundInverted : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            double input = (double)value;
            return input switch
            {
                < 0 => Brushes.LightPink,
                0 => Brushes.LightGreen,
                > 0 => Brushes.LightBlue,
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class AmountToBrushConverterForForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            //double input = (double)value;
           return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            Color input = ((string)value).ToColor();
            return input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            string input = ((Color)value).ToHex();
            return input;
        }
    }

    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            SolidColorBrush brush = new SolidColorBrush(((string)value).ToColor());
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class PersianOrderStringToEnglishOrderStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            if ((OrderType)value == OrderType.ASC) return "صعودی";
            return "نزولی";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            if ((string)value == "صعودی") return OrderType.ASC;
            return OrderType.DESC;
        }
    }
}