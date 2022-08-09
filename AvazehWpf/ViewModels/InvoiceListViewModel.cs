using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace AvazehWpf.ViewModels
{
    public class InvoiceListViewModel : Screen
    {
        public InvoiceListViewModel(IInvoiceCollectionManager manager, InvoiceDetailSingleton singleton)
        {
            ICM = manager;
            _SelectedInvoice = new();
            Singleton = singleton;
            Search().ConfigureAwait(true);
        }

        private IInvoiceCollectionManager _ICM;
        private InvoiceListModel _SelectedInvoice;
        private InvoiceDetailSingleton Singleton;

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

        public async Task AddNewInvoice()
        {
            WindowManager wm = new();
            await wm.ShowWindowAsync(new NewInvoiceViewModel(Singleton));
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

    public class AmountToBrushConverterForForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            double input = (double)value;
           return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}