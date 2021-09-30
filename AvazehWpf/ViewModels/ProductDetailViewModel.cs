﻿using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using System.Windows;
using AvazehApiClient.DataAccess;
using System.Threading.Tasks;
using System;

namespace AvazehWpf.ViewModels
{
    public class ProductDetailViewModel : ViewAware
    {
        public ProductDetailViewModel(ICollectionManager<ProductModel> manager, ProductModel Product, Func<Task> callBack)
        {
            Manager = manager;
            CallBackFunc = callBack;
            if (Product is not null)
            {
                BackupProduct = new();
                this.Product = Product;
                Product.Clone(BackupProduct);
            }
        }

        private readonly ICollectionManager<ProductModel> Manager;
        private ProductModel _Product;
        private ProductModel _BackupProduct;
        private Func<Task> CallBackFunc;

        public ProductModel Product
        {
            get => _Product;
            set { _Product = value; NotifyOfPropertyChange(() => Product); }
        }

        public ProductModel BackupProduct
        {
            get => _BackupProduct;
            set
            {
                _BackupProduct = value;
                NotifyOfPropertyChange(() => BackupProduct);
            }
        }

        public async Task DeleteAndClose()
        {
            if (Product == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete {Product.ProductName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (await Manager.DeleteItemAsync(Product.Id) == false) MessageBox.Show($"Product with ID: {Product.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CloseWindow();
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        public async Task SaveAndNew()
        {
            if (await SaveToDatabase() == false) return;
            var newProduct = new ProductModel();
            WindowManager wm = new();
            await wm.ShowWindowAsync(new ProductDetailViewModel(Manager, newProduct, CallBackFunc));
            CloseWindow();
        }

        public void CancelAndClose()
        {
            CloseWindow();
        }


        public async Task SaveAndClose()
        {
            if (await SaveToDatabase() == false) return;
            CloseWindow();
        }

        private async Task<bool> SaveToDatabase()
        {
            if (BackupProduct == null)
                return false;
            var validate = Manager.ValidateItem(BackupProduct);
            if (validate.IsValid)
            {
                ProductModel outPut;
                if (Product.Id == 0) //It's a new Product
                    outPut = await Manager.CreateItemAsync(BackupProduct);
                else //Update Product
                    outPut = await Manager.UpdateItemAsync(BackupProduct);
                if (outPut is null)
                {
                    MessageBox.Show($"There was a problem when saving to Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                outPut.Clone(BackupProduct);
                BackupProduct.Clone(Product);
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
            Product.Clone(BackupProduct);
            CallBackFunc();
        }
    }
}