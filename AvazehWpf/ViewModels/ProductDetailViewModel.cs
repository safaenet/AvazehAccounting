using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using System.Windows;
using AvazehApiClient.DataAccess;
using System.Threading.Tasks;
using System;
using System.Windows.Input;
using SharedLibrary.SecurityAndSettingsModels;

namespace AvazehWpf.ViewModels;

public class ProductDetailViewModel : ViewAware
{
    public ProductDetailViewModel(ICollectionManager<ProductModel> manager, ProductModel product, LoggedInUser_DTO user, Func<Task> callBack)
    {
        Manager = manager;
        User = user;
        CallBackFunc = callBack;
        LoadSettings();
        if (product is not null)
        {
            Product = product;
            WindowTitle = product.ProductName + " - کالا"; ;
        }
        else
        {
            Product = new();
            WindowTitle = "کالای جدید";
        }
    }

    private void LoadSettings()
    {
        CanEditProduct = Manager.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanEditProduct));
        CanDeleteProduct = Manager.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanDeleteProduct));
    }

    private readonly ICollectionManager<ProductModel> Manager;
    public LoggedInUser_DTO User { get; init; }
    private ProductModel _Product;
    //private ProductModel _BackupProduct;
    private Func<Task> CallBackFunc;
    private string windowTitle;

    public string WindowTitle
    {
        get { return windowTitle; }
        set { windowTitle = value; NotifyOfPropertyChange(() => WindowTitle); }
    }

    public ProductModel Product
    {
        get => _Product;
        set { _Product = value; NotifyOfPropertyChange(() => Product); }
    }
    private bool canEditProduct;
    public bool CanEditProduct
    {
        get { return canEditProduct; }
        set { canEditProduct = value; NotifyOfPropertyChange(() => CanEditProduct); }
    }
    private bool canDeleteProduct;
    public bool CanDeleteProduct
    {
        get { return canDeleteProduct; }
        set { canDeleteProduct = value; NotifyOfPropertyChange(() => CanDeleteProduct); }
    }

    public async Task DeleteAndCloseAsync()
    {
        if (!CanDeleteProduct || Product == null || Product.Id == 0) return;
        var result = MessageBox.Show("Are you sure ?", $"Delete {Product.ProductName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
        if (result == MessageBoxResult.No) return;
        if (await Manager.DeleteItemAsync(Product.Id) == false) MessageBox.Show($"Product with ID: {Product.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        CloseWindow();
    }

    public void CloseWindow()
    {
        (GetView() as Window).Close();
    }

    public async Task SaveAndNewAsync()
    {
        if (!CanEditProduct) return;
        if (await SaveToDatabaseAsync() == false) return;
        var newProduct = new ProductModel();
        WindowManager wm = new();
        await wm.ShowWindowAsync(new ProductDetailViewModel(Manager, newProduct, User, CallBackFunc));
        CloseWindow();
    }

    public void CancelAndClose()
    {
        CloseWindow();
    }

    public async Task SaveAndCloseAsync()
    {
        if (!CanEditProduct) return;
        if (await SaveToDatabaseAsync() == false) return;
        CloseWindow();
    }

    private async Task<bool> SaveToDatabaseAsync()
    {
        if (!CanEditProduct || Product == null) return false;
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
            outPut.Clone(Product);
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

    public async Task ClosingWindowAsync()
    {
        await CallBackFunc?.Invoke();
    }

    public void Window_PreviewKeyDown(object window, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) (GetView() as Window).Close();
    }
}