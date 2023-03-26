using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;
using SharedLibrary.SecurityAndSettingsModels;
using SharedLibrary.Validators;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace AvazehWpf.ViewModels;

public class PrevInvoiceListViewModel : Screen
{
    public PrevInvoiceListViewModel(IInvoiceCollectionManager manager, int InvoiceId, string CustomerFullName = "Customer")
    {
        ICM = manager;
        _SelectedInvoice = new();
        WindowTitle = "فاکتورهای " + CustomerFullName;
        this.InvoiceId = InvoiceId;
        _ = SearchAsync().ConfigureAwait(true);
    }

    private IInvoiceCollectionManager _ICM;
    private InvoiceListModel _SelectedInvoice;

    public InvoiceListModel SelectedInvoice
    {
        get { return _SelectedInvoice; }
        set { _SelectedInvoice = value; NotifyOfPropertyChange(() => SelectedInvoice); }
    }

    public IInvoiceCollectionManager ICM
    {
        get { return _ICM; }
        set
        {
            _ICM = value;
            NotifyOfPropertyChange(() => ICM);
            NotifyOfPropertyChange(() => Invoices);
        }
    }

    public ObservableCollection<InvoiceListModel> Invoices
    {
        get => ICM.Items;
        set
        {
            ICM.Items = value;
            NotifyOfPropertyChange(() => ICM);
            NotifyOfPropertyChange(() => Invoices);
        }
    }

    public int InvoiceId { get; set; }

    public string WindowTitle { get; set; }

    public async Task SearchAsync()
    {
        await ICM.LoadFirstPageAsync();
        NotifyOfPropertyChange(() => Invoices);
    }

    public void SearchSync()
    {
        _ = Task.Run(SearchAsync);
    }

    public async Task SearchBoxKeyDownHandlerAsync(ActionExecutionContext context)
    {
        if (context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
        {
            await SearchAsync();
        }
    }

    public async Task SearchBoxTextChangedHandlerAsync()
    {
        await SearchAsync();
    }

    public void Window_PreviewKeyDown(object window, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) (GetView() as Window).Close();
    }
}