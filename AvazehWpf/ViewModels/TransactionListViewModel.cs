using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using SharedLibrary.SecurityAndSettingsModels;
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
        public TransactionListViewModel(ITransactionCollectionManager manager, SingletonClass singleton, LoggedInUser_DTO user, SimpleContainer sc)
        {
            TCM = manager;
            User = user;
            SC = sc;
            _SelectedTransaction = new();
            Singleton = singleton;
            _ = LoadSettingsAsync().ConfigureAwait(true);
        }

        SimpleContainer SC;
        private ITransactionCollectionManager _TCM;
        private readonly LoggedInUser_DTO User;
        private TransactionListModel _SelectedTransaction;
        private readonly SingletonClass Singleton;


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
        private async Task LoadSettingsAsync()
        {
            TCM.PageSize = User.UserSettings.TransactionListPageSize;
            TCM.QueryOrderType = User.UserSettings.TransactionListQueryOrderType;
            await SearchAsync();
        }

        public async Task AddNewTransactionAsync()
        {
            WindowManager wm = new();
            await wm.ShowDialogAsync(new NewTransactionViewModel(Singleton, null, TCM, RefreshPageAsync, User, SC));
        }

        public async Task PreviousPageAsync()
        {
            await TCM.LoadPreviousPageAsync();
            NotifyOfPropertyChange(() => Transactions);
        }

        public async Task NextPageAsync()
        {
            await TCM.LoadNextPageAsync();
            NotifyOfPropertyChange(() => Transactions);
        }

        public async Task RefreshPageAsync()
        {
            await TCM.RefreshPage();
            NotifyOfPropertyChange(() => Transactions);
        }

        public async Task SearchAsync()
        {
            TransactionFinancialStatus? FinStatus = SelectedFinStatus >= Enum.GetNames(typeof(TransactionFinancialStatus)).Length ? null : (TransactionFinancialStatus)SelectedFinStatus;
            TCM.SearchValue = SearchText;
            TCM.FinStatus = FinStatus;
            await TCM.LoadFirstPageAsync();
            NotifyOfPropertyChange(() => Transactions);
        }

        public async Task SearchBoxKeyDownHandlerAsync(ActionExecutionContext context)
        {
            if (context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
            {
                await SearchAsync();
            }
        }

        public async Task EditTransactionAsync()
        {
            if (Transactions == null || Transactions.Count == 0 || SelectedTransaction == null || SelectedTransaction.Id == 0) return;
            var tdm = SC.GetInstance<ITransactionDetailManager>();
            WindowManager wm = new();
            await wm.ShowWindowAsync(new TransactionDetailViewModel(TCM, tdm, User, Singleton, SelectedTransaction.Id, RefreshPageAsync));
        }

        public async Task DeleteTransactionAsync()
        {
            if (Transactions == null || Transactions.Count == 0 || SelectedTransaction == null || SelectedTransaction.Id == 0) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete Transaction file {SelectedTransaction.FileName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = await TCM.DeleteItemAsync(SelectedTransaction.Id);
            if (output) Transactions.Remove(SelectedTransaction);
            else MessageBox.Show($"Transaction with ID: {SelectedTransaction.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public async Task RenameTransactionAsync()
        {
            if (SelectedTransaction == null) return;
            WindowManager wm = new();
            await wm.ShowDialogAsync(new NewTransactionViewModel(Singleton, SelectedTransaction.Id, TCM, RefreshPageAsync, User, SC));
        }

        public void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {
                _ = DeleteTransactionAsync().ConfigureAwait(true);
                e.Handled = true;
            }
        }

        public void Window_PreviewKeyDown(object window, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) (GetView() as Window).Close();
        }
    }

    public static class TransactionFinStatusItems //For ComboBoxes
    {
        public static Dictionary<int, string> GetTransactionFinStatusItems()
        {
            Dictionary<int, string> choices = new();
            for (int i = 0; i < Enum.GetNames(typeof(TransactionFinancialStatus)).Length; i++)
            {
                if (Enum.GetName(typeof(TransactionFinancialStatus), i) == TransactionFinancialStatus.Balanced.ToString())
                    choices.Add((int)TransactionFinancialStatus.Balanced, "تسویه");
                else if (Enum.GetName(typeof(TransactionFinancialStatus), i) == TransactionFinancialStatus.Positive.ToString())
                    choices.Add((int)TransactionFinancialStatus.Positive, "مثبت");
                else if (Enum.GetName(typeof(TransactionFinancialStatus), i) == TransactionFinancialStatus.Negative.ToString())
                    choices.Add((int)TransactionFinancialStatus.Negative, "منفی");
            }
            choices.Add(Enum.GetNames(typeof(TransactionFinancialStatus)).Length, "همه");
            return choices;
        }
    }
}