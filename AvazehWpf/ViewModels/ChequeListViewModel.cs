using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using SharedLibrary.SecurityAndSettingsModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels;

public class ChequeListViewModel : Screen
{
    public ChequeListViewModel(IChequeCollectionManagerAsync manager, LoggedInUser_DTO user, SingletonClass singelton)
    {
        CCM = manager;
        User = user;
        CurrentPersianDate = new PersianCalendar().GetPersianDate();
        Singleton = singelton;
        _SelectedCheque = new();
        _ = LoadSettingsAsync().ConfigureAwait(true);
    }

    private IChequeCollectionManagerAsync _CCM;
    private LoggedInUser_DTO user;
    private ChequeModel _SelectedCheque;
    private readonly SingletonClass Singleton;
    public string CurrentPersianDate { get; init; }
    public LoggedInUser_DTO User { get => user; init => user = value; }

    public ChequeModel SelectedCheque
    {
        get { return _SelectedCheque; }
        set { _SelectedCheque = value; NotifyOfPropertyChange(() => SelectedCheque); }
    }

    private bool canEditChequeAsync;
    public bool CanEditChequeAsync
    {
        get { return canEditChequeAsync; }
        set { canEditChequeAsync = value; NotifyOfPropertyChange(() => CanEditChequeAsync); }
    }

    private bool canAddNewChequeAsync;
    public bool CanAddNewChequeAsync
    {
        get { return canAddNewChequeAsync; }
        set { canAddNewChequeAsync = value; NotifyOfPropertyChange(() => CanAddNewChequeAsync); }
    }

    private bool canDeleteChequeAsync;
    public bool CanDeleteChequeAsync
    {
        get { return canDeleteChequeAsync; }
        set { canDeleteChequeAsync = value; NotifyOfPropertyChange(() => CanDeleteChequeAsync); }
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

    private async Task LoadSettingsAsync()
    {
        CanAddNewChequeAsync = CCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanAddNewCheque));
        CanEditChequeAsync = CCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanViewChequeDetails));
        CanDeleteChequeAsync = CCM.ApiProcessor.IsInRole(nameof(UserPermissionsModel.CanDeleteCheque));

        CCM.PageSize = User.UserSettings.ChequeListPageSize;
        CCM.QueryOrderType = User.UserSettings.ChequeListQueryOrderType;
        await SearchAsync();
    }

    public async Task AddNewChequeAsync()
    {
        WindowManager wm = new();
        await wm.ShowWindowAsync(new ChequeDetailViewModel(CCM, null, Singleton, RefreshPageAsync));
    }

    public async Task PreviousPageAsync()
    {
        await CCM.LoadPreviousPageAsync();
        NotifyOfPropertyChange(() => Cheques);
    }

    public async Task NextPageAsync()
    {
        await CCM.LoadNextPageAsync();
        NotifyOfPropertyChange(() => Cheques);
    }

    public async Task RefreshPageAsync()
    {
        await CCM.RefreshPage();
        NotifyOfPropertyChange(() => Cheques);
    }

    public async Task SearchAsync()
    {
        ChequeListQueryStatus? ListQueryStatus = SelectedListQueryStatus >= Enum.GetNames(typeof(ChequeListQueryStatus)).Length ? null : (ChequeListQueryStatus)SelectedListQueryStatus;
        CCM.ListQueryStatus = ListQueryStatus;
        CCM.SearchValue = SearchText;
        await CCM.LoadFirstPageAsync();
        NotifyOfPropertyChange(() => Cheques);
    }

    public async Task SearchBoxKeyDownHandlerAsync(ActionExecutionContext context)
    {
        if (!User.UserSettings.SearchWhenTyping && context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
        {
            await SearchAsync();
        }
    }

    public async Task SearchBoxTextChangedHandlerAsync()
    {
        if (User.UserSettings.SearchWhenTyping)
        {
            await SearchAsync();
        }
    }

    public async Task EditChequeAsync()
    {
        if (!CanEditChequeAsync || Cheques == null || Cheques.Count == 0 || SelectedCheque == null || SelectedCheque.Id == 0) return;
        WindowManager wm = new();
        await wm.ShowWindowAsync(new ChequeDetailViewModel(CCM, SelectedCheque, Singleton, RefreshPageAsync));
    }

    public async Task DeleteChequeAsync()
    {
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