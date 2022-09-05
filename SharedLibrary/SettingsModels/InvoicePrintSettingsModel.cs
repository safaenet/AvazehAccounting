using SharedLibrary.DalModels;
using System;
using System.Collections.Generic;

namespace SharedLibrary.SettingsModels
{
    public class InvoicePrintSettingsModel
    {
        public string MainHeaderText { get; set; } = "فروشگاه آوازه";
        public string HeaderDescription1 { get; set; } = "کرکره برقی، جک پارکینگی، دزدگیر اماکن";
        public string HeaderDescription2 { get; set; } = "01734430827 - 09357330303";
        public string LeftHeaderImage { get; set; } = @"\Images\LeftImage.png";
        public string RightHeaderImage { get; set; } = @"\Images\RightImage.png";
        public List<UserDescriptionModel> UserDescriptions { get; set; }
        public string FooterTextLeft { get; set; } = "کیفیت برتر، قیمت مناسب";
        public string FooterTextRight { get; set; } = "توسعه دهنده نرم افزار: صفا دانا";

        public int MainHeaderTextFontSize { get; set; } = 30;
        public int HeaderDescriptionFontSize { get; set; } = 10;
        public int InvoiceTypeTextFontSize { get; set; } = 16;
        public int PageHeaderFontSize { get; set; } = 10;
        public int DetailsFontSize { get; set; } = 10;
        public int PageFooterFontSize { get; set; } = 10;
        public int DescriptionFontSize { get; set; } = 14;

        public string DefaultPrintLayout { get; set; } = "عمودی"; //Landscape, Portrait
        public string DefaultPaperSize { get; set; } = "A5"; //A5, A4
    }
}