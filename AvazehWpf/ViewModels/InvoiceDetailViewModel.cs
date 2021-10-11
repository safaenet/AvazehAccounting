using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using System.Threading.Tasks;
using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using SharedLibrary.Validators;

namespace AvazehWpf.ViewModels
{
    public class InvoiceDetailViewModel : ViewAware
    {
        public InvoiceDetailViewModel(IInvoiceCollectionManager iManager, IInvoiceDetailManager dManager, InvoiceDetailSingleton singleton, int? InvoiceId, Func<Task> callBack)
        {
            InvoiceManager = iManager;
            Manager = dManager;
            CallBackFunc = callBack;
            Singleton = singleton;
            if (InvoiceId is not null)
            {
                ReloadInvoice(InvoiceId).ConfigureAwait(true);
                ReloadCustomerBalance().ConfigureAwait(true);
            }
            GetProductComboboxItems();
        }

        private readonly IInvoiceCollectionManager InvoiceManager;
        private readonly IInvoiceDetailManager Manager;
        private InvoiceDetailSingleton Singleton;
        private InvoiceModel _Invoice;
        private readonly Func<Task> CallBackFunc;
        private List<ProductNamesForComboBox> productItems;
        private InvoiceItemModel _workItem = new();

        public InvoiceItemModel SelectedItem { get; set; }
        public InvoiceItemModel WorkItem { get => _workItem; set { _workItem = value; NotifyOfPropertyChange(() => WorkItem); } }
        public int WorkItemProductId { get; set; }
        public double CustomerPreviousTotalBalance { get; private set; }
        public double CustomerTotalBalanceMinusThis => CustomerPreviousTotalBalance - (Invoice == null ? 0 : Invoice.TotalBalance);
        public List<ProductNamesForComboBox> ProductItemsForComboBox { get => productItems; set { productItems = value; NotifyOfPropertyChange(() => ProductItemsForComboBox); } }
        private ProductNamesForComboBox _selectedProductItem;

        public ProductNamesForComboBox SelectedProductItem
        {
            get { return _selectedProductItem; }
            set { _selectedProductItem = value; NotifyOfPropertyChange(() => SelectedProductItem); }
        }


        public InvoiceModel Invoice
        {
            get => _Invoice;
            set { _Invoice = value; NotifyOfPropertyChange(() => Invoice); }
        }

        private async Task ReloadInvoice(int? InvoiceId)
        {
            if(InvoiceId is null) return;
            Invoice = await InvoiceManager.GetItemById((int)InvoiceId);
        }

        private async Task ReloadCustomerBalance()
        {
            if(Invoice is null) return;
            CustomerPreviousTotalBalance = await InvoiceManager.GetCustomerTotalBalanceById(Invoice.Customer.Id, Invoice.Id);
        }

        public void EditItem()
        {
            if (Invoice == null || SelectedItem == null) return;
            WorkItem = SelectedItem;
            SelectedProductItem = ProductItemsForComboBox.SingleOrDefault(x => x.Id == WorkItem.Product.Id);
        }

