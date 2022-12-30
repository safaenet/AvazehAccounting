using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace DataLibraryCore.DataAccess
{
    public static class SettingsDataAccess
    {
        public static IConfiguration AppConfiguration()
        {
            IConfiguration conf;
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                conf = builder.Build();
                return conf;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error when loading configuration");
            }
            return null;
        }
    }
}