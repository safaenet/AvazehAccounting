using Caliburn.Micro;
using SharedLibrary.DtoModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvazehApiClient.DataAccess;
using SharedLibrary.DalModels;
using System.Windows;
using System.Windows.Input;
using SharedLibrary.SecurityAndSettingsModels;
using AvazehApiClient.DataAccess.Interfaces;

namespace AvazehWpf.ViewModels
{
    public class SettingsViewModel : ViewAware
    {
        public SettingsViewModel(SimpleContainer sc, SingletonClass singleton, LoggedInUser_DTO user)
        {
            Singleton = singleton;
            SC = sc;
            ASM = SC.GetInstance<IAppSettingsManager>();
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

        private PrintSettingsModel printSettings;

        public PrintSettingsModel PrintSettings
        {
            get { return printSettings; }
            set { printSettings = value; NotifyOfPropertyChange(() => PrintSettings); }
        }

        private GeneralSettingsModel generalSettings;

        public GeneralSettingsModel GeneralSettings
        {
            get { return generalSettings; }
            set { generalSettings = value; NotifyOfPropertyChange(() => GeneralSettings); }
        }

        public UserPermissionsModel UserPermissions
        {
            get => userPermissions;
            set
            {
                userPermissions = value;
                NotifyOfPropertyChange(() => UserPermissions);
            }
        }

        private readonly SingletonClass Singleton;
        private readonly SimpleContainer SC;
        private readonly Func<Task> CallBackFunc;
        private readonly IAppSettingsManager ASM;
        private ObservableCollection<ItemsForComboBox> transactionItemsForComboBox;
        private ItemsForComboBox selectedTransactionItem1;
        private ItemsForComboBox selectedTransactionItem2;
        private ItemsForComboBox selectedTransactionItem3;
        private UserDescriptionModel selectedUserDescriptionModel;
        private ObservableCollection<UserDescriptionModel> userDescriptions;
        private string verifyPassword;
        private bool settingsLoaded;
        private UserPermissionsModel userPermissions;

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

            UserSettings = User.UserSettings.Clone();
            PrintSettings = User.PrintSettings.Clone();
            GeneralSettings = User.GeneralSettings.Clone();
            if (PrintSettings.UserDescriptions != null) UserDescriptions = new(PrintSettings.UserDescriptions); else UserDescriptions = new();
            if (UserSettings != null)
            {
                if (UserSettings.TransactionShortcut1Id > 0) SelectedTransactionItem1 = TransactionItemsForComboBox.Where(x => x.Id == UserSettings.TransactionShortcut1Id).SingleOrDefault();
                if (UserSettings.TransactionShortcut2Id > 0) SelectedTransactionItem2 = TransactionItemsForComboBox.Where(x => x.Id == UserSettings.TransactionShortcut2Id).SingleOrDefault();
                if (UserSettings.TransactionShortcut3Id > 0) SelectedTransactionItem3 = TransactionItemsForComboBox.Where(x => x.Id == UserSettings.TransactionShortcut3Id).SingleOrDefault();
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

        public async Task ClosingWindowAsync()
        {
            if (CallBackFunc != null) await CallBackFunc?.Invoke();
        }

        public async Task SaveSettingsAsync()
        {
            User.PrintSettings.UserDescriptions = UserDescriptions.ToList();
            AppSettingsModel appSettings = new();
            appSettings.GeneralSettings = GeneralSettings;
            appSettings.PrintSettings = PrintSettings;
            await ASM.SaveAllAppSettings(appSettings);
            User.GeneralSettings = GeneralSettings.Clone();
            User.PrintSettings = PrintSettings.Clone();
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }
    }
}