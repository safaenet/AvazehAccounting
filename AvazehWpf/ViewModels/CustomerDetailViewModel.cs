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
    public class CustomerDetailViewModel : ViewAware
    {
        public CustomerDetailViewModel(ICustomerCollectionManager manager, CustomerModel Customer)
        {
            Manager = manager;
            if (Customer != null)
            {
                if (Customer.Id == 0) Customer.DateJoined = PersianCalendarModel.GetCurrentPersianDate();
                this.Customer = Customer;
                _BackupCustomer = new CustomerModel();
                CloneCustomer(Customer, ref _BackupCustomer);
            }
        }

        private readonly ICustomerCollectionManager Manager;
        private CustomerModel _Customer;
        private CustomerModel _BackupCustomer;
        private bool _CancelAndClose = true;

        public CustomerModel Customer
        {
            get { return _Customer; }
            set { _Customer = value; NotifyOfPropertyChange(() => Customer); }
        }
        
        public void AddNewPhoneNumber()
        {
            PhoneNumberModel newPhone = new PhoneNumberModel();
            if (Customer.PhoneNumbers == null)
            {
                Customer.PhoneNumbers = new System.Collections.ObjectModel.ObservableCollection<PhoneNumberModel>();
                NotifyOfPropertyChange(() => Customer);
            }
            newPhone.CustomerId = Customer.Id;
            Customer.PhoneNumbers.Add(newPhone);
        }

        public void DeletePhoneNumber()
        {
            if(Customer == null || Customer.PhoneNumbers == null || !Customer.PhoneNumbers.Any()) return;
            Customer.PhoneNumbers.RemoveAt(Customer.PhoneNumbers.Count - 1);
        }

        public void DeleteAndClose()
        {
            if (Customer == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete {Customer.FullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (Manager.Processor.DeleteItemById(Customer.Id) == 0) MessageBox.Show($"Customer with ID: {Customer.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            Customer = new CustomerModel();
            WindowManager wm = new WindowManager();
            wm.ShowWindowAsync(new CustomerDetailViewModel(Manager, Customer));
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
            if (string.IsNullOrEmpty(Customer.DateJoined)) Customer.DateJoined = PersianCalendarModel.GetCurrentPersianDate();
            var validate = Manager.Processor.ValidateItem(Customer);
            if (validate.IsValid)
            {
                int outPut = 0;
                if (Customer == null)
                {
                    var result = MessageBox.Show("Customer is not assigned, Nothing will be saved; Close anyway ?", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes) CloseWindow();
                }
                if (Customer.Id == 0) //It's a new Customer
                {
                    outPut = Manager.Processor.CreateItem(Customer);
                    if (outPut == 0) MessageBox.Show($"There was a problem when saving to Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else //Update Customer
                {
                    
                    outPut = Manager.Processor.UpdateItem(Customer);
                    if (outPut == 0) MessageBox.Show($"There was a problem when updating the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return outPut;
            }
            else
            {
                string str = "";
                foreach (var error in validate.Errors)
                {
                    str += error.ErrorMessage + "\n";
                }
                MessageBox.Show(str);
                return 0;
            }
        }

        private void CloneCustomer(CustomerModel From, ref CustomerModel To)
        {
            if (From == null || To == null) return;
            To.CompanyName = From.CompanyName;
            To.DateJoined = From.DateJoined;
            To.Descriptions = From.Descriptions;
            To.EmailAddress = From.EmailAddress;
            To.FirstName = From.FirstName;
            To.Id = From.Id;
            To.LastName = From.LastName;
            if (From.PhoneNumbers != null)
            {
                To.PhoneNumbers = new System.Collections.ObjectModel.ObservableCollection<PhoneNumberModel>();
                foreach (var phone in From.PhoneNumbers)
                {
                    To.PhoneNumbers.Add(phone);
                }
            }
            To.PostAddress = From.PostAddress;
        }

        public void ClosingWindow()
        {
            if(_CancelAndClose)
            {
                CloneCustomer(_BackupCustomer, ref _Customer);
            }
        }
    }
}