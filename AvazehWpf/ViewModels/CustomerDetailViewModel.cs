using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AvazehWpf.ViewModels
{
    public class CustomerDetailViewModel : ViewAware
    {
        public CustomerDetailViewModel(ICollectionManager<CustomerModel> manager, CustomerModel Customer, Func<Task> callBack)
        {
            Manager = manager;
            CallBackFunc = callBack;
            if (Customer is not null)
            {
                BackupCustomer = new();
                this.Customer = Customer;
                Customer.Clone(BackupCustomer);
            }
        }

        private readonly ICollectionManager<CustomerModel> Manager;
        private CustomerModel _Customer;
        private CustomerModel _BackupCustomer;
        private Func<Task> CallBackFunc;

        public CustomerModel Customer
        {
            get { return _Customer; }
            set { _Customer = value; NotifyOfPropertyChange(() => Customer); }
        }

        public CustomerModel BackupCustomer
        {
            get => _BackupCustomer;
            set
            {
                _BackupCustomer = value;
                NotifyOfPropertyChange(() => BackupCustomer);
            }
        }

        public void AddNewPhoneNumber()
        {
            PhoneNumberModel newPhone = new PhoneNumberModel();
            if (BackupCustomer.PhoneNumbers == null)
            {
                BackupCustomer.PhoneNumbers = new System.Collections.ObjectModel.ObservableCollection<PhoneNumberModel>();
                NotifyOfPropertyChange(() => Customer);
            }
            newPhone.CustomerId = BackupCustomer.Id;
            BackupCustomer.PhoneNumbers.Add(newPhone);
        }

        public void DeletePhoneNumber()
        {
            if (BackupCustomer == null || BackupCustomer.PhoneNumbers == null || !BackupCustomer.PhoneNumbers.Any()) return;
            BackupCustomer.PhoneNumbers.RemoveAt(BackupCustomer.PhoneNumbers.Count - 1);
        }

        public async Task DeleteAndClose()
        {
            if (Customer == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete {Customer.FullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (await Manager.DeleteItemAsync(Customer.Id) == false) MessageBox.Show($"Customer with ID: {Customer.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CloseWindow();
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        public async Task SaveAndNew()
        {
            if (await SaveToDatabase() == false) return;
            var newCustomer = new CustomerModel();
            WindowManager wm = new();
            await wm.ShowWindowAsync(new CustomerDetailViewModel(Manager, newCustomer, CallBackFunc));
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
            if (BackupCustomer == null)
                return false;
            var validate = Manager.ValidateItem(BackupCustomer);
            if (validate.IsValid)
            {
                CustomerModel outPut;
                if (Customer.Id == 0) //It's a new Customer
                    outPut = await Manager.CreateItemAsync(BackupCustomer);
                else //Update Customer
                    outPut = await Manager.UpdateItemAsync(BackupCustomer);
                if (outPut is null)
                {
                    MessageBox.Show($"There was a problem when saving to Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                outPut.Clone(BackupCustomer);
                BackupCustomer.Clone(Customer);
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
            Customer.Clone(BackupCustomer);
            CallBackFunc();
        }
    }
}