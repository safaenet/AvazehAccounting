using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class InvoiceListViewModel : Screen
    {
        public InvoiceListViewModel(IInvoiceCollectionManager manager)
        {
            ICM = manager;
            _SelectedInvoice = new();
            Search().ConfigureAwait(true);
        }

        private IInvoiceCollectionManager _ICM;
        private InvoiceListModel _SelectedInvoice;

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

        public void AddNewInvoice()
        {
            InvoiceModel newInvoice = new();
            WindowManager wm = new();
            //wm.ShowWindowAsync(new InvoiceDetailViewModel(ICM, newInvoice));
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
            var keyArgs = context.EventArgs as KeyEventArgs;
            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                await Search();
            }
        }

        public async Task EditInvoice()
        {
            if (Invoices == null || Invoices.Count == 0) return;
            var invoiceDto = await ICM.GetItemById(SelectedInvoice.Id);
            WindowManager wm = new();
            //wm.ShowDialogAsync(new InvoiceDetailViewModel(ICM, invoiceDto));
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
    public static class FinStatusAndLifeStatusItems //For ComboBoxes
    {
        public static Dictionary<int, string> GetFinStatusItems()
        {
            Dictionary<int, string> choices = new();
            for (int i = 0; i < Enum.GetNames(typeof(InvoiceFinancialStatus)).Length; i++)
            {
                choices.Add(i, Enum.GetName(typeof(InvoiceFinancialStatus), i));
            }
            choices.Add(Enum.GetNames(typeof(InvoiceFinancialStatus)).Length, "All");
            return choices;
        }
        public static Dictionary<int, string> GetLifeStatusItems()
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
}