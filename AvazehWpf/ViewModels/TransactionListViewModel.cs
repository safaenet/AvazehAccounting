using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
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
    public class TransactionListViewModel : Screen
    {
        public TransactionListViewModel(ITransactionCollectionManager manager, InvoiceDetailSingleton singleton)
        {
            TCM = manager;
            _SelectedTransaction = new();
            Singleton = singleton;
            Search().ConfigureAwait(true);
        }

        private ITransactionCollectionManager _TCM;
        private TransactionListModel _SelectedTransaction;
        private InvoiceDetailSingleton Singleton;

        public TransactionListModel SelectedTransaction
        {
            get { return _SelectedTransaction; }
            set { _SelectedTransaction = value; NotifyOfPropertyChange(() => SelectedTransaction); }
        }

        public ITransactionCollectionManager TCM
        {
            get { return _TCM; }
            set
            {
                _TCM = value;
                NotifyOfPropertyChange(() => TCM);
                NotifyOfPropertyChange(() => Transactions);
            }
        }

        public ObservableCollection<TransactionListModel> Transactions
        {
            get => TCM.Items;
            set
            {
                TCM.Items = value;
                NotifyOfPropertyChange(() => TCM);
                NotifyOfPropertyChange(() => Transactions);
            }
        }

        public string SearchText { get; set; }
        public int SelectedFinStatus { get; set; } = 3;

        public async Task AddNewTransaction()
        {
            WindowManager wm = new();
            await wm.ShowWindowAsync(new TransactionDetailViewModel(TCM, new TransactionDetailManager(TCM.ApiProcessor), Singleton, null, RefreshPage));
        }

        public async Task PreviousPage()
        {
            await TCM.LoadPreviousPageAsync();
            NotifyOfPropertyChange(() => Transactions);
        }

        public async Task NextPage()
        {
            await TCM.LoadNextPageAsync();
            NotifyOfPropertyChange(() => Transactions);
        }

        public async Task RefreshPage()
        {
            await TCM.RefreshPage();
            NotifyOfPropertyChange(() => Transactions);
        }

        public async Task Search()
        {
            TransactionFinancialStatus? FinStatus = SelectedFinStatus >= Enum.GetNames(typeof(TransactionFinancialStatus)).Length ? null : (TransactionFinancialStatus)SelectedFinStatus;
            TCM.SearchValue = SearchText;
            TCM.FinStatus = FinStatus;
            await TCM.LoadFirstPageAsync();
            NotifyOfPropertyChange(() => Transactions);
        }

        public async Task SearchBoxKeyDownHandler(ActionExecutionContext context)
        {
            if (context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
            {
                await Search();
            }
        }

        public async Task EditTransaction()
        {
            if (Transactions == null || Transactions.Count == 0 || SelectedTransaction == null) return;
            WindowManager wm = new();
            await wm.ShowWindowAsync(new TransactionDetailViewModel(TCM, new TransactionDetailManager(TCM.ApiProcessor), Singleton, SelectedTransaction.Id, RefreshPage));
        }

        public async Task DeleteTransaction()
        {
            if (Transactions == null || Transactions.Count == 0 || SelectedTransaction == null || SelectedTransaction.Id == 0) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete Transaction file {SelectedTransaction.FileName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = await TCM.DeleteItemAsync(SelectedTransaction.Id);
            if (output) Transactions.Remove(SelectedTransaction);
            else MessageBox.Show($"Transaction with ID: {SelectedTransaction.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static class TransactionFinStatusItems //For ComboBoxes
    {
        public static Dictionary<int, string> GetTransactionFinStatusItems()
        {
            Dictionary<int, string> choices = new();
            for (int i = 0; i < Enum.GetNames(typeof(TransactionFinancialStatus)).Length; i++)
            {
                choices.Add(i, Enum.GetName(typeof(TransactionFinancialStatus), i));
            }
            choices.Add(Enum.GetNames(typeof(TransactionFinancialStatus)).Length, "All");
            return choices;
        }
    }
}