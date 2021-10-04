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
                this.Customer = Customer;
        }

        private readonly ICollectionManager<CustomerModel> Manager;
        private CustomerModel _Customer;
        private Func<Task> CallBackFunc;

        public CustomerModel Customer
        {
            get => _Customer;
            set { _Customer = value; NotifyOfPropertyChange(() => Customer); }
        }

        public void AddNewPhoneNumber()
        {
            PhoneNumberModel newPhone = new();
            if (Customer.PhoneNumbers == null)
            {
                Customer.PhoneNumbers = new();
                NotifyOfPropertyChange(() => Customer);
            }
            newPhone.CustomerId = Customer.Id;
            Customer.PhoneNumbers.Add(newPhone);
        }

        public void DeletePhoneNumber()
        {
            if (Customer == null || Customer.PhoneNumbers == null || !Customer.PhoneNumbers.Any()) return;
            Customer.PhoneNumbers.RemoveAt(Customer.PhoneNumbers.Count - 1);
        }

        public async Task DeleteAndClose()
        {
            if (Customer == null || Customer.Id == 0) return;
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
            if (Customer == null) return false;
            var validate = Manager.ValidateItem(Customer);
            if (validate.IsValid)
            {
                CustomerModel outPut;
                if (Customer.Id == 0) //It's a new Customer
                    outPut = await Manager.CreateItemAsync(Customer);
                else //Update Customer
                    outPut = await Manager.UpdateItemAsync(Customer);
                if (outPut is null)
                {
                    MessageBox.Show($"There was a problem when saving to Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                outPut.Clone(Customer);
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

        public async Task ClosingWindow()
        {
            await CallBackFunc?.Invoke();
        }
    }
}