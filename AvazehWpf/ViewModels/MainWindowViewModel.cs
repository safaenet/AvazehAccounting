﻿using Caliburn.Micro;
using System.Windows;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using System.Threading.Tasks;
using AvazehApiClient.DataAccess;
using System.Net.Http;
using SharedLibrary.SecurityAndSettingsModels;
using System.Security.Claims;

namespace AvazehWpf.ViewModels
{
    public class MainWindowViewModel : ViewAware
    {
        public MainWindowViewModel(LoggedInUser_DTO user, SimpleContainer sc)
        {
            User = user;
            SC = sc;
            _ = LoadKnowledgeOfTheDayAsync().ConfigureAwait(true);
            ApiProcessor = SC.GetInstance<IApiProcessor>();
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (User != null) UserFullName = $"{ApiProcessor.GetRoleValue(ClaimTypes.GivenName)} {ApiProcessor.GetRoleValue(ClaimTypes.Surname)}";
            CanAddNewInvoiceAsync = ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanAddNewInvoice));
            CanAddNewTransactionAsync = ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanAddNewTransaction));
            CanViewInvoiceListAsync = ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanViewInvoicesList));
            CanViewTransactionListAsync = ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanViewTransactionsList));
            CanViewChequesAsync = ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanViewChequesList));
            CanViewCustomersAsync = ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanViewCustomersList));
            CanViewProductsAsync = ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanViewProductsList));
            CanViewSettingsAsync = ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanManageItself)) || ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanManageOthers));
        }

        public LoggedInUser_DTO User
        {
            get => user;
            set
            {
                user = value; NotifyOfPropertyChange(() => User);
            }
        }
        private readonly SimpleContainer SC;
        private bool settingsLoaded;
        private readonly IApiProcessor ApiProcessor;

        public bool SettingsLoaded
        {
            get { return settingsLoaded; }
            set { settingsLoaded = value; NotifyOfPropertyChange(() => SettingsLoaded); }
        }

        private bool kodAvailable;
        private KnowledgeModel knowledgeOfTheDay;
        private string userFullName;

        public bool KodAvailable
        {
            get { return kodAvailable; }
            set { kodAvailable = value; NotifyOfPropertyChange(() => KodAvailable); }
        }

        public KnowledgeModel KnowledgeOfTheDay
        {
            get { return knowledgeOfTheDay; }
            set { knowledgeOfTheDay = value; NotifyOfPropertyChange(() => KnowledgeOfTheDay); }
        }

        public string UserFullName
        {
            get => userFullName;
            set
            {
                userFullName = value;
                NotifyOfPropertyChange(() => UserFullName);
            }
        }

        private bool canAddNewInvoiceAsync;
        public bool CanAddNewInvoiceAsync
        {
            get { return canAddNewInvoiceAsync; }
            set { canAddNewInvoiceAsync = value; NotifyOfPropertyChange(() => CanAddNewInvoiceAsync); }
        }

        private bool canAddNewTransactionAsync;
        public bool CanAddNewTransactionAsync
        {
            get { return canAddNewTransactionAsync; }
            set { canAddNewTransactionAsync = value; NotifyOfPropertyChange(() => CanAddNewTransactionAsync); }
        }

        private bool canViewInvoiceListAsync;
        public bool CanViewInvoiceListAsync
        {
            get { return canViewInvoiceListAsync; }
            set { canViewInvoiceListAsync = value; NotifyOfPropertyChange(() => CanViewInvoiceListAsync); }
        }

        private bool canViewTransactionListAsync;
        public bool CanViewTransactionListAsync
        {
            get { return canViewTransactionListAsync; }
            set { canViewTransactionListAsync = value; NotifyOfPropertyChange(() => CanViewTransactionListAsync); }
        }

        private bool canViewChequesAsync;
        public bool CanViewChequesAsync
        {
            get { return canViewChequesAsync; }
            set { canViewChequesAsync = value; NotifyOfPropertyChange(() => CanViewChequesAsync); }
        }

        private bool canViewCustomersAsync;
        public bool CanViewCustomersAsync
        {
            get { return canViewCustomersAsync; }
            set { canViewCustomersAsync = value; NotifyOfPropertyChange(() => CanViewCustomersAsync); }
        }

        private bool canViewProductsAsync;
        public bool CanViewProductsAsync
        {
            get { return canViewProductsAsync; }
            set { canViewProductsAsync = value; NotifyOfPropertyChange(() => CanViewProductsAsync); }
        }

        private bool canViewSettingsAsync;
        private LoggedInUser_DTO user;

        public bool CanViewSettingsAsync
        {
            get { return canViewSettingsAsync; }
            set { canViewSettingsAsync = value; NotifyOfPropertyChange(() => CanViewSettingsAsync); }
        }

        public bool ShowChequeNotification { get; set; }

        private async Task LoadKnowledgeOfTheDayAsync()
        {
            HttpClient httpClient = new();
            string knowlegeUri = @"https://one-api.ir/danestani/?token=649611:6321a450ea46f3.88748501";
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var kod = await httpClient.GetAsync(knowlegeUri);
            if (kod.IsSuccessStatusCode)
            {
                KnowledgeOfTheDay = await kod.Content.ReadAsAsync<KnowledgeModel>();
                if (KnowledgeOfTheDay.status == 200)
                {
                    if (KnowledgeOfTheDay.result.Content != null) KnowledgeOfTheDay.result.Content = KnowledgeOfTheDay.result.Content.Replace('\n', ' ');
                    KodAvailable = KnowledgeOfTheDay.status == 200 ? true : false;
                }
            }
            else KodAvailable = false;
        }

        public async Task AddNewInvoiceAsync()
        {
            var icm = SC.GetInstance<IInvoiceCollectionManager>();
            var ccm = SC.GetInstance<ICollectionManager<CustomerModel>>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new NewInvoiceViewModel(singleton, null, icm, ccm, null, User, SC);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task ViewInvoiceListAsync()
        {
            var icm = SC.GetInstance<IInvoiceCollectionManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new InvoiceListViewModel(icm, singleton, User, SC);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task AddNewTransactionAsync()
        {
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new NewTransactionViewModel(singleton, null, tcm, null, User, SC);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task ViewTransactionListAsync()
        {
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new TransactionListViewModel(tcm, singleton, User, SC);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task ViewChequesAsync()
        {
            var ccm = SC.GetInstance<IChequeCollectionManagerAsync>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new ChequeListViewModel(ccm, User, singleton);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task ViewCustomersAsync()
        {
            var ccm = SC.GetInstance<ICollectionManager<CustomerModel>>();
            WindowManager wm = new();
            var viewModel = new CustomerListViewModel(ccm, User);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task ViewProductsAsync()
        {
            var pcm = SC.GetInstance<ICollectionManager<ProductModel>>();
            WindowManager wm = new();
            var viewModel = new ProductListViewModel(pcm, User);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task TransactionShortcut1Async()
        {
            if (User.UserSettings.TransactionShortcut1Id == 0) return;
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
            if (await tcm.GetItemById(User.UserSettings.TransactionShortcut1Id) == null)
            {
                MessageBox.Show("فایل یافت نشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var tdm = SC.GetInstance<ITransactionDetailManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new TransactionDetailViewModel(tcm, tdm, User, singleton, User.UserSettings.TransactionShortcut1Id, null);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task TransactionShortcut2Async()
        {
            if (User.UserSettings.TransactionShortcut2Id == 0) return;
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
            if (await tcm.GetItemById(User.UserSettings.TransactionShortcut2Id) == null)
            {
                MessageBox.Show("فایل یافت نشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var tdm = SC.GetInstance<ITransactionDetailManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new TransactionDetailViewModel(tcm, tdm, User, singleton, User.UserSettings.TransactionShortcut2Id, null);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task TransactionShortcut3Async()
        {
            if (User.UserSettings.TransactionShortcut3Id == 0) return;
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
            if (await tcm.GetItemById(User.UserSettings.TransactionShortcut3Id) == null)
            {
                MessageBox.Show("فایل یافت نشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var tdm = SC.GetInstance<ITransactionDetailManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new TransactionDetailViewModel(tcm, tdm, User, singleton, User.UserSettings.TransactionShortcut3Id, null);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task ViewSettingsAsync()
        {
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new SettingsViewModel(SC, singleton, User);
            await wm.ShowWindowAsync(viewModel);
        }

        public void Window_Closed()
        {
            //Application.Current.Shutdown();
        }

        public async Task LogoutAsync()
        {
            ApiProcessor.Token = null;
            for (int i = Application.Current.Windows.Count - 1; i > 0; i--)
            {
                Application.Current.Windows[i].Close();
            }
            WindowManager wm = new();
            var viewModel = new LoginViewModel(SC, ApiProcessor);
            await wm.ShowWindowAsync(viewModel);
            (GetView() as Window).Close();
        }
    }

    public class KnowledgeModel
    {
        public int status { get; set; }
        public Result result { get; set; }
    }

    public class Result
    {
        public string Content { get; set; }
    }
}