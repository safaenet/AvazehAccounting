using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.SecurityAndSettingsModels;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class InvoicePaymentsViewModel : ViewAware
    {
        public InvoicePaymentsViewModel(IInvoiceCollectionManager iManager, IInvoiceDetailManager dManager, LoggedInUser_DTO user, InvoiceModel invoice, System.Action callBack, SimpleContainer sc, bool ReloadInvoiceNeeded = false)
        {
            Invoice = invoice;
            InvoiceCollectionManager = iManager;
            InvoiceDetailManager = dManager;
            User = user;
            SC = sc;
            CallBackAction = callBack;
            if (ReloadInvoiceNeeded && Invoice != null)  _ = ReloadInvoiceAsync(Invoice.Id).ConfigureAwait(true);
            _ = ReloadCustomerBalanceAsync().ConfigureAwait(true);
        }
        private InvoiceModel _invoice;
        public double CustomerTotalBalance { get => customerTotalBalance; private set { customerTotalBalance = value; NotifyOfPropertyChange(() => CustomerTotalBalance); } }
        private readonly IInvoiceCollectionManager InvoiceCollectionManager;
        private readonly IInvoiceDetailManager InvoiceDetailManager;
        private readonly LoggedInUser_DTO User;
        readonly SimpleContainer SC;
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

        public void EditItem() //DataGrid doubleClick event
        {
            if (Invoice == null || SelectedPaymentItem == null) return;
            EdittingItem = true;
            SelectedPaymentItem.Clone(WorkItem);
            NotifyOfPropertyChange(() => WorkItem);
        }

        public async Task AddOrUpdateItemAsync()
        {
            if (Invoice == null) return;
            if (WorkItem == null) return;
            WorkItem.InvoiceId = Invoice.Id;
            var validate = InvoiceDetailManager.ValidateItem(WorkItem);
            if (!validate.IsValid)
            {
                var str = "";
                foreach (var error in validate.Errors)
                    str += error.ErrorMessage + "\n";
                MessageBox.Show(str, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (EdittingItem == false) //New Item
            {
                if (Invoice.Payments == null) Invoice.Payments = new();
                var addedItem = await InvoiceDetailManager.CreatePaymentAsync(WorkItem);
                if (addedItem is not null)
                    Invoice.Payments.Add(addedItem);
            }
            else //Edit Item
            {
                await UpdateItemInDatabaseAsync(WorkItem);
                EdittingItem = false;
            }
            WorkItem = new();
            SelectedPaymentItem = null;
            NotifyOfPropertyChange(() => Invoice.Payments);
            NotifyOfPropertyChange(() => Invoice);
        }

        private async Task UpdateItemInDatabaseAsync(InvoicePaymentModel item)
        {
            var ResultItem = await InvoiceDetailManager.UpdatePaymentAsync(item);
            var EdittedItem = Invoice.Payments.FirstOrDefault(x => x.Id == item.Id);
            if (ResultItem != null) ResultItem.Clone(EdittedItem);
            RefreshDataGrid();
        }

        public async Task DeleteItemAsync()
        {
            if (Invoice == null || Invoice.Payments == null || !Invoice.Payments.Any() || SelectedPaymentItem == null) return;
            var result = MessageBox.Show("Are you sure you want to delete this row ?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (await InvoiceDetailManager.DeletePaymentAsync(SelectedPaymentItem.Id))
                Invoice.Payments.Remove(SelectedPaymentItem);
            RefreshDataGrid();
            //ReloadCustomerTotalBalance();
            NotifyOfPropertyChange(() => Invoice);
        }

        public void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {
                _ = DeleteItemAsync().ConfigureAwait(true);
                e.Handled = true;
            }
        }

        private void RefreshDataGrid()
        {
            InvoiceModel temp;
            temp = Invoice;
            Invoice = null;
            Invoice = temp;
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        private async Task ReloadInvoiceAsync(int InvoiceId)
        {
            Invoice = await InvoiceCollectionManager.GetItemById(InvoiceId);
        }

        public async Task ReloadCustomerBalanceAsync()
        {
            if (Invoice is null) return;
            CustomerTotalBalance = await InvoiceCollectionManager.GetCustomerTotalBalanceById(Invoice.Customer.Id);
        }

        public void ReloadInvoiceBalance()
        {
            if (Invoice is null) return;
            _ = ReloadInvoiceAsync(Invoice.Id).ConfigureAwait(true);
        }

        public void ClosingWindow()
        {
            CallBackAction?.Invoke();
        }

        public void PutCustomerTotalBalanceInPayment()
        {
            if (WorkItem is null) WorkItem = new();
            WorkItem.PayAmount = CustomerTotalBalance;
            WorkItem.Descriptions = "تسویه کل بدهی";
            NotifyOfPropertyChange(() => WorkItem);
        }

        public void PutInvoiceTotalBalanceInPayment()
        {
            if (WorkItem is null) WorkItem = new();
            WorkItem.PayAmount = Invoice == null ? 0 : Invoice.TotalBalance;
            WorkItem.Descriptions = "تسویه فاکتور";
            NotifyOfPropertyChange(() => WorkItem);
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

        public void SetKeyboardLayout()
        {
            if (User.Settings.AutoSelectPersianLanguage)
                ExtensionsAndStatics.ChangeLanguageToPersian();
        }
    }
}