using AvazehApiClient.DataAccess.Interfaces;
using AvazehApiClient.Models;
using Caliburn.Micro;
using System.Collections.ObjectModel;
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
            Search();
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

        public void PreviousPage()
        {
            PCM.LoadPreviousPageAsync();
            NotifyOfPropertyChange(() => Products);
        }

        public void NextPage()
        {
            PCM.LoadNextPageAsync();
            NotifyOfPropertyChange(() => Products);
        }

        public void Search()
        {
            PCM.SearchValue = SearchText;
            PCM.LoadFirstPageAsync();
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

        public void EditProduct()
        {
            WindowManager wm = new();
            //wm.ShowDialogAsync(new ProductDetailViewModel(PCM, SelectedProduct));
            NotifyOfPropertyChange(() => Products);
            NotifyOfPropertyChange(() => SelectedProduct);
        }

        public void DeleteProduct()
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