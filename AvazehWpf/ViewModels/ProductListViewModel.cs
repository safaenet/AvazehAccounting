using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class ProductListViewModel : Screen
    {
        public ProductListViewModel(ICollectionManager<ProductModel> manager)
        {
            PCM = manager;
            _SelectedProduct = new();
            _ = SearchAsync().ConfigureAwait(true);
        }

        private ICollectionManager<ProductModel> _PCM;
        private ProductModel _SelectedProduct;

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

        public async Task AddNewProductAsync()
        {
            WindowManager wm = new();
            await wm.ShowWindowAsync(new ProductDetailViewModel(PCM, null, RefreshPageAsync));
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
            if (context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
            {
                await SearchAsync();
            }
        }

        public async Task EditProductAsync()
        {
            if (Products == null || Products.Count == 0 || SelectedProduct == null || SelectedProduct.Id == 0) return;
            WindowManager wm = new();
            await wm.ShowWindowAsync(new ProductDetailViewModel(PCM, SelectedProduct, RefreshPageAsync));
        }

        public async Task DeleteProductAsync()
        {
            if (Products == null || Products.Count == 0 || SelectedProduct == null || SelectedProduct.Id == 0) return;
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