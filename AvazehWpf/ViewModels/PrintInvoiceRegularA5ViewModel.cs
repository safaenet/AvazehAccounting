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
using AvazehWpf.Views;
using System.Windows.Documents;

namespace AvazehWpf.ViewModels
{
    public class PrintInvoiceRegularA5ViewModel : IViewAware
    {
        public PrintInvoiceRegularA5ViewModel(InvoiceModel invoice)
        {
            Invoice = invoice;
        }

        private InvoiceModel _invoice;
        private PrintInvoiceRegularA5View _view;

        public event EventHandler<ViewAttachedEventArgs> ViewAttached;

        public InvoiceModel Invoice
        {
            get { return _invoice; }
            set { _invoice = value; /*NotifyOfPropertyChange(() => Invoice);*/ }
        }

        public void PrintDoc()
        {
            //var pd=new PrintDialog();
            //if (pd.ShowDialog() == true)
            //{
            //    IDocumentPaginatorSource idp = _view.FlowDocX;
            //    pd.PrintDocument(idp.DocumentPaginator, "Avazeh");
            //}
            //var paginator = new ProgramPaginator(_view);
            //var dlg = new PrintDialog();
            //if ((bool)dlg.ShowDialog())
            //{
            //    paginator.PageSize = new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight);
            //    dlg.PrintDocument(paginator, "Program");
            //}
            var printDialog = new PrintDialog();
            //if (printDialog.ShowDialog() == true)
            //{
            //    //MessageBox.Show("W: " + printDialog.PrintableAreaWidth.ToString() + ", H: " + printDialog.PrintableAreaHeight.ToString());
            //    var paginator = new RandomTabularPaginator(100,
            //      new Size(printDialog.PrintableAreaWidth,
            //        printDialog.PrintableAreaHeight));

            //    printDialog.PrintDocument(paginator, "My Random Data Table");
            //}
            if (printDialog.ShowDialog() != true) return;
            var fd = PrintHelper.GetFixedDocument(_view, printDialog);
            PrintHelper.ShowPrintPreview(fd);
        }

        public void AttachView(object view, object context = null)
        {
            _view = view as PrintInvoiceRegularA5View;
            if (ViewAttached != null)
                ViewAttached(this,
                new ViewAttachedEventArgs() { Context = context, View = view });
        }

        public object GetView(object context = null)
        {
            return _view;
        }
    }
}