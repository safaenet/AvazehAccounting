using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using System.Threading.Tasks;
using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using SharedLibrary.Validators;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace AvazehWpf.ViewModels
{
    public class PrintInvoiceRegularA5ViewModel : ViewAware
    {
        public PrintInvoiceRegularA5ViewModel(InvoiceModel invoice)
        {
            Invoice = invoice;
        }

        private InvoiceModel _invoice;

        public InvoiceModel Invoice
        {
            get { return _invoice; }
            set { _invoice = value; NotifyOfPropertyChange(() => Invoice); }
        }
    }
}