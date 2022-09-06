using System;
using System.Collections.Generic;

namespace PrintInterface
{
    public class InvoicePrintSettingsModel
    {
        private string mainHeaderText = "فروشگاه آوازه";
        private string headerDescription1 = "کرکره برقی، جک پارکینگی، دزدگیر اماکن";
        private string headerDescription2 = "01734430827 - 09357330303";
        private string leftHeaderImage = @"\Images\LeftImage.png";
        private string rightHeaderImage = @"\Images\RightImage.png";
        private List<UserDescriptionModel> userDescriptions;
        private string footerTextLeft = "کیفیت برتر، قیمت مناسب";
        private string footerTextRight = "توسعه دهنده نرم افزار: صفا دانا";
        private int mainHeaderTextFontSizeA5P = 30;
        private int headerDescriptionFontSizeA5P = 10;
        private int invoiceTypeTextFontSizeA5P = 16;
        private int mainHeaderTextFontSizeA5L = 30;
        private int headerDescriptionFontSizeA5L = 10;
        private int invoiceTypeTextFontSizeA5L = 14;
        private int mainHeaderTextFontSizeA4P = 30;
        private int headerDescriptionFontSizeA4P = 10;
        private int invoiceTypeTextFontSizeA4P = 14;
        private int pageHeaderFontSize = 10;
        private int detailsFontSize = 10;
        private int pageFooterFontSize = 10;
        private int descriptionFontSize = 14;
        private string defaultPrintLayout = "عمودی";
        private string defaultPaperSize = "A5";

        public string MainHeaderText { get => mainHeaderText; set => mainHeaderText = value; }
        public string HeaderDescription1 { get => headerDescription1; set => headerDescription1 = value; }
        public string HeaderDescription2 { get => headerDescription2; set => headerDescription2 = value; }
        public string LeftHeaderImage { get => leftHeaderImage; set => leftHeaderImage = value; }
        public string RightHeaderImage { get => rightHeaderImage; set => rightHeaderImage = value; }
        public List<UserDescriptionModel> UserDescriptions { get => userDescriptions; set => userDescriptions = value; }
        public string FooterTextLeft { get => footerTextLeft; set => footerTextLeft = value; }
        public string FooterTextRight { get => footerTextRight; set => footerTextRight = value; }

        public int MainHeaderTextFontSizeA5P { get => mainHeaderTextFontSizeA5P; set => mainHeaderTextFontSizeA5P = value; }
        public int HeaderDescriptionFontSizeA5P { get => headerDescriptionFontSizeA5P; set => headerDescriptionFontSizeA5P = value; }
        public int InvoiceTypeTextFontSizeA5P { get => invoiceTypeTextFontSizeA5P; set => invoiceTypeTextFontSizeA5P = value; }

        public int MainHeaderTextFontSizeA5L { get => mainHeaderTextFontSizeA5L; set => mainHeaderTextFontSizeA5L = value; }
        public int HeaderDescriptionFontSizeA5L { get => headerDescriptionFontSizeA5L; set => headerDescriptionFontSizeA5L = value; }
        public int InvoiceTypeTextFontSizeA5L { get => invoiceTypeTextFontSizeA5L; set => invoiceTypeTextFontSizeA5L = value; }

        public int MainHeaderTextFontSizeA4P { get => mainHeaderTextFontSizeA4P; set => mainHeaderTextFontSizeA4P = value; }
        public int HeaderDescriptionFontSizeA4P { get => headerDescriptionFontSizeA4P; set => headerDescriptionFontSizeA4P = value; }
        public int InvoiceTypeTextFontSizeA4P { get => invoiceTypeTextFontSizeA4P; set => invoiceTypeTextFontSizeA4P = value; }

        public int PageHeaderFontSize { get => pageHeaderFontSize; set => pageHeaderFontSize = value; }
        public int DetailsFontSize { get => detailsFontSize; set => detailsFontSize = value; }
        public int PageFooterFontSize { get => pageFooterFontSize; set => pageFooterFontSize = value; }
        public int DescriptionFontSize { get => descriptionFontSize; set => descriptionFontSize = value; }
        public string DefaultPrintLayout { get => defaultPrintLayout; set => defaultPrintLayout = value; }
        public string DefaultPaperSize { get => defaultPaperSize; set => defaultPaperSize = value; }
    }
}