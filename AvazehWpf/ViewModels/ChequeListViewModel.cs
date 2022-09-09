using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class ChequeListViewModel : Screen
    {
        public ChequeListViewModel(ICollectionManager<ChequeModel> manager, IAppSettingsManager settingsManager)
        {
            CCM = manager;
            ASM = settingsManager;
            _SelectedCheque = new();
            LoadSettings().ConfigureAwait(true);
        }

        private ICollectionManager<ChequeModel> _CCM;
        private readonly IAppSettingsManager ASM;
        private ChequeModel _SelectedCheque;
        private ChequeSettingsModel chequeSettings;
        private GeneralSettingsModel generalSettings;

        public ChequeModel SelectedCheque
        {
            get { return _SelectedCheque; }
            set { _SelectedCheque = value; NotifyOfPropertyChange(() => SelectedCheque); }
        }
        public GeneralSettingsModel GeneralSettings
        {
            get => generalSettings; set
            {
                generalSettings = value;
                NotifyOfPropertyChange(() => GeneralSettings);
            }
        }
        public ChequeSettingsModel ChequeSettings
        {
            get => chequeSettings; set
            {
                chequeSettings = value;
                NotifyOfPropertyChange(() => ChequeSettings);
            }
        }

        public ICollectionManager<ChequeModel> CCM
        {
            get { return _CCM; }
            set
            {
                _CCM = value;
                NotifyOfPropertyChange(() => CCM);
                NotifyOfPropertyChange(() => Cheques);
            }
        }

        public ObservableCollection<ChequeModel> Cheques
        {
            get => CCM.Items;
            set
            {
                CCM.Items = value;
                NotifyOfPropertyChange(() => CCM);
                NotifyOfPropertyChange(() => Cheques);
            }
        }

        public string SearchText { get; set; }
        private async Task LoadSettings()
        {
            var Settings = await ASM.LoadAllAppSettings();
            if (Settings == null) Settings = new();
            ChequeSettings = Settings.ChequeSettings;
            GeneralSettings = Settings.GeneralSettings;

            CCM.PageSize = ChequeSettings.PageSize;
            CCM.QueryOrderType = ChequeSettings.QueryOrderType;
            await Search();
        }

        public void AddNewCheque()
        {
            WindowManager wm = new();
            wm.ShowWindowAsync(new ChequeDetailViewModel(CCM, null, RefreshPage));
        }

        public async Task PreviousPage()
        {
            await CCM.LoadPreviousPageAsync();
            NotifyOfPropertyChange(() => Cheques);
        }

        public async Task NextPage()
        {
            await CCM.LoadNextPageAsync();
            NotifyOfPropertyChange(() => Cheques);
        }

        public async Task RefreshPage()
        {
            await CCM.RefreshPage();
            NotifyOfPropertyChange(() => Cheques);
        }

        public async Task Search()
        {
            CCM.SearchValue = SearchText;
            await CCM.LoadFirstPageAsync();
            NotifyOfPropertyChange(() => Cheques);
        }

        public async Task SearchBoxKeyDownHandler(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;
            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                await Search();
            }
        }

        public async Task EditCheque()
        {
            if (Cheques == null || Cheques.Count == 0 || SelectedCheque == null) return;
            WindowManager wm = new();
            await wm.ShowWindowAsync(new ChequeDetailViewModel(CCM, SelectedCheque, RefreshPage));
        }

        public async Task DeleteCheque()
        {
            if (Cheques == null || Cheques.Count == 0 || SelectedCheque == null || SelectedCheque.Id == 0) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete cheque from {SelectedCheque.Drawer}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = await CCM.DeleteItemAsync(SelectedCheque.Id);
            if (output) Cheques.Remove(SelectedCheque);
            else MessageBox.Show($"Cheque with ID: {SelectedCheque.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}