using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
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
    public class NewTransactionViewModel : ViewAware
    {
        public NewTransactionViewModel(int? TransactionId, ITransactionCollectionManager icManager, Func<Task> callBack)
        {
            TCM = icManager;
            TransactionID = TransactionId;
            CallBack = callBack;
            if (TransactionID == null)
            {
                ButtonTitle = "Add";
                WindowTitle = "فایل جدید";
            }
            else
            {
                ButtonTitle = "Update";
                LoadSelectedItem().ConfigureAwait(true);
            }
        }
        public ITransactionCollectionManager TCM { get; set; }
        private readonly int? TransactionID;
        private string transactionName;
        private string buttonTitle;
        private Func<Task> CallBack;
        private string windowTitle;

        public string WindowTitle
        {
            get { return windowTitle; }
            set { windowTitle = value; NotifyOfPropertyChange(() => WindowTitle); }
        }


        public string ButtonTitle
        {
            get { return buttonTitle; }
            set { buttonTitle = value; NotifyOfPropertyChange(() => ButtonTitle); }
        }

        public string TransactionInput
        {
            get => transactionName;
            set { transactionName = value; NotifyOfPropertyChange(() => TransactionInput); }
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        public async Task AddOrUpdateTransaction()
        {
            if (string.IsNullOrEmpty(TransactionInput))
            {
                MessageBox.Show("نام فایل را وارد کنید", TransactionInput, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (TransactionID is null) //Add new
            {
                TransactionModel t = new();
                t.FileName = TransactionInput;
                var newTransaction = await TCM.CreateItemAsync(t);
                if (newTransaction != null) CloseWindow();
                else MessageBox.Show("خطا هنگام ایجاد فایل جدید", TransactionInput, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else //Update Owner
            {
                int id = (int)TransactionID;
                var transaction = await TCM.GetItemById(id);
                if (transaction == null)
                {
                    MessageBox.Show("Cannot find such transaction.");
                    return;
                }
                if (MessageBox.Show("آیا نام فایل تغییر کند؟", "تغییر نام", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No) return;
                transaction.FileName = TransactionInput;
                await TCM.UpdateItemAsync(transaction);
                CloseWindow();
            }
        }

        private async Task LoadSelectedItem()
        {
            var transaction = await TCM.GetItemById((int)TransactionID);
            TransactionInput = transaction.FileName;
            WindowTitle = transaction.FileName + " - تغییر نام";
        }

        public void ClosingWindow()
        {
            CallBack?.Invoke();
        }

        public void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                (GetView() as Window).Close();
        }

        public async Task TransactionNameBoxKeyDownHandler(ActionExecutionContext context)
        {
            if (context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
            {
                await AddOrUpdateTransaction();
            }
        }
    }
}