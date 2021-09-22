using Caliburn.Micro;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class InvoiceListViewModel : Screen
    {
        private IInvoiceCollectionManager _ICM;
        public InvoiceListViewModel(IInvoiceCollectionManager manager)
        {
            _ICM = manager;
            _SelectedInvoice = new();
            ICM.GotoPage(1);
        }
        
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
            get { return ICM.Items; }
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
            wm.ShowDialogAsync(new InvoiceDetailViewModel(ICM, newInvoice));
            //if (newInvoice != null) Invoices.Add(newInvoice);
        }

        public void PreviousPage()
        {
            ICM.LoadPreviousPage();
            NotifyOfPropertyChange(() => Invoices);
        }

        public void NextPage()
        {
            ICM.LoadNextPage();
            NotifyOfPropertyChange(() => Invoices);
        }

        public void Search()
        {
            InvoiceFinancialStatus? FinStatus = SelectedFinStatus >= Enum.GetNames(typeof(InvoiceFinancialStatus)).Length ? null : (InvoiceFinancialStatus)SelectedFinStatus;
            InvoiceLifeStatus? LifeStatus = SelectedLifeStatus >= Enum.GetNames(typeof(InvoiceLifeStatus)).Length ? null : (InvoiceLifeStatus)SelectedLifeStatus;
            ICM.GenerateWhereClause(SearchText, LifeStatus, FinStatus);
            NotifyOfPropertyChange(() => Invoices);
        }

        public void SearchBoxKeyDownHandler(ActionExecutionContext context)
        {
            if (context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
            {
                Search();
            }
        }

        public void EditInvoice()
        {
            InvoiceModel invoice = ICM.Processor.LoadSingleItem(SelectedInvoice.Id);
            WindowManager wm = new();
            wm.ShowDialogAsync(new InvoiceDetailViewModel(ICM, invoice));
            NotifyOfPropertyChange(() => Invoices);
            NotifyOfPropertyChange(() => SelectedInvoice);
        }

        public void DeleteInvoice()
        {
            if (SelectedInvoice == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete invoice of {SelectedInvoice.CustomerFullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = ICM.Processor.DeleteItemById(SelectedInvoice.Id);
            if (output > 0) Invoices.Remove(SelectedInvoice);
            else MessageBox.Show($"Invoice with ID: {SelectedInvoice.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void PrintList()
        {

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