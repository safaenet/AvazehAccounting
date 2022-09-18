﻿using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using SharedLibrary.SettingsModels;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class TransactionListViewModel : Screen
    {
        public TransactionListViewModel(ITransactionCollectionManager manager, SingletonClass singleton, IAppSettingsManager settingsManager, SimpleContainer sc)
        {
            TCM = manager;
            ASM = settingsManager;
            SC = sc;
            _SelectedTransaction = new();
            Singleton = singleton;
            LoadSettings().ConfigureAwait(true);
        }

        SimpleContainer SC;
        private ITransactionCollectionManager _TCM;
        private readonly IAppSettingsManager ASM;
        private TransactionSettingsModel transactionSettings;
        private PrintSettingsModel printSettings;
        private GeneralSettingsModel generalSettings;
        private TransactionListModel _SelectedTransaction;
        private SingletonClass Singleton;


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
        private async Task LoadSettings()
        {
            var Settings = await ASM.LoadAllAppSettings();
            if (Settings == null) Settings = new();
            TransactionSettings = Settings.TransactionSettings;
            GeneralSettings = Settings.GeneralSettings;

            TCM.PageSize = TransactionSettings.PageSize;
            TCM.QueryOrderType = TransactionSettings.QueryOrderType;
            await Search();
        }

        public TransactionSettingsModel TransactionSettings
        {
            get => transactionSettings; set
            {
                transactionSettings = value;
                NotifyOfPropertyChange(() => TransactionSettings);
            }
        }
        public PrintSettingsModel PrintSettings
        {
            get => printSettings; set
            {
                printSettings = value;
                NotifyOfPropertyChange(() => PrintSettings);
            }
        }

        public GeneralSettingsModel GeneralSettings
        {
            get => generalSettings; set
            {
                generalSettings = value;
                NotifyOfPropertyChange(() => GeneralSettings);
            }
        }

        public async Task AddNewTransaction()
        {
            if (!GeneralSettings.CanAddNewTransaction) return;
            WindowManager wm = new();
            await wm.ShowDialogAsync(new NewTransactionViewModel(Singleton, null, TCM, RefreshPage, ASM, SC));
        }

        public async Task PreviousPage()
        {
            if (!GeneralSettings.CanViewTransactions) return;
            await TCM.LoadPreviousPageAsync();
            NotifyOfPropertyChange(() => Transactions);
        }

        public async Task NextPage()
        {
            if (!GeneralSettings.CanViewTransactions) return;
            await TCM.LoadNextPageAsync();
            NotifyOfPropertyChange(() => Transactions);
        }

        public async Task RefreshPage()
        {
            if (!GeneralSettings.CanViewTransactions) return;
            await TCM.RefreshPage();
            NotifyOfPropertyChange(() => Transactions);
        }

        public async Task Search()
        {
            if (!GeneralSettings.CanViewTransactions) return;
            if (GeneralSettings != null && !GeneralSettings.CanViewTransactions) return;
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
            if (GeneralSettings == null || !GeneralSettings.CanViewTransactions) return;
            if (Transactions == null || Transactions.Count == 0 || SelectedTransaction == null || SelectedTransaction.Id == 0) return;
            var tdm = SC.GetInstance<ITransactionDetailManager>();
            WindowManager wm = new();
            await wm.ShowWindowAsync(new TransactionDetailViewModel(TCM, tdm, ASM, Singleton, SelectedTransaction.Id, RefreshPage));
        }

        public async Task DeleteTransaction()
        {
            if (!GeneralSettings.CanEditTransactions) return;
            if (Transactions == null || Transactions.Count == 0 || SelectedTransaction == null || SelectedTransaction.Id == 0) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete Transaction file {SelectedTransaction.FileName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = await TCM.DeleteItemAsync(SelectedTransaction.Id);
            if (output) Transactions.Remove(SelectedTransaction);
            else MessageBox.Show($"Transaction with ID: {SelectedTransaction.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public async Task RenameTransaction()
        {
            if (!GeneralSettings.CanEditTransactions) return;
            if (SelectedTransaction == null) return;
            WindowManager wm = new();
            await wm.ShowDialogAsync(new NewTransactionViewModel(Singleton, SelectedTransaction.Id, TCM, RefreshPage, ASM, SC));
        }

        public void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {
                DeleteTransaction().ConfigureAwait(true);
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