using AvazehApiClient.DataAccess.Interfaces;
using AvazehApiClient.Models;
using Caliburn.Micro;
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
            Search().ConfigureAwait(true);
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

        public void AddNewProduct()
        {
            ProductModel newProduct = new();
            WindowManager wm = new();
            //wm.ShowDialogAsync(new ProductDetailViewModel(PCM, newProduct));
            if (newProduct != null) Products.Add(newProduct);
        }

        public async Task PreviousPage()
        {
            await PCM.LoadPreviousPageAsync();
            NotifyOfPropertyChange(() => Products);
        }

        public async Task NextPage()
        {
            await PCM.LoadNextPageAsync();
            NotifyOfPropertyChange(() => Products);
        }

        public async Task Search()
        {
            PCM.SearchValue = SearchText;
            await PCM.LoadFirstPageAsync();
            NotifyOfPropertyChange(() => Products);
        }

        public void SearchBoxKeyDownHandler(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                Search();
            }
        }

        public async Task EditProduct()
        {
            WindowManager wm = new();
            //wm.ShowDialogAsync(new ProductDetailViewModel(PCM, SelectedProduct));
            NotifyOfPropertyChange(() => Products);
            NotifyOfPropertyChange(() => SelectedProduct);
        }

        public async Task DeleteProduct()
        {
            if (SelectedProduct == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete {SelectedProduct.ProductName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = PCM.DeleteItemAsync(SelectedProduct.Id);
            if (output.Result) Products.Remove(SelectedProduct);
            else MessageBox.Show($"Product with ID: {SelectedProduct.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}