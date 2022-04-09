using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class InvoicePaymentsViewModel : ViewAware
    {
        public InvoicePaymentsViewModel(IInvoiceCollectionManager iManager, IInvoiceDetailManager dManager, InvoiceModel invoice, System.Action callBack, bool ReloadInvoiceNeeded = false)
        {
            Invoice = invoice;
            InvoiceCollectionManager = iManager;
            InvoiceDetailManager = dManager;
            CallBackAction = callBack;
            if (ReloadInvoiceNeeded && Invoice != null)
                ReloadInvoice(Invoice.Id).ConfigureAwait(true);
            ReloadCustomerBalance().ConfigureAwait(true);
        }
        private InvoiceModel _invoice;
        public double CustomerTotalBalance { get => customerTotalBalance; private set { customerTotalBalance = value; NotifyOfPropertyChange(() => CustomerTotalBalance); } }
        private readonly IInvoiceCollectionManager InvoiceCollectionManager;
        private readonly IInvoiceDetailManager InvoiceDetailManager;
        private readonly System.Action CallBackAction;
        private bool EdittingItem = false;
        private InvoicePaymentModel _workItem = new();
        private InvoicePaymentModel selectedPaymentItem;
        private double customerTotalBalance;

        public InvoicePaymentModel SelectedPaymentItem
        {
            get => selectedPaymentItem;
            set { selectedPaymentItem = value; NotifyOfPropertyChange(() => SelectedPaymentItem); }
        }

        public InvoicePaymentModel WorkItem { get => _workItem; set { _workItem = value; NotifyOfPropertyChange(() => WorkItem); } }

        public InvoiceModel Invoice
        {
            get { return _invoice; }
            set { _invoice = value; NotifyOfPropertyChange(() => Invoice); }
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        private async Task ReloadInvoice(int InvoiceId)
        {
            Invoice = await InvoiceCollectionManager.GetItemById(InvoiceId);
        }

        private async Task ReloadCustomerBalance()
        {
            if (Invoice is null) return;
            CustomerTotalBalance = await InvoiceCollectionManager.GetCustomerTotalBalanceById(Invoice.Customer.Id);
        }

        public void ClosingWindow()
        {
            CallBackAction?.Invoke();
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
                    SelectedPaymentItem = new();
                }
            }
        }
    }
}