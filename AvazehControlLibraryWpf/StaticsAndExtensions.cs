using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehUserControlLibraryWpf
{
    internal static class StaticsAndExtensions
    {
        public static string GetPersianDate(this PersianCalendar pCal)
        {
            var now = System.DateTime.Now;
            return string.Format("{0:0000}/{1:00}/{2:00}", pCal.GetYear(now), pCal.GetMonth(now), pCal.GetDayOfMonth(now));
        }
    }
}