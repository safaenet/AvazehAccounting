using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace AvazehWpf.ViewModels
{
    public class NewInvoiceViewModel : ViewAware
    {
        public NewInvoiceViewModel(InvoiceDetailSingleton singleton, int? Id, IInvoiceCollectionManager icManager, ICollectionManager<CustomerModel> ccManager)
        {
            ICM = icManager;
            CCM = ccManager;
            Singleton = singleton;
            ID = Id;
            if (ID == null) ButtonTitle = "Add"; else ButtonTitle = "Update";
            GetComboboxItems().ConfigureAwait(true);
        }
        public IInvoiceCollectionManager ICM { get; set; }
        public ICollectionManager<CustomerModel> CCM { get; set; }
        private InvoiceDetailSingleton Singleton;
        private int? ID;
        private ObservableCollection<ItemsForComboBox> customerNames;
        public ObservableCollection<ItemsForComboBox> CustomerNamesForComboBox { get => customerNames; set { customerNames = value; NotifyOfPropertyChange(() => CustomerNamesForComboBox); } }
        private bool isCustomerInputDropDownOpen;
        private ItemsForComboBox _selectedCustomer;
        private string customerInput;
        private string buttonTitle;

        public string ButtonTitle
        {
            get { return buttonTitle; }
            set { buttonTitle = value; NotifyOfPropertyChange(() => IsCustomerInputDropDownOpen); }
        }

        public bool IsCustomerInputDropDownOpen { get => isCustomerInputDropDownOpen; set { isCustomerInputDropDownOpen = value; NotifyOfPropertyChange(() => IsCustomerInputDropDownOpen); } }

        public ItemsForComboBox SelectedCustomer
        {
            get => _selectedCustomer;
            set { _selectedCustomer = value; NotifyOfPropertyChange(() => SelectedCustomer); }
        }

        public string CustomerInput
        {
            get => customerInput;
            set { customerInput = value; NotifyOfPropertyChange(() => CustomerInput); }
        }

        private async Task GetComboboxItems()
        {
            CustomerNamesForComboBox = await Singleton.ReloadCustomerNames();
        }

        public void CustomerNames_PreviewTextInput()
        {
            IsCustomerInputDropDownOpen = true;
        }

        public async Task AddOrUpdateInvoice()
        {
            if (SelectedCustomer == null)
            {
                var res = MessageBox.Show("مشتری مورد نظر وجود ندارد. آیا ایجاد شود؟", "ایجاد مشتری جدید", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (res == MessageBoxResult.Yes)
                {

                }
            }
            if (ID is null) //Add new
            {

            }
            else //Update
            {
                int id = (int)ID;
                var invoice = await ICM.GetItemById(id);
                if (invoice != null)
                {
                    var c = await CCM.GetItemById(id);
                    if (c == null)
                    {

                    }
                    invoice.Customer.Id = id;
                    await ICM.UpdateItemAsync(invoice);
                }
                else MessageBox.Show("Cannot find such invoice.");
            }
        }
    }
}