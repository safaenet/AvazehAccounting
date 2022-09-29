using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.SecurityAndSettingsModels;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AvazehWpf.ViewModels
{
    public class LoginViewModel : ViewAware
    {
        public LoginViewModel(SimpleContainer sc, IApiProcessor apiProcessor)
        {
            SC = sc;
            ApiProcessor = apiProcessor;
            _ = GetIfAdminExistsAsync().ConfigureAwait(true);
        }

        private readonly SimpleContainer SC;
        private readonly IApiProcessor ApiProcessor;
        private string username;
        private LoggedInUser_DTO User;
        private bool canRegisterAsync;

        public bool CanRegisterAsync
        {
            get { return canRegisterAsync; }
            set { canRegisterAsync = value; NotifyOfPropertyChange(() => CanRegisterAsync); }
        }


        public string Password
        {
            get => ((GetView() as Window).FindName("Password") as PasswordBox).Password;
            set => ((GetView() as Window).FindName("Password") as PasswordBox).Password = value;
        }

        public string Username { get => username; set { username = value; NotifyOfPropertyChange(() => Username); } }

        public async Task GetIfAdminExistsAsync()
        {
            CanRegisterAsync = !(await ApiProcessor.GetBooleanAsync("Auth/AdminExists"));
        }

        public async Task LoginAsync()
        {
            UserLogin_DTO user = new()
            {
                Username = Username,
                Password = Password
            };

            User = await ApiProcessor.CreateItemAsync<UserLogin_DTO, LoggedInUser_DTO>("Auth/Login", user);
            if (User == null || string.IsNullOrEmpty(User.Token))
            {
                MessageBox.Show("نام کاربری یا رمز عبور اشتباه است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ApiProcessor.Token = User.Token;
            //var handler = new JwtSecurityTokenHandler();
            //var jwtSecurityToken = handler.ReadJwtToken(ApiProcessor.Token);
            
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
    }
}