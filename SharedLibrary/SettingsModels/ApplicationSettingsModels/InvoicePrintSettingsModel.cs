using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SettingsModels.ApplicationSettingsModels
{
    public class InvoicePrintSettingsModel
    {
        public string MainHeaderText { get; set; }
        public string HeaderDescription1 { get; set; }
        public string HeaderDescription2 { get; set; }
        public string LeftHeaderImage { get; set; }
        public string RightHeaderImage { get; set; }
        public string[] UserDescriptions { get; set; }
        public string FooterTextLeft { get; set; }
        public string FooterTextRight { get; set; }

        public int MainHeaderTextFontSize { get; set; }
        public int HeaderDescriptionFontSize { get; set; }
        public int InvoiceTypeTextFontSize { get; set; }
        public int PageHeaderFontSize { get; set; }
        public int DetailsFontSize { get; set; }
        public int PageFooterFontSize { get; set; }
        public int DescriptionFontSize { get; set; }

        public string PrintLayout { get; set; }
        public string PaperSize { get; set; }
    }
}