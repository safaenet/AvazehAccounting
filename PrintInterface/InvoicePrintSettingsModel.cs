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
        private int mainHeaderTextFontSize = 30;
        private int headerDescriptionFontSize = 10;
        private int invoiceTypeTextFontSize = 16;
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
        public int MainHeaderTextFontSize { get => mainHeaderTextFontSize; set => mainHeaderTextFontSize = value; }
        public int HeaderDescriptionFontSize { get => headerDescriptionFontSize; set => headerDescriptionFontSize = value; }
        public int InvoiceTypeTextFontSize { get => invoiceTypeTextFontSize; set => invoiceTypeTextFontSize = value; }
        public int PageHeaderFontSize { get => pageHeaderFontSize; set => pageHeaderFontSize = value; }
        public int DetailsFontSize { get => detailsFontSize; set => detailsFontSize = value; }
        public int PageFooterFontSize { get => pageFooterFontSize; set => pageFooterFontSize = value; }
        public int DescriptionFontSize { get => descriptionFontSize; set => descriptionFontSize = value; }
        public string DefaultPrintLayout { get => defaultPrintLayout; set => defaultPrintLayout = value; }
        public string DefaultPaperSize { get => defaultPaperSize; set => defaultPaperSize = value; }
    }
}