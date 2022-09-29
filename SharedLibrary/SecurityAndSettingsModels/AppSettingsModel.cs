using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.SecurityAndSettingsModels
{
    public class AppSettingsModel
    {
        public GeneralSettingsModel GeneralSettings { get; set; }
        public PrintSettingsModel PrintSettings { get; set; }
    }
}