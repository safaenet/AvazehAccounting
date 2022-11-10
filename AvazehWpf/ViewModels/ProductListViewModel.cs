using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.SecurityAndSettingsModels;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class ProductListViewModel : Screen
    {
        public ProductListViewModel(ICollectionManager<ProductModel> manager, LoggedInUser_DTO user)
        {
            PCM = manager;
            User = user;
            LoadSettings();
            CurrentPersianDate = new PersianCalendar().GetPersianDate();
            _SelectedProduct = new();
            _ = SearchAsync().ConfigureAwait(true);
        }

        private void LoadSettings()
        {
            CanAddNewProduct = PCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanAddNewCustomer));
            CanViewProductDetails = PCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanViewCustomerDetails));
            CanDeleteProduct = PCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanDeleteCustomer));
            CanViewNetProfits = PCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanViewNetProfits));

            PCM.PageSize = User.UserSettings.ProductListPageSize;
            PCM.QueryOrderType = User.UserSettings.ProductListQueryOrderType;
        }

        private ICollectionManager<ProductModel> _PCM;
        private ProductModel _SelectedProduct;

        public LoggedInUser_DTO User { get; init; }
        public string CurrentPersianDate { get; init; }
        public ProductModel SelectedProduct
        {
            get => _SelectedProduct;
            set { _SelectedProduct = value; NotifyOfPropertyChange(() => SelectedProduct); }
        }

        public ICollectionManager<ProductModel> PCM
        {
            get => _PCM;
            set
            {
                _PCM = value;
                NotifyOfPropertyChange(() => PCM);
                NotifyOfPropertyChange(() => Products);
            }
        }

        public ObservableCollection<ProductModel> Products
        {
            get => PCM.Items;
            set
            {
                PCM.Items = value;
                NotifyOfPropertyChange(() => PCM);
                NotifyOfPropertyChange(() => Products);
            }
        }

        public string SearchText { get; set; }

        private bool canAddNewProduct;
        public bool CanAddNewProduct
        {
            get { return canAddNewProduct; }
            set { canAddNewProduct = value; NotifyOfPropertyChange(() => CanAddNewProduct); }
        }

        private bool canViewProductDetails;
        public bool CanViewProductDetails
        {
            get { return canViewProductDetails; }
            set { canViewProductDetails = value; NotifyOfPropertyChange(() => CanViewProductDetails); }
        }

        private bool canDeleteProduct;
        public bool CanDeleteProduct
        {
            get { return canDeleteProduct; }
            set { canDeleteProduct = value; NotifyOfPropertyChange(() => CanDeleteProduct); }
        }

        private bool canViewNetProfits;
        public bool CanViewNetProfits
        {
            get { return canViewNetProfits; }
            set { canViewNetProfits = value; NotifyOfPropertyChange(() => CanViewNetProfits); }
        }

        public async Task AddNewProductAsync()
        {
            if (!CanAddNewProduct) return;
            WindowManager wm = new();
            await wm.ShowWindowAsync(new ProductDetailViewModel(PCM, null, User, RefreshPageAsync));
        }

        public async Task PreviousPageAsync()
        {
            await PCM.LoadPreviousPageAsync();
            NotifyOfPropertyChange(() => Products);
        }

        public async Task NextPageAsync()
        {
            await PCM.LoadNextPageAsync();
            NotifyOfPropertyChange(() => Products);
        }

        public async Task RefreshPageAsync()
        {
            await PCM.RefreshPage();
            NotifyOfPropertyChange(() => Products);
        }

        public async Task SearchAsync()
        {
            PCM.SearchValue = SearchText;
            await PCM.LoadFirstPageAsync();
            NotifyOfPropertyChange(() => Products);
        }

        public async Task SearchBoxKeyDownHandlerAsync(ActionExecutionContext context)
        {
            if (!User.UserSettings.SearchWhenTyping && context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
            {
                await SearchAsync();
            }
        }

        public async Task SearchBoxTextChangedHandlerAsync()
        {
            if (User.UserSettings.SearchWhenTyping)
            {
                await SearchAsync();
            }
        }

        public async Task EditProductAsync()
        {
            if (!CanViewProductDetails || Products == null || Products.Count == 0 || SelectedProduct == null || SelectedProduct.Id == 0) return;
            WindowManager wm = new();
            await wm.ShowWindowAsync(new ProductDetailViewModel(PCM, SelectedProduct, User, RefreshPageAsync));
        }

        public async Task DeleteProductAsync()
        {
            if (!CanDeleteProduct || Products == null || Products.Count == 0 || SelectedProduct == null || SelectedProduct.Id == 0) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete {SelectedProduct.ProductName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = await PCM.DeleteItemAsync(SelectedProduct.Id);
            if (output) Products.Remove(SelectedProduct);
            else MessageBox.Show($"Product with ID: {SelectedProduct.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Window_PreviewKeyDown(object window, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) (GetView() as Window).Close();
        }

        public void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {
                _ = DeleteProductAsync().ConfigureAwait(true);
                e.Handled = true;
            }
        }
    }
}