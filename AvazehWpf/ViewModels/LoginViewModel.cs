using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.SecurityAndSettingsModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AvazehWpf.ViewModels;

public class LoginViewModel : ViewAware
{
    public LoginViewModel(SimpleContainer sc, IApiProcessor apiProcessor)
    {
        CanLoginAsync = true;
        StatusText = "در حال ارتباط با سرور...";
        SC = sc;
        ApiProcessor = apiProcessor;
        _ = TestDBConnectionAsync().ConfigureAwait(true);
        _ = GetIfAdminExistsAsync().ConfigureAwait(true);
        LoadSettings();
    }

    private readonly SimpleContainer SC;
    private readonly IApiProcessor ApiProcessor;
    private string username;
    private LoggedInUser_DTO User;
    private bool canRegisterAsync;
    private bool rememberUsername;

    public bool RememberUsername
    {
        get { return rememberUsername; }
        set { rememberUsername = value; NotifyOfPropertyChange(() => RememberUsername); }
    }


    public bool CanRegisterAsync
    {
        get { return canRegisterAsync; }
        set { canRegisterAsync = value; NotifyOfPropertyChange(() => CanRegisterAsync); }
    }
    private bool canLoginAsync;

    public bool CanLoginAsync
    {
        get { return canLoginAsync; }
        set { canLoginAsync = value; NotifyOfPropertyChange(() => CanLoginAsync); }
    }

    private string statusText;

    public string StatusText
    {
        get { return statusText; }
        set { statusText = value; NotifyOfPropertyChange(() => StatusText); }
    }

    public string Password
    {
        get => ((GetView() as Window).FindName("txtPassword") as PasswordBox).Password;
        set => ((GetView() as Window).FindName("txtPassword") as PasswordBox).Password = value;
    }

    public string Username { get => username; set { username = value; NotifyOfPropertyChange(() => Username); } }
    public UIElement FocusedElement { get; set; }

    public void LoadSettings()
    {
        var localsettings = LocalSettingsManager.LoadAllSettings();
        RememberUsername = localsettings.RememberUsername;
        if (RememberUsername)
        {
            Username = localsettings.LastLoggedUsername;
            //var window = ((GetView() as Window).FindName("txtPassword") as PasswordBox).Focus();
        }
        //else ((GetView() as Window).FindName("txtUsername") as TextBox).Focus();
    }

    public async Task GetIfAdminExistsAsync()
    {
        //CanRegisterAsync = true;
        CanRegisterAsync = !(await ApiProcessor.GetBooleanAsync("Auth/AdminExists"));
    }

    public async Task LoginAsync()
    {
        CanLoginAsync = false;
        StatusText = "در حال ورود. لطفا شکیبا باشید";
        UserLogin_DTO user = new()
        {
            Username = Username,
            Password = Password
        };

        User = await ApiProcessor.CreateItemAsync<UserLogin_DTO, LoggedInUser_DTO>("Auth/Login", user);
        if (User == null || string.IsNullOrEmpty(User.Token))
        {
            CanLoginAsync = true;
            StatusText = "ورود موفقیت آمیز نبود";
            MessageBox.Show("نام کاربری یا رمز عبور اشتباه است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        ApiProcessor.Token = User.Token;
        var localSettings = LocalSettingsManager.LoadAllSettings();
        if (RememberUsername)
        {
            localSettings.RememberUsername = true;
            localSettings.LastLoggedUsername = Username;
        }
        else
        {
            localSettings.RememberUsername = false;
            localSettings.LastLoggedUsername = null;
        }
        _ = LocalSettingsManager.SaveAllSettings(localSettings);

        WindowManager wm = new();
        var viewModel = new MainWindowViewModel(User, SC);
        await wm.ShowWindowAsync(viewModel);
        (GetView() as Window).Close();
    }

    public async Task RegisterAsync()
    {
        WindowManager wm = new();
        var viewModel = new RegisterViewModel(SC, ApiProcessor);
        await wm.ShowWindowAsync(viewModel);
        (GetView() as Window).Close();
    }

    public void FocusOnTextBox()
    {
        var txt = ((GetView() as Window).FindName("txtUsername") as TextBox).Text;
        if (string.IsNullOrEmpty(txt)) ((GetView() as Window).FindName("txtUsername") as TextBox).Focus();
        else ((GetView() as Window).FindName("txtPassword") as PasswordBox).Focus();
    }

    public async Task TestDBConnectionAsync()
    {
        if (await ApiProcessor.TestDBConnectionAsync())
        {
            StatusText = "ارتباط با سرور برقرار شد. لطفا وارد شوید";
        }
        else StatusText = "ارتباط با سرور برقرار نشد. لطفا تنظیمات را چک کنید";
    }
}