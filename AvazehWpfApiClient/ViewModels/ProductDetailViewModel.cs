using Caliburn.Micro;
using AvazehWpfApiClient.DataAccess.SqlServer;
using AvazehWpfApiClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AvazehWpfApiClient.DataAccess.Interfaces;

namespace AvazehWpf.ViewModels
{
    public class ProductDetailViewModel : ViewAware
    {
        public ProductDetailViewModel(IProductCollectionManager manager, ProductModel Product)
        {
            Manager = manager;
            if (Product != null)
            {
                this.Product = Product;
                _BackupProduct = new ProductModel();
                CloneProduct(Product, ref _BackupProduct);
            }
        }

        private readonly IProductCollectionManager Manager;
        private ProductModel _Product;
        private ProductModel _BackupProduct;
        private bool _CancelAndClose = true;

        public ProductModel Product
        {
            get { return _Product; }
            set { _Product = value; NotifyOfPropertyChange(() => Product); }
        }

        public void DeleteAndClose()
        {
            if (Product == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete {Product.ProductName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (Manager.Processor.DeleteItemById(Product.Id) == 0) MessageBox.Show($"Product with ID: {Product.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _CancelAndClose = false;
            CloseWindow();
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        public void SaveAndNew()
        {
            if (SaveToDatabase() == 0) return;
            _CancelAndClose = false;
            Product = new ProductModel();
            WindowManager wm = new();
            wm.ShowWindowAsync(new ProductDetailViewModel(Manager, Product));
            CloseWindow();
        }

        public void CancelAndClose()
        {
            _CancelAndClose = true;
            CloseWindow();
        }


        public void SaveAndClose()
        {
            if (SaveToDatabase() == 0) return;
            _CancelAndClose = false;
            CloseWindow();
        }

        private int SaveToDatabase()
        {
            var validate = Manager.Processor.ValidateItem(Product);
            if (validate.IsValid)
            {
                if (Product == null)
                {
                    var result = MessageBox.Show("Product is not assigned, Nothing will be saved; Close anyway ?", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes) CloseWindow();
                }
                int outPut;
                if (Product.Id == 0) //It's a new Product
                {
                    outPut = Manager.Processor.CreateItem(Product);
                    if (outPut == 0) MessageBox.Show($"There was a problem when saving to Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else //Update Product
                {

                    outPut = Manager.Processor.UpdateItem(Product);
                    if (outPut == 0) MessageBox.Show($"There was a problem when updating the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return outPut;
            }
            else
            {
                var str = "";
                foreach (var error in validate.Errors)
                {
                    str += error.ErrorMessage + "\n";
                }
                MessageBox.Show(str);
                return 0;
            }
        }

        private static void CloneProduct(ProductModel From, ref ProductModel To)
        {
            if (From == null || To == null) return;
            To.Id = From.Id;
            To.ProductName = From.ProductName;
            To.BuyPrice = From.BuyPrice;
            To.SellPrice = From.SellPrice;
            To.Barcode = From.Barcode;
            To.Descriptions = From.Descriptions;
        }

        public void ClosingWindow()
        {
            if (_CancelAndClose)
            {
                CloneProduct(_BackupProduct, ref _Product);
            }
        }
    }
}