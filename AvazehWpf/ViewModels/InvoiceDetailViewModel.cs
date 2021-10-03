using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using System.Threading.Tasks;

namespace AvazehWpf.ViewModels
{
    public class InvoiceDetailViewModel : ViewAware
    {
        public InvoiceDetailViewModel(IInvoiceCollectionManager manager, InvoiceModel_DTO_Read invoiceDto, Func<Task> callBack)
        {
            Manager = manager;
            CallBackFunc = callBack;
            if (invoiceDto is not null)
            {
                Invoice = invoiceDto.Invoice;
                //if (invoice.Id == 0) invoice.DateCreated = PersianCalendarModel.GetCurrentPersianDate();
                CustomerTotalBalance = invoiceDto.CustomerTotalBalance;
            }
            ProductNames = new();
            ProductItemsForComboBox = GetProductItems();
        }

        private readonly IInvoiceCollectionManager Manager;
        private InvoiceModel _Invoice;
        private Func<Task> CallBackFunc;
        private Dictionary<int, string> productItems;

        public InvoiceItemModel SelectedItem { get; set; }
        public InvoiceItemModel WorkItem { get; set; }
        public int WorkItemProductId { get; set; }
        public double CustomerTotalBalance { get; set; }
        public Dictionary<int, string> ProductItemsForComboBox { get => productItems; set { productItems = value; NotifyOfPropertyChange(() => ProductItemsForComboBox); } }

        public InvoiceModel Invoice
        {
            get { return _Invoice; }
            set { _Invoice = value; NotifyOfPropertyChange(() => Invoice); }
        }

        public void AddOrUpdateItem()
        {
            if (Invoice == null) return;
            if (Invoice.Items == null) Invoice.Items = new();
            if (WorkItem.Id == 0) //New Item
            {

                Invoice.Items.Add(WorkItem);
            }
            else //Edit Item
            {

            }
            WorkItem = new();
            NotifyOfPropertyChange(() => Invoice);
        }

        public void DeleteItem()
        {
            if (Invoice == null || Invoice.Items == null || !Invoice.Items.Any()) return;
            Invoice.Items.Remove(SelectedItem);
        }

        public async Task DeleteInvoiceAndClose()
        {
            if (Invoice == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete Invoice for {Invoice.Customer.FullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (await Manager.DeleteItemAsync(Invoice.Id) == false) MessageBox.Show($"Invoice with ID: {Invoice.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CloseWindow();
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        public void ClosingWindow()
        {
            CallBackFunc();
        }

        private Dictionary<int, string> GetProductItems()
        {
            //return Manager.Processor.GetProductItems();
            throw new NotImplementedException();
        }

        void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        #region Product ComboBox Suggestion Methods
        public List<string> ProductNames { get; set; }
        public static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        public void PreviewTextInput_EnhanceComboSearch(object sender, TextCompositionEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            cmb.IsDropDownOpen = true;

            if (!string.IsNullOrEmpty(cmb.Text))
            {
                string fullText = cmb.Text.Insert(GetChildOfType<TextBox>(cmb).CaretIndex, e.Text);
                cmb.ItemsSource = ProductNames.Where(s => s.IndexOf(fullText, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
            }
            else if (!string.IsNullOrEmpty(e.Text))
            {
                cmb.ItemsSource = ProductNames.Where(s => s.IndexOf(e.Text, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
            }
            else
            {
                cmb.ItemsSource = ProductNames;
            }
        }

        public void Pasting_EnhanceComboSearch(object sender, DataObjectPastingEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            cmb.IsDropDownOpen = true;

            string pastedText = (string)e.DataObject.GetData(typeof(string));
            string fullText = cmb.Text.Insert(GetChildOfType<TextBox>(cmb).CaretIndex, pastedText);

            if (!string.IsNullOrEmpty(fullText))
            {
                cmb.ItemsSource = ProductNames.Where(s => s.IndexOf(fullText, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
            }
            else
            {
                cmb.ItemsSource = ProductNames;
            }
        }

        public void PreviewKeyUp_EnhanceComboSearch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                ComboBox cmb = (ComboBox)sender;

                cmb.IsDropDownOpen = true;

                if (!string.IsNullOrEmpty(cmb.Text))
                {
                    cmb.ItemsSource = ProductNames.Where(s => s.IndexOf(cmb.Text, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
                }
                else
                {
                    cmb.ItemsSource = ProductNames;
                }
            }
        }
        #endregion
    }
    public static class ProductNameItems //For ComboBox
    {
        public static Dictionary<int, string> GetProductItems()
        {
            Dictionary<int, string> choices = new();
            //string sql = $@"SELECT p.Id, p.ProductName FROM Products p";
            //using IDbConnection conn = new SqlConnection(DataAccess.GetConnectionString());
            //var items = conn.Query<ProductModel>(sql, null);
            //choices = items.ToDictionary(x => x.Id, x => x.ProductName);
            return choices;
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