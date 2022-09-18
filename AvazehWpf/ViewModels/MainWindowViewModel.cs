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
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Diagnostics;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using SharedLibrary.SettingsModels;
using System.Globalization;
using System.Net.Http;

namespace AvazehWpf.ViewModels
{
    public class MainWindowViewModel : ViewAware
    {
        public MainWindowViewModel(IAppSettingsManager settingsManager, SimpleContainer sc)
        {
            ASM = settingsManager;
            SC = sc;
            LoadSettings().ConfigureAwait(true);
            LoadKnowledgeOfTheDayAsync().ConfigureAwait(true);
        }

        private readonly IAppSettingsManager ASM;
        private GeneralSettingsModel generalSettings;
        SimpleContainer SC;
        private bool settingsLoaded;

        public bool SettingsLoaded
        {
            get { return settingsLoaded; }
            set { settingsLoaded = value; NotifyOfPropertyChange(() => SettingsLoaded); }
        }

        private bool kodAvailable;
        private KnowledgeModel knowledgeOfTheDay;
        
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


        public GeneralSettingsModel GeneralSettings { get => generalSettings; private set { generalSettings = value; NotifyOfPropertyChange(() => GeneralSettings); } }
        public bool ShowChequeNotification { get; set; }

        private async Task LoadSettings()
        {
            var Settings = await ASM.LoadAllAppSettings();
            if (Settings == null) Settings = new();
            GeneralSettings = Settings.GeneralSettings;
            SettingsLoaded = true;
        }

        public async Task AddNewInvoice()
        {
            var icm = SC.GetInstance<IInvoiceCollectionManager>();
            var ccm = SC.GetInstance<ICollectionManager<CustomerModel>>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new NewInvoiceViewModel(singleton, null, icm, ccm, null, ASM, SC);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task ViewInvoiceList()
        {
            var icm = SC.GetInstance<IInvoiceCollectionManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new InvoiceListViewModel(icm, singleton, ASM, SC);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task AddNewTransaction()
        {
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new NewTransactionViewModel(singleton, null, tcm, null, ASM, SC);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task ViewTransactionList()
        {
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new TransactionListViewModel(tcm, singleton, ASM, SC);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task ViewCheques()
        {
            var ccm = SC.GetInstance<IChequeCollectionManagerAsync>();
            WindowManager wm = new();
            var viewModel = new ChequeListViewModel(ccm, ASM);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task ViewCustomers()
        {
            var ccm = SC.GetInstance<ICollectionManager<CustomerModel>>();
            WindowManager wm = new();
            var viewModel = new CustomerListViewModel(ccm);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task ViewProducts()
        {
            var pcm = SC.GetInstance<ICollectionManager<ProductModel>>();
            WindowManager wm = new();
            var viewModel = new ProductListViewModel(pcm);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task TransactionShortcut1()
        {
            if (GeneralSettings != null && GeneralSettings.TransactionShortcut1.TransactionId <= 0) return;
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
            if (await tcm.GetItemById(GeneralSettings.TransactionShortcut1.TransactionId) == null)
            {
                MessageBox.Show("فایل یافت نشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var tdm = SC.GetInstance<ITransactionDetailManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new TransactionDetailViewModel(tcm, tdm, ASM, singleton, GeneralSettings.TransactionShortcut1.TransactionId, null);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task TransactionShortcut2()
        {
            if (GeneralSettings != null && GeneralSettings.TransactionShortcut2.TransactionId <= 0) return;
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
            if (await tcm.GetItemById(GeneralSettings.TransactionShortcut2.TransactionId) == null)
            {
                MessageBox.Show("فایل یافت نشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var tdm = SC.GetInstance<ITransactionDetailManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new TransactionDetailViewModel(tcm, tdm, ASM, singleton, GeneralSettings.TransactionShortcut2.TransactionId, null);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task TransactionShortcut3()
        {
            if (GeneralSettings != null && GeneralSettings.TransactionShortcut3.TransactionId <= 0) return;
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
            if (await tcm.GetItemById(GeneralSettings.TransactionShortcut3.TransactionId) == null)
            {
                MessageBox.Show("فایل یافت نشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var tdm = SC.GetInstance<ITransactionDetailManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new TransactionDetailViewModel(tcm, tdm, ASM, singleton, GeneralSettings.TransactionShortcut3.TransactionId, null);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task ViewSettings()
        {
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new SettingsViewModel(singleton, ASM, LoadSettings);
            await wm.ShowWindowAsync(viewModel);
        }

        public void Window_Closed()
        {
            Application.Current.Shutdown();
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