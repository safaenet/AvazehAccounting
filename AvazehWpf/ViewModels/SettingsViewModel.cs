using Caliburn.Micro;
using SharedLibrary.DtoModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SharedLibrary.SecurityAndSettingsModels;

namespace AvazehWpf.ViewModels
{
    public class SettingsViewModel : ViewAware
    {
        public SettingsViewModel(SingletonClass singleton, LoggedInUser_DTO user)
        {
            Singleton = singleton;
            User = user;
            _ = LoadAllSettingsAsync().ConfigureAwait(true);
        }

        private LoggedInUser_DTO User;
        private UserSettingsModel userSettings;

        public UserSettingsModel UserSettings
        {
            get { return userSettings; }
            set { userSettings = value; NotifyOfPropertyChange(() => UserSettings); }
        }

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

        public ObservableCollection<ItemsForComboBox> TransactionItemsForComboBox { get => transactionItemsForComboBox; set { transactionItemsForComboBox = value; NotifyOfPropertyChange(() => TransactionItemsForComboBox); } }

        public ItemsForComboBox SelectedTransactionItem1
        {
            get => selectedTransactionItem1;
            set { selectedTransactionItem1 = value; User.UserSettings.TransactionShortcut1Id = selectedTransactionItem1 == null ? 0 : selectedTransactionItem1.Id; NotifyOfPropertyChange(() => SelectedTransactionItem1); }
        }

        public ItemsForComboBox SelectedTransactionItem2
        {
            get => selectedTransactionItem2;
            set { selectedTransactionItem2 = value; User.UserSettings.TransactionShortcut2Id = selectedTransactionItem2 == null ? 0 : selectedTransactionItem2.Id; NotifyOfPropertyChange(() => SelectedTransactionItem2); }
        }

        public ItemsForComboBox SelectedTransactionItem3
        {
            get => selectedTransactionItem3;
            set { selectedTransactionItem3 = value; User.UserSettings.TransactionShortcut3Id = selectedTransactionItem3 == null ? 0 : selectedTransactionItem3.Id; NotifyOfPropertyChange(() => SelectedTransactionItem3); }
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

        private async Task LoadTransactionNamesAsync()
        {
            TransactionItemsForComboBox = await Singleton.ReloadTransactionNames();
        }

        private async Task LoadAllSettingsAsync()
        {
            await LoadTransactionNamesAsync();
            AppSettings = new();
            var s = await User.LoadAllAppSettings();
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
        }

        public void DeleteUserDescription()
        {
            UserDescriptions.Remove(SelectedUserDescriptionModel);
            NotifyOfPropertyChange(() => SelectedUserDescriptionModel);
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
            //if (User.GeneralSettings.RequireAuthentication)
            //{
            //    var pass1 = (((GetView() as Window).FindName("Password1")) as PasswordBox).Password;
            //    var pass2 = (((GetView() as Window).FindName("Password2")) as PasswordBox).Password;
            //    if (pass1 != pass2)
            //    {
            //        MessageBox.Show("رمز عبور و تایید آن برابر نیستند", "خطای رمز", MessageBoxButton.OK, MessageBoxImage.Error);
            //        return;
            //    }
            //    else if (pass1.Length < 4)
            //    {
            //        MessageBox.Show("رمز عبور باید بزرگتر از 3 کاراکتر باشد", "خطای رمز", MessageBoxButton.OK, MessageBoxImage.Error);
            //        return;
            //    }
            //    else AppSettings.GeneralSettings.Password = pass1;
            //}
            User.PrintSettings.UserDescriptions = UserDescriptions.ToList();
            ASM.SaveAllAppSettings(AppSettings);
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }
    }
}