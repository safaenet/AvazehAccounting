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
        public NewInvoiceViewModel(InvoiceDetailSingleton singleton)
        {
            Singleton = singleton;
            GetComboboxItems().ConfigureAwait(true);
        }
        private InvoiceDetailSingleton Singleton;
        private ObservableCollection<ItemsForComboBox> customerNames;
        public ObservableCollection<ItemsForComboBox> CustomerNamesForComboBox { get => customerNames; set { customerNames = value; NotifyOfPropertyChange(() => CustomerNamesForComboBox); } }
        private bool isCustomerInputDropDownOpen;
        private ItemsForComboBox _selectedCustomer;
        private string customerInput;

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
    }
}