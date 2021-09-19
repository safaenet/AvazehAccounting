using Caliburn.Micro;
using DataLibraryCore.DataAccess.CollectionManagers;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class ProductListViewModel : Screen
    {
        public ProductListViewModel(IProductCollectionManager manager)
        {
            PCM = manager;
            _SelectedProduct = new();
            PCM.LoadFirstPageAsync();
        }

        private IProductCollectionManager _PCM;
        private ProductModel _SelectedProduct;

        public ProductModel SelectedProduct
        {
            get { return _SelectedProduct; }
            set { _SelectedProduct = value; NotifyOfPropertyChange(() => SelectedProduct); }
        }

        public IProductCollectionManager PCM
        {
            get { return _PCM; }
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
            wm.ShowDialogAsync(new ProductDetailViewModel(PCM, newProduct));
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
            PCM.GenerateWhereClauseAsync(SearchText);
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
            wm.ShowDialogAsync(new ProductDetailViewModel(PCM, SelectedProduct));
            NotifyOfPropertyChange(() => Products);
            NotifyOfPropertyChange(() => SelectedProduct);
        }

        public void DeleteProduct()
        {
            if (SelectedProduct == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete {SelectedProduct.ProductName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = PCM.Processor.DeleteItemByIdAsync(SelectedProduct.Id);
            if (output.Result > 0) Products.Remove(SelectedProduct);
            else MessageBox.Show($"Product with ID: {SelectedProduct.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}