        public async Task AddOrUpdateItem()
        {
            if (Invoice == null || WorkItem == null || SelectedProductItem == null) return;
            WorkItem.InvoiceId = Invoice.Id;
            ICollectionManager<ProductModel> productManager = new ProductCollectionManagerAsync<ProductModel, ProductModel_DTO_Create_Update, ProductValidator>(InvoiceManager.ApiProcessor);
            WorkItem.Product = await productManager.GetItemById(SelectedProductItem.Id);
            var validate = Manager.ValidateItem(WorkItem);
            if (!validate.IsValid)
            {
                var str = "";
                foreach (var error in validate.Errors)
                    str += error.ErrorMessage + "\n";
                MessageBox.Show(str, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (WorkItem.Id == 0) //New Item
            {
                if (Invoice.Items == null) Invoice.Items = new();
                var addedItem = await Manager.CreateItemAsync(WorkItem);
                if (addedItem is not null)
                    Invoice.Items.Add(addedItem);
            }
            else //Edit Item
            {
                var editedItem = await Manager.UpdateItemAsync(WorkItem);
                var item = Invoice.Items.FirstOrDefault(x => x.Id == WorkItem.Id);
                if (editedItem != null) editedItem.Clone(item);
            }
            WorkItem = new();
            SelectedProductItem = new();
            WorkItem = null;
            SelectedProductItem = null;
            NotifyOfPropertyChange(() => Invoice);
        }

        public async Task DeleteItem()
        {
            if (Invoice == null || Invoice.Items == null || !Invoice.Items.Any() || SelectedItem == null) return;
            if (await Manager.DeleteItemAsync(SelectedItem.Id))
                Invoice.Items.Remove(SelectedItem);
            NotifyOfPropertyChange(() => Invoice);
        }

        public async Task DeleteInvoiceAndClose()
        {
            if (Invoice == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete Invoice for {Invoice.Customer.FullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (await InvoiceManager.DeleteItemAsync(Invoice.Id) == false) MessageBox.Show($"Invoice with ID: {Invoice.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CloseWindow();
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        public async Task ClosingWindow()
        {
            await CallBackFunc?.Invoke();
        }

        public void ProductNames_PreviewTextInput(object sender, EventArgs e)
        {
            //var combo = sender as ComboBox;
            //combo.IsDropDownOpen = false;
            ////combo.Items.Clear();
            //ProductItemsForComboBox.Clear();

            //if (Singleton.ProductItemsForCombobox != null)
            //{
            //    var result = Singleton.ProductItemsForCombobox.Where(x => x.ProductName.Contains(combo.Text));
            //    ProductItemsForComboBox = result?.ToList();
            //}
            //NotifyOfPropertyChange(() => ProductItemsForComboBox);
            //var a = combo.Template.FindName("PART_EditableTextBox", combo) as TextBox;
            //combo.IsDropDownOpen = true;
            //a.SelectionStart = a.Text.Length;
            //a.CaretIndex = a.Text.Length;
        }

        private void GetProductComboboxItems()
        {
            ProductItemsForComboBox = Singleton.ProductItemsForCombobox;
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }

    #region DataGrid Row Number
    public class DataGridBehavior
    {
        #region DisplayRowNumber

        public static DependencyProperty DisplayRowNumberProperty =
            DependencyProperty.RegisterAttached("DisplayRowNumber",
                                                typeof(bool),
                                                typeof(DataGridBehavior),
                                                new FrameworkPropertyMetadata(false, OnDisplayRowNumberChanged));
        public static bool GetDisplayRowNumber(DependencyObject target)
        {
            return (bool)target.GetValue(DisplayRowNumberProperty);
        }
        public static void SetDisplayRowNumber(DependencyObject target, bool value)
        {
            target.SetValue(DisplayRowNumberProperty, value);
        }

        private static void OnDisplayRowNumberChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = target as DataGrid;
            if ((bool)e.NewValue)
            {
                EventHandler<DataGridRowEventArgs> loadedRowHandler = null;
                loadedRowHandler = (object sender, DataGridRowEventArgs ea) =>
                {
                    if (GetDisplayRowNumber(dataGrid) == false)
                    {
                        dataGrid.LoadingRow -= loadedRowHandler;
                        return;
                    }
                    ea.Row.Header = ea.Row.GetIndex() + 1;
                };
                dataGrid.LoadingRow += loadedRowHandler;

                ItemsChangedEventHandler itemsChangedHandler = null;
                itemsChangedHandler = (object sender, ItemsChangedEventArgs ea) =>
                {
                    if (GetDisplayRowNumber(dataGrid) == false)
                    {
                        dataGrid.ItemContainerGenerator.ItemsChanged -= itemsChangedHandler;
                        return;
                    }
                    GetVisualChildCollection<DataGridRow>(dataGrid).
                        ForEach(d => d.Header = d.GetIndex() + 1);
                };
                dataGrid.ItemContainerGenerator.ItemsChanged += itemsChangedHandler;
            }
        }

        #endregion // DisplayRowNumber

        #region Get Visuals

        private static List<T> GetVisualChildCollection<T>(object parent) where T : Visual
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                if (child != null)
                {
                    GetVisualChildCollection(child, visualCollection);
                }
            }
        }

        #endregion // Get Visuals
    }
    #endregion
}