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
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class SettingsViewModel : ViewAware
    {
        public SettingsViewModel(SingletonClass singleton, IAppSettingsManager settingsManager, Func<Task> callBack)
        {
            Singleton = singleton;
            SettingsManager = settingsManager;
            CallBackFunc = callBack;
            LoadAllSettings().ConfigureAwait(true);
        }

        private AppSettingsModel appSettings;
        IAppSettingsManager SettingsManager;
        private readonly SingletonClass Singleton;
        private readonly Func<Task> CallBackFunc;
        private ObservableCollection<ItemsForComboBox> transactionItemsForComboBox;
        private ItemsForComboBox selectedTransactionItem1;
        private ItemsForComboBox selectedTransactionItem2;
        private ItemsForComboBox selectedTransactionItem3;
        private UserDescriptionModel selectedUserDescriptionModel;
        private ObservableCollection<UserDescriptionModel> userDescriptions;
        private string verifyPassword;
        private bool settingsLoaded;

        public bool SettingsLoaded
        {
            get { return settingsLoaded; }
            set { settingsLoaded = value; NotifyOfPropertyChange(() => SettingsLoaded); }
        }


        public string VerifyPassword
        {
            get { return verifyPassword; }
            set { verifyPassword = value; NotifyOfPropertyChange(() => VerifyPassword); }
        }


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
            set { selectedTransactionItem1 = value; AppSettings.GeneralSettings.TransactionShortcut1.TransactionId = selectedTransactionItem1 == null ? 0 : selectedTransactionItem1.Id; NotifyOfPropertyChange(() => SelectedTransactionItem1); }
        }

        public ItemsForComboBox SelectedTransactionItem2
        {
            get => selectedTransactionItem2;
            set { selectedTransactionItem2 = value; AppSettings.GeneralSettings.TransactionShortcut2.TransactionId = selectedTransactionItem2 == null ? 0 : selectedTransactionItem2.Id; NotifyOfPropertyChange(() => SelectedTransactionItem2); }
        }

        public ItemsForComboBox SelectedTransactionItem3
        {
            get => selectedTransactionItem3;
            set { selectedTransactionItem3 = value; AppSettings.GeneralSettings.TransactionShortcut3.TransactionId = selectedTransactionItem3 == null ? 0 : selectedTransactionItem3.Id; NotifyOfPropertyChange(() => SelectedTransactionItem3); }
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
            await LoadTransactionNames();
            AppSettings = new();
            var s = await SettingsManager.LoadAllAppSettings();
            if (s != null) AppSettings = s;
            UserDescriptions = new(AppSettings.PrintSettings.UserDescriptions);
            if (AppSettings != null && AppSettings.GeneralSettings != null)
            {
                if (AppSettings.GeneralSettings.ShowTransactionShortcut1) SelectedTransactionItem1 = TransactionItemsForComboBox.Where(x => x.Id == AppSettings.GeneralSettings.TransactionShortcut1.TransactionId).SingleOrDefault();
                if (AppSettings.GeneralSettings.ShowTransactionShortcut2) SelectedTransactionItem2 = TransactionItemsForComboBox.Where(x => x.Id == AppSettings.GeneralSettings.TransactionShortcut2.TransactionId).SingleOrDefault();
                if (AppSettings.GeneralSettings.ShowTransactionShortcut3) SelectedTransactionItem3 = TransactionItemsForComboBox.Where(x => x.Id == AppSettings.GeneralSettings.TransactionShortcut3.TransactionId).SingleOrDefault();
            }
            SettingsLoaded = true;
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
        public void Window_PreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape) CloseWindow();
        }

        public async Task ClosingWindow()
        {
            if (CallBackFunc != null) await CallBackFunc?.Invoke();
        }

        public void SaveSettings()
        {
            if (AppSettings.GeneralSettings.RequireAuthentication)
            {
                var pass1 = (((GetView() as Window).FindName("Password1")) as PasswordBox).Password;
                var pass2 = (((GetView() as Window).FindName("Password2")) as PasswordBox).Password;
                if (pass1 != pass2)
                {
                    MessageBox.Show("رمز عبور و تایید آن برابر نیستند", "خطای رمز", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (pass1.Length < 4)
                {
                    MessageBox.Show("رمز عبور باید بزرگتر از 3 کاراکتر باشد", "خطای رمز", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else AppSettings.GeneralSettings.Password = pass1;
            }
            if (AppSettings.GeneralSettings.ShowTransactionShortcut1 && AppSettings.GeneralSettings.TransactionShortcut1.TransactionId <= 0) AppSettings.GeneralSettings.ShowTransactionShortcut1 = false;
            if (AppSettings.GeneralSettings.ShowTransactionShortcut2 && AppSettings.GeneralSettings.TransactionShortcut2.TransactionId <= 0) AppSettings.GeneralSettings.ShowTransactionShortcut2 = false;
            if (AppSettings.GeneralSettings.ShowTransactionShortcut3 && AppSettings.GeneralSettings.TransactionShortcut3.TransactionId <= 0) AppSettings.GeneralSettings.ShowTransactionShortcut3 = false;
            AppSettings.PrintSettings.UserDescriptions = UserDescriptions.ToList();
            SettingsManager.SaveAllAppSettings(AppSettings);
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }
    }
}