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
            Search().ConfigureAwait(true);
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

        public void AddNewCustomer()
        {
            WindowManager wm = new();
            wm.ShowWindowAsync(new CustomerDetailViewModel(CCM, null, RefreshPage));
        }

        public async Task PreviousPage()
        {
            await CCM.LoadPreviousPageAsync();
            NotifyOfPropertyChange(() => Customers);
        }

        public async Task NextPage()
        {
            await CCM.LoadNextPageAsync();
            NotifyOfPropertyChange(() => Customers);
        }

        public async Task RefreshPage()
        {
            await CCM.RefreshPage();
            NotifyOfPropertyChange(() => Customers);
        }

        public async Task Search()
        {
            CCM.SearchValue = SearchText;
            await CCM.LoadFirstPageAsync();
            NotifyOfPropertyChange(() => Customers);
        }

        public async Task SearchBoxKeyDownHandler(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;
            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                await Search();
            }
        }

        public async Task EditCustomer()
        {
            if (Customers == null || Customers.Count == 0) return;
            WindowManager wm = new();
            await wm.ShowWindowAsync(new CustomerDetailViewModel(CCM, SelectedCustomer, RefreshPage));
        }

        public async Task DeleteCustomer()
        {
            if (Customers == null || Customers.Count == 0 || SelectedCustomer == null || SelectedCustomer.Id == 0) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete {SelectedCustomer.FullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = await CCM.DeleteItemAsync(SelectedCustomer.Id);
            if (output) Customers.Remove(SelectedCustomer);
            else MessageBox.Show($"Customer with ID: {SelectedCustomer.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}