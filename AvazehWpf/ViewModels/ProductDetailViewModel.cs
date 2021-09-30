using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using System.Windows;
using AvazehApiClient.DataAccess;
using System.Threading.Tasks;

namespace AvazehWpf.ViewModels
{
    public class ProductDetailViewModel : ViewAware
    {
        public ProductDetailViewModel(ICollectionManager<ProductModel> manager, ProductModel Product)
        {
            Manager = manager;
            if (Product is not null)
            {
                this.Product = Product;
                _BackupProduct = new ProductModel();
                Product.Clone(ref _BackupProduct);
            }
        }

        private readonly ICollectionManager<ProductModel> Manager;
        private ProductModel _Product;
        private ProductModel _BackupProduct;
        private bool _CancelAndClose = true;

        public ProductModel Product
        {
            get { return _Product; }
            set { _Product = value; NotifyOfPropertyChange(() => Product); }
        }

        public async Task DeleteAndClose()
        {
            if (Product == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete {Product.ProductName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (await Manager.DeleteItemAsync(Product.Id) == false) MessageBox.Show($"Product with ID: {Product.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _CancelAndClose = false;
            CloseWindow();
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        public async Task SaveAndNew()
        {
            if (await SaveToDatabase() == false) return;
            _CancelAndClose = false;
            Product = new ProductModel();
            WindowManager wm = new();
            await wm.ShowWindowAsync(new ProductDetailViewModel(Manager, Product));
            CloseWindow();
        }

        public void CancelAndClose()
        {
            _CancelAndClose = true;
            CloseWindow();
        }


        public async Task SaveAndClose()
        {
            if (await SaveToDatabase() == false) return;
            _CancelAndClose = false;
            CloseWindow();
        }

        private async Task<bool> SaveToDatabase()
        {
            if (Product == null)
            {
                var result = MessageBox.Show("Product is not assigned, Nothing will be saved; Close anyway ?", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes) CloseWindow();
            }
            var validate = Manager.ValidateItem(Product);
            if (validate.IsValid)
            {
                ProductModel outPut;
                if (Product.Id == 0) //It's a new Product
                    outPut = await Manager.CreateItemAsync(Product);
                else //Update Product
                    outPut = await Manager.UpdateItemAsync(Product);
                if (outPut is null)
                {
                    MessageBox.Show($"There was a problem when saving to Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                return true;
            }
            else
            {
                var str = "";
                foreach (var error in validate.Errors)
                {
                    str += error.ErrorMessage + "\n";
                }
                MessageBox.Show(str);
                return false;
            }
        }

        public void ClosingWindow()
        {
            if (_CancelAndClose)
            {
                _BackupProduct.Clone(ref _Product);
            }
        }
    }
}