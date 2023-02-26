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
using System.Windows.Controls;

namespace AvazehWpf.ViewModels;

public class SettingsViewModel : ViewAware
{
    public SettingsViewModel(SimpleContainer sc, SingletonClass singleton, LoggedInUser_DTO user, Func<Task> callback = null)
    {
        CallBackFunc = callback;
        Singleton = singleton;
        SC = sc;
        ASM = SC.GetInstance<IAppSettingsManager>();
        ApiProcessor = SC.GetInstance<IApiProcessor>();
        User = user;
        _ = LoadAllSettingsAsync().ConfigureAwait(true);
    }

    private readonly LoggedInUser_DTO User;
    private readonly IApiProcessor ApiProcessor;

    private bool transactionShortcut1Enabled;
    public bool TransactionShortcut1Enabled
    {
        get { return transactionShortcut1Enabled; }
        set { transactionShortcut1Enabled = value; NotifyOfPropertyChange(() => TransactionShortcut1Enabled); }
    }

    private bool transactionShortcut2Enabled;
    public bool TransactionShortcut2Enabled
    {
        get { return transactionShortcut2Enabled; }
        set { transactionShortcut2Enabled = value; NotifyOfPropertyChange(() => TransactionShortcut2Enabled); }
    }

    private bool transactionShortcut3Enabled;
    public bool TransactionShortcut3Enabled
    {
        get { return transactionShortcut3Enabled; }
        set { transactionShortcut3Enabled = value; NotifyOfPropertyChange(() => TransactionShortcut3Enabled); }
    }

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

    private UserInfoBaseModel selectedUserInfoBase;
    public UserInfoBaseModel SelectedUserInfoBase
    {
        get => selectedUserInfoBase;
        set
        {
            selectedUserInfoBase = value;
            NotifyOfPropertyChange(() => SelectedUserInfoBase);
        }
    }

    private UserPermissionsModel selectedUserPermissions;
    public UserPermissionsModel SelectedUserPermissions
    {
        get => selectedUserPermissions;
        set
        {
            selectedUserPermissions = value;
            NotifyOfPropertyChange(() => SelectedUserPermissions);
        }
    }

    private ObservableCollection<UserInfoBaseModel> userInfoBases;
    public ObservableCollection<UserInfoBaseModel> UserInfoBases
    {
        get => userInfoBases;
        set { userInfoBases = value; NotifyOfPropertyChange(() => UserInfoBases); }
    }

    private ObservableCollection<ProductUnitModel> productUnits;
    public ObservableCollection<ProductUnitModel> ProductUnits
    {
        get => productUnits;
        set { productUnits = value; NotifyOfPropertyChange(() => ProductUnits); }
    }


    private ProductUnitModel selectedProductUnit;
    public ProductUnitModel SelectedProductUnit
    {
        get { return selectedProductUnit; }
        set { selectedProductUnit = value; NotifyOfPropertyChange(() => SelectedProductUnit); }
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

    private bool settingsLoaded;
    public bool SettingsLoaded
    {
        get { return settingsLoaded; }
        set { settingsLoaded = value; NotifyOfPropertyChange(() => SettingsLoaded); }
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
        }
    }

    private bool canManageItself;
    public bool CanManageItself
    {
        get { return canManageItself; }
        set { canManageItself = value; NotifyOfPropertyChange(() => CanManageItself); }
    }

    private bool canManageOthers;
    public bool CanManageOthers
    {
        get { return canManageOthers; }
        set { canManageOthers = value; NotifyOfPropertyChange(() => CanManageOthers); }
    }

    public bool CanManageEither => CanManageItself || CanManageOthers;

    public string Password1
    {
        get => ((GetView() as Window).FindName("Password1") as PasswordBox).Password;
        set => ((GetView() as Window).FindName("Password1") as PasswordBox).Password = value;
    }

    public string Password2
    {
        get => ((GetView() as Window).FindName("Password2") as PasswordBox).Password;
        set => ((GetView() as Window).FindName("Password2") as PasswordBox).Password = value;
    }

