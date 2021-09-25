using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AvazehApiClient.Models
{
    public static class PersianCalendarModel
    {
        /// <summary>
        /// Gets the current Persian date in format of "YYYY/mm/dd"
        /// </summary>
        public static string GetCurrentPersianDate()
        {
            PersianCalendar pCal = new();
            DateTime now = DateTime.Now;
            return string.Format("{0:0000}/{1:00}/{2:00}", pCal.GetYear(now), pCal.GetMonth(now), pCal.GetDayOfMonth(now));
        }

        /// <summary>
        /// Gets the current time in format of "HH:mm:ss"
        /// </summary>
        public static string GetCurrentTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}