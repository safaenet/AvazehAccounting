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
        public string[] UserDescriptions { get; set; }
        public string LeftHeaderImage { get; set; }
        public string RightHeaderImage { get; set; }
    }
}