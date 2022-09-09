using Caliburn.Micro;
using SharedLibrary.DtoModels;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using AvazehApiClient.DataAccess;
using Xceed.Wpf.Toolkit;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using System.Windows;
using System.Windows.Controls;

namespace AvazehWpf.ViewModels
{
    public class SettingsViewModel : ViewAware
    {
        public SettingsViewModel(SingletonClass singleton, IAppSettingsManager settingsManager)
        {
            Singleton = singleton;
            SettingsManager = settingsManager;
            LoadTransactionNames().ConfigureAwait(true);
            LoadAllSettings().ConfigureAwait(true);
        }

        private AppSettingsModel appSettings;
        IAppSettingsManager SettingsManager;
        SingletonClass Singleton;
        private ObservableCollection<ItemsForComboBox> transactionItemsForComboBox;
        private ItemsForComboBox selectedTransactionItem1;
        private ItemsForComboBox selectedTransactionItem2;
        private ItemsForComboBox selectedTransactionItem3;
        private UserDescriptionModel selectedUserDescriptionModel;
        private ObservableCollection<UserDescriptionModel> userDescriptions;

        public ObservableCollection<UserDescriptionModel> UserDescriptions
        {
            get { return userDescriptions; }
            set { userDescriptions = value; NotifyOfPropertyChange(() => UserDescriptions); }
        }


        public AppSettingsModel AppSettings
        {
            get { return appSettings; }
            set { appSettings = value; NotifyOfPropertyChange(() => AppSettings); }
        }

        public ObservableCollection<ItemsForComboBox> TransactionItemsForComboBox { get => transactionItemsForComboBox; set { transactionItemsForComboBox = value; NotifyOfPropertyChange(() => TransactionItemsForComboBox); } }

        public ItemsForComboBox SelectedTransactionItem1
        {
            get => selectedTransactionItem1;
            set { selectedTransactionItem1 = value; AppSettings.GeneralSettings.TransactionShortcut1.TransactionId = selectedTransactionItem1.Id; NotifyOfPropertyChange(() => SelectedTransactionItem1); }
        }

        public ItemsForComboBox SelectedTransactionItem2
        {
            get => selectedTransactionItem2;
            set { selectedTransactionItem2 = value; AppSettings.GeneralSettings.TransactionShortcut2.TransactionId = selectedTransactionItem2.Id; NotifyOfPropertyChange(() => SelectedTransactionItem2); }
        }

        public ItemsForComboBox SelectedTransactionItem3
        {
            get => selectedTransactionItem3;
            set { selectedTransactionItem3 = value; AppSettings.GeneralSettings.TransactionShortcut3.TransactionId = selectedTransactionItem3.Id; NotifyOfPropertyChange(() => SelectedTransactionItem3); }
        }

        public UserDescriptionModel SelectedUserDescriptionModel
        {
            get => selectedUserDescriptionModel;
            set
            {
                selectedUserDescriptionModel = value;
                NotifyOfPropertyChange(() => SelectedUserDescriptionModel);
                //((GetView() as Window).FindName("cmbUserDescriptions") as ComboBox).
            }
        }

        private async Task LoadTransactionNames()
        {
            TransactionItemsForComboBox = await Singleton.ReloadTransactionNames();
        }

        private async Task LoadAllSettings()
        {
            AppSettings = new();
            var s = await SettingsManager.LoadAllAppSettings();
            if (s != null) AppSettings = s;
            UserDescriptions = new(AppSettings.PrintSettings.UserDescriptions);
        }

        public void AddNewUserDescription()
        {
            UserDescriptionModel model = new();
            model.DescriptionTitle = "توضیحات جدید";
            UserDescriptions.Add(model);
            SelectedUserDescriptionModel = model;
            NotifyOfPropertyChange(() => SelectedUserDescriptionModel);
            NotifyOfPropertyChange(() => AppSettings);
        }

        public void DeleteUserDescription()
        {
            UserDescriptions.Remove(SelectedUserDescriptionModel);
            NotifyOfPropertyChange(() => SelectedUserDescriptionModel);
            NotifyOfPropertyChange(() => AppSettings);
        }

        public void SaveSettings()
        {
            AppSettings.PrintSettings.UserDescriptions = UserDescriptions.ToList();
            SettingsManager.SaveAllAppSettings(AppSettings);
        }

        public void CloseWindow()
        {

        }
    }
}