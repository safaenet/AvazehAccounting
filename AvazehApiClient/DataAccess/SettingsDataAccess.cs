using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess
{
    public static class SettingsDataAccess
    {
        public static IConfiguration AppConfiguration()
        {
            IConfiguration conf;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            conf = builder.Build();
            return conf;
        }
    }
}