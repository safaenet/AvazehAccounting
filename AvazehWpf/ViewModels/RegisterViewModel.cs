using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.SecurityAndSettingsModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AvazehWpf.ViewModels;

public class RegisterViewModel : ViewAware
{
    public RegisterViewModel(SimpleContainer sc, IApiProcessor apiProcessor)
    {
        SC = sc;
        ApiProcessor = apiProcessor;
    }

    private readonly SimpleContainer SC;
    private readonly IApiProcessor ApiProcessor;

    private string username;

    public string Username
    {
        get { return username; }
        set { username = value; }
    }

    public string Password
    {
        get => ((GetView() as Window).FindName("Password1") as PasswordBox).Password;
        set => ((GetView() as Window).FindName("Password1") as PasswordBox).Password = value;
    }

    public string VerifyPassword
    {
        get => ((GetView() as Window).FindName("Password2") as PasswordBox).Password;
        set => ((GetView() as Window).FindName("Password2") as PasswordBox).Password = value;
    }

    private string firstName;

    public string FirstName
    {
        get { return firstName; }
        set { firstName = value; }
    }

    private string lastName;

    public string LastName
    {
        get { return lastName; }
        set { lastName = value; }
    }

    public async Task RegisterAsync()
    {
        if (Password != VerifyPassword)
        {
            MessageBox.Show("رمز عبور با تایید آن مطابقت ندارد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        User_DTO_CreateUpdate newUser = new()
        {
            Username = Username,
            Password = Password,
            FirstName = FirstName,
            LastName = LastName,
            IsActive = true
        };
        newUser.Permissions = new();
        newUser.Settings = new();
        var result = await ApiProcessor.CreateItemAsync<User_DTO_CreateUpdate, UserInfoBaseModel>("Auth/Register", newUser);
        if (result == null)
        {
            MessageBox.Show("خطا در ایجاد کاربر جدید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        else
        {
            WindowManager wm = new();
            var viewModel = new LoginViewModel(SC, ApiProcessor);
            await wm.ShowWindowAsync(viewModel);
            (GetView() as Window).Close();
        }
    }
}