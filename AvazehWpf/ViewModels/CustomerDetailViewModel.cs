using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.SecurityAndSettingsModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class CustomerDetailViewModel : ViewAware
    {
        public CustomerDetailViewModel(ICollectionManager<CustomerModel> manager, CustomerModel customer, LoggedInUser_DTO user, Func<Task> callBack)
        {
            Manager = manager;
            User = user;
            CallBackFunc = callBack;
            LoadSettings();
            if (customer is not null)
            {
                Customer = customer;
                WindowTitle = customer.FullName + " - مشتری"; ;
            }
            else
            {
                Customer = new();
                WindowTitle = "مشتری جدید";
            }
        }

        private void LoadSettings()
        {
            CanEditCustomer = Manager.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanEditCustomer));
            CanDeleteCustomer = Manager.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanDeleteCustomer));
        }

        private readonly ICollectionManager<CustomerModel> Manager;
        private CustomerModel _Customer;
        private Func<Task> CallBackFunc;
        private string windowTitle;

        public string WindowTitle
        {
            get { return windowTitle; }
            set { windowTitle = value; NotifyOfPropertyChange(() => WindowTitle); }
        }

        public CustomerModel Customer
        {
            get => _Customer;
            set { _Customer = value; NotifyOfPropertyChange(() => Customer); }
        }

        public LoggedInUser_DTO User { get; init; }

        private bool canEditCustomer;
        public bool CanEditCustomer
        {
            get { return canEditCustomer; }
            set { canEditCustomer = value; NotifyOfPropertyChange(() => CanEditCustomer); }
        }
        private bool canDeleteCustomer;
        public bool CanDeleteCustomer
        {
            get { return canDeleteCustomer; }
            set { canDeleteCustomer = value; NotifyOfPropertyChange(() => CanDeleteCustomer); }
        }

        public void AddNewPhoneNumber()
        {
            if (!CanEditCustomer) return;
            if (Customer == null) Customer = new();
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
            if (!CanEditCustomer) return;
            if (Customer == null || Customer.PhoneNumbers == null || !Customer.PhoneNumbers.Any()) return;
            Customer.PhoneNumbers.RemoveAt(Customer.PhoneNumbers.Count - 1);
        }

        public async Task DeleteAndCloseAsync()
        {
            if (!CanDeleteCustomer) return;
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

        public async Task SaveAndNewAsync()
        {
            if (!CanEditCustomer) return;
            if (await SaveToDatabaseAsync() == false) return;
            var newCustomer = new CustomerModel();
            WindowManager wm = new();
            await wm.ShowWindowAsync(new CustomerDetailViewModel(Manager, newCustomer, User, CallBackFunc));
            CloseWindow();
        }

        public void CancelAndClose()
        {
            CloseWindow();
        }

        public async Task SaveAndCloseAsync()
        {
            if (!CanEditCustomer) return;
            if (await SaveToDatabaseAsync() == false) return;
            CloseWindow();
        }

        private async Task<bool> SaveToDatabaseAsync()
        {
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

        public async Task ClosingWindowAsync()
        {
            await CallBackFunc?.Invoke();
        }

        public void Window_PreviewKeyDown(object window, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) (GetView() as Window).Close();
        }
    }
}