    private async Task LoadTransactionNamesAsync()
    {
        TransactionItemsForComboBox = await Singleton.ReloadTransactionNames();
    }

    private async Task LoadAllSettingsAsync()
    {
        CanManageItself = ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanManageItself));
        CanManageOthers = ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanManageOthers));

        await LoadTransactionNamesAsync();
        UserInfoBases = await Singleton.ReloadUserInfoBases();
        if (UserInfoBases != null && UserInfoBases.Count > 0) SelectedUserInfoBase = UserInfoBases.SingleOrDefault(user => user.Id == User.Id);
        await LoadUserPermissionsAsync();
        UserSettings = User.UserSettings.Clone();
        PrintSettings = User.PrintSettings.Clone();
        GeneralSettings = User.GeneralSettings.Clone();
        if (PrintSettings.UserDescriptions != null) UserDescriptions = new(PrintSettings.UserDescriptions); else UserDescriptions = new();
        if (GeneralSettings.ProductUnits != null)
        {
            ProductUnits = new(GeneralSettings.ProductUnits);
            SelectedProductUnit = ProductUnits.FirstOrDefault();
        }
        else ProductUnits = new();
        if (UserSettings != null)
        {
            if (UserSettings.TransactionShortcut1Id > 0)
            {
                SelectedTransactionItem1 = TransactionItemsForComboBox.SingleOrDefault(x => x.Id == UserSettings.TransactionShortcut1Id);
                TransactionShortcut1Enabled = true;
            }
            if (UserSettings.TransactionShortcut2Id > 0)
            {
                SelectedTransactionItem2 = TransactionItemsForComboBox.SingleOrDefault(x => x.Id == UserSettings.TransactionShortcut2Id);
                TransactionShortcut2Enabled = true;
            }
            if (UserSettings.TransactionShortcut3Id > 0)
            {
                SelectedTransactionItem3 = TransactionItemsForComboBox.SingleOrDefault(x => x.Id == UserSettings.TransactionShortcut3Id);
                TransactionShortcut3Enabled = true;
            }
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
        //User.PrintSettings.UserDescriptions = UserDescriptions.ToList();
        //User.GeneralSettings.ProductUnits = ProductUnits.ToList();
        if (CanManageOthers)
        {
            AppSettingsModel appSettings = new();
            appSettings.GeneralSettings = GeneralSettings;
            appSettings.PrintSettings = PrintSettings;
            appSettings.GeneralSettings.ProductUnits = ProductUnits.ToList();
            appSettings.PrintSettings.UserDescriptions = UserDescriptions.ToList();
            await ASM.SaveAllAppSettings(appSettings);
            //ProductUnits = await Singleton.ReloadProductUnits();
            User.PrintSettings = PrintSettings.Clone();
            User.GeneralSettings = GeneralSettings.Clone();
        }
        if (CanManageItself)
        {
            if (TransactionShortcut1Enabled && SelectedTransactionItem1 != null) UserSettings.TransactionShortcut1Id = SelectedTransactionItem1.Id; else UserSettings.TransactionShortcut1Id = 0;
            if (TransactionShortcut2Enabled && SelectedTransactionItem2 != null) UserSettings.TransactionShortcut2Id = SelectedTransactionItem2.Id; else UserSettings.TransactionShortcut2Id = 0;
            if (TransactionShortcut3Enabled && SelectedTransactionItem3 != null) UserSettings.TransactionShortcut3Id = SelectedTransactionItem3.Id; else UserSettings.TransactionShortcut3Id = 0;

            var result = await ApiProcessor.UpdateItemAsync<UserSettingsModel, UserSettingsModel>("Auth/UpdateUserSettings", User.Id, UserSettings);
            if (result == null) MessageBox.Show("خطا هنگام ذخیره تنظیمات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            else User.UserSettings = UserSettings.Clone();
        }
    }

    public async Task SaveUserChangesAsync()
    {
        if (!CanManageOthers && !(CanManageItself && SelectedUserInfoBase.Id == User.Id))
        {
            MessageBox.Show("اجازه انجام تغییرات را ندارید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        var IsNew = SelectedUserInfoBase.Id == -1;
        User_DTO_CreateUpdate user = new()
        {
            Username = SelectedUserInfoBase.Username,
            FirstName = SelectedUserInfoBase.FirstName,
            LastName = SelectedUserInfoBase.LastName,
            IsActive = SelectedUserInfoBase.IsActive,
            Permissions = SelectedUserPermissions
        };
        if (!string.IsNullOrEmpty(Password1))
        {
            if (Password1.Length < 4)
            {
                MessageBox.Show("رمز باید حداقل 4 کاراکتر باشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Password1 != Password2)
            {
                MessageBox.Show("رمز جدید با تایید آن مطابقت ندارد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            user.Password = Password1;
        }
        else if (IsNew)
        {
            MessageBox.Show("کاربر جدید باید رمز داشته باشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        else user.Password = null;
        UserInfoBaseModel result;
        if (IsNew)
        {
            user.Settings = new();
            result = await ApiProcessor.CreateItemAsync<User_DTO_CreateUpdate, UserInfoBaseModel>("Auth/RegisterNew", user);
            if (result != null) SelectedUserInfoBase.Id = result.Id;
        }
        else result = await ApiProcessor.UpdateItemAsync<User_DTO_CreateUpdate, UserInfoBaseModel>("Auth/UpdateUser", SelectedUserInfoBase.Id, user);
        if (result == null) MessageBox.Show("خطا هنگام ذخیره تنظیمات کاربر", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public async Task DeleteUserAsync()
    {
        var msg = MessageBox.Show("آیا مطمئنید؟ با زدن دکمه 'بله' کاربر حذف خواهد شد", "حذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        var result = await ApiProcessor.DeleteItemAsync("Auth/DeleteUser", SelectedUserInfoBase.Id);
        if (!result)
        {
            MessageBox.Show("خطا هنگام حذف کاربر", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        var delete = MessageBox.Show("آیا مطمئنید؟ با زدن دکمه 'بله' کاربر حذف خواهد شد", "حذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (delete == MessageBoxResult.Yes) UserInfoBases.Remove(SelectedUserInfoBase);
    }

    public async Task LoadUserPermissionsAsync()
    {
        if (SelectedUserInfoBase == null) return;
        if (SelectedUserInfoBase.Id == -1)
            SelectedUserPermissions = new();
        else
        {
            var perms = await ApiProcessor.GetItemAsync<UserPermissionsModel>("Auth/UserPermissions", SelectedUserInfoBase.Id.ToString());
            SelectedUserPermissions = perms;
        }
    }

    public void CreateNewUser()
    {
        var newUsers = UserInfoBases.Where(user => user.Id == -1);
        if (newUsers != null && newUsers.Any())
        {
            SelectedUserInfoBase = newUsers.FirstOrDefault();
            return;
        }
        UserInfoBaseModel newUser = new();
        newUser.Id = -1;
        UserInfoBases.Add(newUser);
        SelectedUserInfoBase = newUser;
    }

    public void AddNewProductUnit()
    {
        ProductUnitModel newUnit = new();
        newUnit.Id = -1;
        ProductUnits.Add(newUnit);
        SelectedProductUnit = newUnit;
    }

    public void DeleteSelectedProductUnit()
    {
        MessageBox.Show("اگر کالایی با این واحد کالا در فاکتوری درج شده باشد حذف این واحد کالا غیرممکن است. لذا ابتدا باید  تمام آن کالاهای درج شده را اصلاح کنید و سپس اقدام به حذف این واحد کنید", "اطلاعات", MessageBoxButton.OK);
        ProductUnits.Remove(SelectedProductUnit);
        SelectedProductUnit = null;
    }

    public void CloseWindow()
    {
        (GetView() as Window).Close();
    }
}