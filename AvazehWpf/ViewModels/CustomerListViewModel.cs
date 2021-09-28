//using Caliburn.Micro;
//using AvazehWpfApiClient.DataAccess.Interfaces;
//using AvazehWpfApiClient.DataAccess.SqlServer;
//using AvazehWpfApiClient.Models;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Text;
//using System.Timers;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;

//namespace AvazehWpf.ViewModels
//{
//    public class CustomerListViewModel : Screen
//    {
//        public CustomerListViewModel(ICustomerCollectionManager manager)
//        {
//            _CCM = manager;
//            _SelectedCustomer = new();
//            CCM.GotoPage(1);
//        }

//        private ICustomerCollectionManager _CCM;
//        private CustomerModel _SelectedCustomer;

//        public CustomerModel SelectedCustomer
//        {
//            get { return _SelectedCustomer; }
//            set { _SelectedCustomer = value; NotifyOfPropertyChange(() => SelectedCustomer); }
//        }


//        public ICustomerCollectionManager CCM
//        {
//            get { return _CCM; }
//            set 
//            { 
//                _CCM = value;
//                NotifyOfPropertyChange(() => CCM);
//                NotifyOfPropertyChange(() => Customers);
//            }
//        }

//        public ObservableCollection<CustomerModel> Customers
//        {
//            get { return CCM.Items; }
//            set
//            {
//                CCM.Items = value;
//                NotifyOfPropertyChange(() => CCM);
//                NotifyOfPropertyChange(() => Customers);
//            }
//        }

//        public string SearchText { get; set; }

//        public void AddNewCustomer()
//        {
//            CustomerModel newCustomer = new();
//            WindowManager wm = new();
//            wm.ShowDialogAsync(new CustomerDetailViewModel(CCM, newCustomer));
//            if(newCustomer != null) Customers.Add(newCustomer);
//        }

//        public void PreviousPage()
//        {
//            CCM.LoadPreviousPage();
//            NotifyOfPropertyChange(() => Customers);
//        }

//        public void NextPage()
//        {
//            CCM.LoadNextPage();
//            NotifyOfPropertyChange(() => Customers);
//        }

//        public void Search()
//        {
//            CCM.GenerateWhereClause(SearchText);
//            NotifyOfPropertyChange(() => Customers);
//        }

//        public void SearchBoxKeyDownHandler(ActionExecutionContext context)
//        {
//            var keyArgs = context.EventArgs as KeyEventArgs;

//            if (keyArgs != null && keyArgs.Key == Key.Enter)
//            {
//                Search();
//            }
//        }

//        public void EditCustomer()
//        {
//            WindowManager wm = new();
//            wm.ShowDialogAsync(new CustomerDetailViewModel(CCM, SelectedCustomer));
//            NotifyOfPropertyChange(() => Customers);
//            NotifyOfPropertyChange(() => SelectedCustomer);
//        }

//        public void DeleteCustomer()
//        {
//            if (SelectedCustomer == null) return;
//            var result = MessageBox.Show("Are you sure ?", $"Delete {SelectedCustomer.FullName}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
//            if (result == MessageBoxResult.No) return;
//            var output = CCM.Processor.DeleteItemById(SelectedCustomer.Id);
//            if (output > 0) Customers.Remove(SelectedCustomer);
//            else MessageBox.Show($"Customer with ID: {SelectedCustomer.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//        }
//    }
//}