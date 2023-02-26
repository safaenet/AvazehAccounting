using System;
using System.Globalization;

namespace SharedLibrary.Helpers;

public static class PersianCalendarHelper
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
    /// Gets the current Persian date + AddDays in format of "YYYY/mm/dd"
    /// </summary>
    public static string GetPersianDate(int AddDays)
    {
        PersianCalendar pCal = new();
        DateTime now = DateTime.Now.AddDays(AddDays);
        return string.Format("{0:0000}/{1:00}/{2:00}", pCal.GetYear(now), pCal.GetMonth(now), pCal.GetDayOfMonth(now));
    }

    /// <summary>
    /// Gets the current Persian date in format of "YYYYmmdd"
    /// </summary>
    public static string GetCurrentRawPersianDate()
    {
        PersianCalendar pCal = new();
        DateTime now = DateTime.Now;
        return string.Format("{0:0000}{1:00}{2:00}", pCal.GetYear(now), pCal.GetMonth(now), pCal.GetDayOfMonth(now));
    }

    /// <summary>
    /// Gets the current Persian date + AddDays in format of "YYYYmmdd"
    /// </summary>
    public static string GetRawPersianDate(int AddDays)
    {
        PersianCalendar pCal = new();
        DateTime now = DateTime.Now.AddDays(AddDays);
        return string.Format("{0:0000}{1:00}{2:00}", pCal.GetYear(now), pCal.GetMonth(now), pCal.GetDayOfMonth(now));
    }

    /// <summary>
    /// Gets the current time in format of "HH:mm:ss"
    /// </summary>
    public static string GetCurrentTime() => DateTime.Now.ToString("HH:mm:ss");
}