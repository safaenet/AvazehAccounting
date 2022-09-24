using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class ChequeListViewModel : Screen
    {
        public ChequeListViewModel(IChequeCollectionManagerAsync manager, IAppSettingsManager settingsManager, SingletonClass singelton)
        {
            CCM = manager;
            ASM = settingsManager;
            Singleton = singelton;
            _SelectedCheque = new();
            LoadSettings().ConfigureAwait(true);
        }

        private IChequeCollectionManagerAsync _CCM;
        private readonly IAppSettingsManager ASM;
        private ChequeModel _SelectedCheque;
        private ChequeSettingsModel chequeSettings;
        private GeneralSettingsModel generalSettings;
        private SingletonClass Singleton;

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

        public IChequeCollectionManagerAsync CCM
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
        public int SelectedListQueryStatus { get; set; } = 4;

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
            if (GeneralSettings!= null && !GeneralSettings.CanAddNewCheque) return;
            WindowManager wm = new();
            wm.ShowWindowAsync(new ChequeDetailViewModel(CCM, null, Singleton, RefreshPage));
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
            if (!GeneralSettings.CanViewCheques) return;
            ChequeListQueryStatus? ListQueryStatus = SelectedListQueryStatus >= Enum.GetNames(typeof(ChequeListQueryStatus)).Length ? null : (ChequeListQueryStatus)SelectedListQueryStatus;
            CCM.ListQueryStatus = ListQueryStatus;
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
            if (!GeneralSettings.CanEditCheques) return;
            if (Cheques == null || Cheques.Count == 0 || SelectedCheque == null || SelectedCheque.Id == 0) return;
            WindowManager wm = new();
            await wm.ShowWindowAsync(new ChequeDetailViewModel(CCM, SelectedCheque, Singleton, RefreshPage));
        }

        public async Task DeleteCheque()
        {
            if (!GeneralSettings.CanEditCheques) return;
            if (Cheques == null || Cheques.Count == 0 || SelectedCheque == null || SelectedCheque.Id == 0) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete cheque from {SelectedCheque.Drawer}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = await CCM.DeleteItemAsync(SelectedCheque.Id);
            if (output) Cheques.Remove(SelectedCheque);
            else MessageBox.Show($"Cheque with ID: {SelectedCheque.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Window_PreviewKeyDown(object window, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) (GetView() as Window).Close();
        }
    }

    public static class ChequeListQueryStatusItems //For ComboBoxes
    {
        public static Dictionary<int, string> GetChequeListQueryStatusItems()
        {
            Dictionary<int, string> choices = new();
            for (int i = 0; i < Enum.GetNames(typeof(ChequeListQueryStatus)).Length; i++)
            {
                if (Enum.GetName(typeof(ChequeListQueryStatus), i) == ChequeListQueryStatus.NotCashed.ToString())
                    choices.Add((int)ChequeListQueryStatus.NotCashed, "وصول نشده");
                else if (Enum.GetName(typeof(ChequeListQueryStatus), i) == ChequeListQueryStatus.Cashed.ToString())
                    choices.Add((int)ChequeListQueryStatus.Cashed, "وصول شده");
                else if (Enum.GetName(typeof(ChequeListQueryStatus), i) == ChequeListQueryStatus.Sold.ToString())
                    choices.Add((int)ChequeListQueryStatus.Sold, "منتقل شده");
                else if (Enum.GetName(typeof(ChequeListQueryStatus), i) == ChequeListQueryStatus.NonSufficientFund.ToString())
                    choices.Add((int)ChequeListQueryStatus.NonSufficientFund, "برگشت خورده");
                else if (Enum.GetName(typeof(ChequeListQueryStatus), i) == ChequeListQueryStatus.FromNowOn.ToString())
                    choices.Add((int)ChequeListQueryStatus.FromNowOn, "امروز به بعد");
            }
            choices.Add(Enum.GetNames(typeof(ChequeListQueryStatus)).Length, "همه");
            return choices;
        }
    }
}