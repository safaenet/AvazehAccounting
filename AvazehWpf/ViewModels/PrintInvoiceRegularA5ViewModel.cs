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
using System.Windows.Markup;
using System.Printing;

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
            //    var paginator = new DocumentPaginator();

            //    printDialog.PrintDocument(paginator, "My Random Data Table");
            //}
            if (printDialog.ShowDialog() != true) return;
            //var fd = PrintHelper.GetFixedDocument(_view, printDialog);
            //PrintHelper.ShowPrintPreview(fd);
            FixedDocument fd = new();
            PageContent pc = new();
            FixedPage fp = new();
            var rb = _view.ReportBody;
            _view.MainGrid.Children.Remove(rb);

            PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
            Size pageSize = new Size(557, 797);
            Size visibleSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
            FixedDocument fixedDoc = new FixedDocument();
            //If the toPrint visual is not displayed on screen we neeed to measure and arrange it  
            rb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            rb.Arrange(new Rect(new Point(0, 0), rb.DesiredSize));
            //  
            Size size = rb.DesiredSize;
            VisualBrush vb = new VisualBrush(rb);
            vb.Stretch = Stretch.None;
            vb.AlignmentX = AlignmentX.Left;
            vb.AlignmentY = AlignmentY.Top;
            vb.ViewboxUnits = BrushMappingMode.Absolute;
            vb.TileMode = TileMode.None;
            vb.Viewbox = new Rect(0, 0, visibleSize.Width, visibleSize.Height);
            PageContent pageContent = new PageContent();
            FixedPage page = new FixedPage();
            ((IAddChild)pageContent).AddChild(page);
            fixedDoc.Pages.Add(pageContent);
            page.Width = pageSize.Width;
            page.Height = pageSize.Height;
            //Canvas canvas = new Canvas();
            //FixedPage.SetLeft(canvas, capabilities.PageImageableArea.OriginWidth);
            //FixedPage.SetTop(canvas, capabilities.PageImageableArea.OriginHeight);
            //canvas.Width = visibleSize.Width;
            //canvas.Height = visibleSize.Height;
            //canvas.Background = vb;
            //page.Children.Add(canvas);
            //yOffset += visibleSize.Height;
            fp.Children.Add(rb);
            BlockUIContainer buc = new();
            buc.Child = fp;
            ((IAddChild)pc).AddChild(buc);
            fd.Pages.Add(pc);
            Window wnd = new Window();
            wnd.DataContext = Invoice;
            DocumentViewer viewer = new DocumentViewer();
            viewer.Document = fixedDoc;
            wnd.Content = viewer;
            wnd.ShowDialog();
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