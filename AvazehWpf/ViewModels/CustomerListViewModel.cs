using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class CustomerListViewModel : Screen
    {
        public CustomerListViewModel(ICollectionManager<CustomerModel> manager)
        {
            CCM = manager;
            _SelectedCustomer = new();
            _ = SearchAsync().ConfigureAwait(true);
        }

        private ICollectionManager<CustomerModel> _CCM;
        private CustomerModel _SelectedCustomer;

        public CustomerModel SelectedCustomer
        {
            get { return _SelectedCustomer; }
            set { _SelectedCustomer = value; NotifyOfPropertyChange(() => SelectedCustomer); }
        }

        public ICollectionManager<CustomerModel> CCM
        {
            get { return _CCM; }
            set
            {
                _CCM = value;
                NotifyOfPropertyChange(() => CCM);
                NotifyOfPropertyChange(() => Customers);
            }
        }

        public ObservableCollection<CustomerModel> Customers
        {
            get => CCM.Items;
            set
            {
                CCM.Items = value;
                NotifyOfPropertyChange(() => CCM);
                NotifyOfPropertyChange(() => Customers);
            }
        }

        public string SearchText { get; set; }

        public async Task AddNewCustomerAsync()
        {
            WindowManager wm = new();
            await wm.ShowWindowAsync(new CustomerDetailViewModel(CCM, null, RefreshPageAsync));
        }

        public async Task PreviousPageAsync()
        {
            await CCM.LoadPreviousPageAsync();
            NotifyOfPropertyChange(() => Customers);
        }

        public async Task NextPageAsync()
        {
            await CCM.LoadNextPageAsync();
            NotifyOfPropertyChange(() => Customers);
        }

        public async Task RefreshPageAsync()
        {
            await CCM.RefreshPage();
            NotifyOfPropertyChange(() => Customers);
        }

        public async Task SearchAsync()
        {
            CCM.SearchValue = SearchText;
            await CCM.LoadFirstPageAsync();
            NotifyOfPropertyChange(() => Customers);
        }

        public async Task SearchBoxKeyDownHandlerAsync(ActionExecutionContext context)
        {
            if (context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
            {
                await SearchAsync();
            }
        }

        public async Task EditCustomerAsync()
        {
            if (Customers == null || Customers.Count == 0 || SelectedCustomer == null || SelectedCustomer.Id == 0) return;
            WindowManager wm = new();
            await wm.ShowWindowAsync(new CustomerDetailViewModel(CCM, SelectedCustomer, RefreshPageAsync));
        }

        public async Task DeleteCustomerAsync()
        {
            if (Customers == null || Customers.Count == 0 || SelectedCustomer == null || SelectedCustomer.Id == 0) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete {SelectedCustomer.FullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = await CCM.DeleteItemAsync(SelectedCustomer.Id);
            if (output) Customers.Remove(SelectedCustomer);
            else MessageBox.Show($"Customer with ID: {SelectedCustomer.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Window_PreviewKeyDown(object window, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) (GetView() as Window).Close();
        }

        public void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {
                _ = DeleteCustomerAsync().ConfigureAwait(true);
                e.Handled = true;
            }
        }
    }
}