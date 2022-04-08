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
        public InvoicePaymentsViewModel(IInvoiceCollectionManager iManager, IInvoiceDetailManager dManager, InvoiceModel invoice, System.Action callBack)
        {
            Invoice = invoice;
            InvoiceCollectionManager = iManager;
            Manager = dManager;
            CallBackAction = callBack;
        }
        private InvoiceModel _invoice;
        private readonly IInvoiceCollectionManager InvoiceCollectionManager;
        private readonly IInvoiceDetailManager Manager;
        private readonly System.Action CallBackAction;
        private bool EdittingItem = false;
        private InvoicePaymentModel _workItem = new();
        private InvoicePaymentModel selectedPaymentItem;

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