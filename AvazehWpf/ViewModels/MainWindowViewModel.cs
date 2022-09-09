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

namespace AvazehWpf.ViewModels
{
    public class MainWindowViewModel : ViewAware
    {
        public MainWindowViewModel(IAppSettingsManager settingsManager, SimpleContainer sc)
        {
            ASM = settingsManager;
            SC = sc;
            LoadSettings().ConfigureAwait(true);
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


        public GeneralSettingsModel GeneralSettings { get => generalSettings; private set { generalSettings = value; NotifyOfPropertyChange(() => GeneralSettings); } }

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
            var ccm = SC.GetInstance<ICollectionManager<ChequeModel>>();
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
            if (GeneralSettings.TransactionShortcut1.TransactionId <= 0) return;
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
            var tdm = SC.GetInstance<ITransactionDetailManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new TransactionDetailViewModel(tcm, tdm, ASM, singleton, GeneralSettings.TransactionShortcut1.TransactionId, null);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task TransactionShortcut2()
        {
            if (GeneralSettings.TransactionShortcut2.TransactionId <= 0) return;
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
            var tdm = SC.GetInstance<ITransactionDetailManager>();
            var singleton = SC.GetInstance<SingletonClass>();
            WindowManager wm = new();
            var viewModel = new TransactionDetailViewModel(tcm, tdm, ASM, singleton, GeneralSettings.TransactionShortcut2.TransactionId, null);
            await wm.ShowWindowAsync(viewModel);
        }

        public async Task TransactionShortcut3()
        {
            if (GeneralSettings.TransactionShortcut3.TransactionId <= 0) return;
            var tcm = SC.GetInstance<ITransactionCollectionManager>();
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
    }
}