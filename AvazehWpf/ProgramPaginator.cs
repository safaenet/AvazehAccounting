using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace AvazehWpf
{
    public class ProgramPaginator : DocumentPaginator
    {
        private FrameworkElement Element;
        private ProgramPaginator()
        {
        }

        public ProgramPaginator(FrameworkElement element)
        {
            Element = element;
        }

        public override DocumentPage GetPage(int pageNumber)
        {

            Element.RenderTransform = new TranslateTransform(-PageSize.Width * (pageNumber % Columns), -PageSize.Height * (pageNumber / Columns));

            Size elementSize = new Size(
                Element.ActualWidth,
                Element.ActualHeight);
            Element.Measure(elementSize);
            Element.Arrange(new Rect(new Point(0, 0), elementSize));

            var page = new DocumentPage(Element);
            Element.RenderTransform = null;

            return page;
        }

        public override bool IsPageCountValid
        {
            get { return true; }
        }

        public int Columns
        {
            get
            {
                return (int)Math.Ceiling(Element.ActualWidth / PageSize.Width);
            }
        }
        public int Rows
        {
            get
            {
                return (int)Math.Ceiling(Element.ActualHeight / PageSize.Height);
            }
        }

        public override int PageCount
        {
            get
            {
                return Columns * Rows;
            }
        }

        public override Size PageSize
        {
            set; get;
        }

        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }
    }

    public class RandomTabularPaginator : DocumentPaginator
    {
        private int _RowsPerPage;
        private Size _PageSize;
        private int _Rows;
        public RandomTabularPaginator(int rows, Size pageSize)
        {
            _Rows = rows;
            PageSize = pageSize;
        }

        /// <summary>
        /// When overridden in a derived class, 
        /// gets the DocumentPage for the specified page number
        /// </summary>
        /// <param name="pageNumber">
        /// The zero-based page number of the document 
        /// page that is needed.
        /// </param>
        /// <returns>
        /// The DocumentPage for the specified pageNumber, 
        /// or DocumentPage.Missing if the page does not exist.
        /// </returns>
        public override DocumentPage GetPage(int pageNumber)
        {
            int currentRow = _RowsPerPage * pageNumber;

            var page = new PageElement(currentRow,
              Math.Min(_RowsPerPage, _Rows - currentRow))
            {
                Width = PageSize.Width,
                Height = PageSize.Height,
            };

            page.Measure(PageSize);
            page.Arrange(new Rect(new Point(0, 0), PageSize));

            return new DocumentPage(page);
        }


        /// <summary>
        /// When overridden in a derived class, gets a value 
        /// indicating whether PageCount is the total number of pages. 
        /// </summary>
        public override bool IsPageCountValid
        {
            get { return true; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a count 
        /// of the number of pages currently formatted.
        /// </summary>
        public override int PageCount
        {
            get { return (int)Math.Ceiling(_Rows / (double)_RowsPerPage); }
        }

        /// <summary>
        /// When overridden in a derived class, gets or 
        /// sets the suggested width and height of each page.
        /// </summary>
        public override Size PageSize
        {
            get { return _PageSize; }
            set
            {
                _PageSize = value;

                _RowsPerPage = PageElement.RowsPerPage(PageSize.Height);
            }
        }

        /// <summary>
        /// When overridden in a derived class, 
        /// returns the element being paginated.
        /// </summary>
        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }
    }

    public class PageElement : UserControl
    {
        private const int PageMargin = 75;
        private const int HeaderHeight = 25;
        private const int LineHeight = 20;
        private const int ColumnWidth = 140;

        private int _CurrentRow;
        private int _Rows;

        public PageElement(int currentRow, int rows)
        {
            Margin = new Thickness(PageMargin);
            _CurrentRow = currentRow;
            _Rows = rows;
        }

        public static int RowsPerPage(double height)
        {
            return (int)Math.Floor((height - (2 * PageMargin)
              - HeaderHeight) / LineHeight);
        }

        private static FormattedText MakeText(string text)
        {
            return new FormattedText(text, CultureInfo.CurrentCulture,
              FlowDirection.LeftToRight, new Typeface("Tahoma"), 14, Brushes.Black);
        }

        protected override void OnRender(DrawingContext dc)
        {
            Point curPoint = new Point(0, 0);

            dc.DrawText(MakeText("Row Number"), curPoint);
            curPoint.X += ColumnWidth;
            for (int i = 1; i < 4; i++)
            {
                dc.DrawText(MakeText("Column " + i), curPoint);
                curPoint.X += ColumnWidth;
            }

            curPoint.X = 0;
            curPoint.Y += LineHeight;

            dc.DrawRectangle(Brushes.Black, null,
              new Rect(curPoint, new Size(Width, 2)));
            curPoint.Y += HeaderHeight - LineHeight;

            Random numberGen = new Random();
            for (int i = _CurrentRow; i < _CurrentRow + _Rows; i++)
            {
                dc.DrawText(MakeText(i.ToString()), curPoint);
                curPoint.X += ColumnWidth;
                for (int j = 1; j < 4; j++)
                {
                    dc.DrawText(MakeText(numberGen.Next().ToString()), curPoint);
                    curPoint.X += ColumnWidth;
                }
                curPoint.Y += LineHeight;
                curPoint.X = 0;
            }
        }
    }

    public static class PrintHelper
    {
        public static FixedDocument GetFixedDocument(FrameworkElement toPrint, PrintDialog printDialog)
        {
            PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
            Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            Size visibleSize = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
            FixedDocument fixedDoc = new FixedDocument();
            //If the toPrint visual is not displayed on screen we neeed to measure and arrange it  
            toPrint.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            toPrint.Arrange(new Rect(new Point(0, 0), toPrint.DesiredSize));
            //  
            Size size = toPrint.DesiredSize;
            //Will assume for simplicity the control fits horizontally on the page  
            double yOffset = 0;
            while (yOffset < size.Height)
            {
                VisualBrush vb = new VisualBrush(toPrint);
                vb.Stretch = Stretch.None;
                vb.AlignmentX = AlignmentX.Left;
                vb.AlignmentY = AlignmentY.Top;
                vb.ViewboxUnits = BrushMappingMode.Absolute;
                vb.TileMode = TileMode.None;
                vb.Viewbox = new Rect(0, yOffset, visibleSize.Width, visibleSize.Height);
                PageContent pageContent = new PageContent();
                FixedPage page = new FixedPage();
                ((IAddChild)pageContent).AddChild(page);
                fixedDoc.Pages.Add(pageContent);
                page.Width = pageSize.Width;
                page.Height = pageSize.Height;
                Canvas canvas = new Canvas();
                FixedPage.SetLeft(canvas, capabilities.PageImageableArea.OriginWidth);
                FixedPage.SetTop(canvas, capabilities.PageImageableArea.OriginHeight);
                canvas.Width = visibleSize.Width;
                canvas.Height = visibleSize.Height;
                canvas.Background = vb;
                page.Children.Add(canvas);
                yOffset += visibleSize.Height;
            }
            return fixedDoc;
        }

        public static void ShowPrintPreview(FixedDocument fixedDoc)
        {
            Window wnd = new Window();
            DocumentViewer viewer = new DocumentViewer();
            viewer.Document = fixedDoc;
            wnd.Content = viewer;
            wnd.ShowDialog();
        }
    }
}