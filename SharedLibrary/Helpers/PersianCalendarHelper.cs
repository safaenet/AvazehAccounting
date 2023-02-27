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

    public static string ToPersianDate(this DateTime datetime)
    {
        PersianCalendar pCal = new();
        return $"{pCal.GetYear(datetime):0000}/{pCal.GetMonth(datetime):00}/{pCal.GetDayOfMonth(datetime):00}";
    }
    public static string ToPersianDateTime(this DateTime datetime)
    {
        PersianCalendar pCal = new();
        return $"{pCal.GetYear(datetime):0000}/{pCal.GetMonth(datetime):00}/{pCal.GetDayOfMonth(datetime):00} {datetime.ToTime()}";
    }

    public static string ToTime(this DateTime datetime)
    {
        return $"{ datetime:'HH:mm:ss'}";
    }

    public static DateOnly? ToGregorianDate(this string persianDate)
    {
        if (persianDate == null) return null;
        var subs = persianDate.Split('/');
        if (subs == null || subs.Length != 3) return null;
        PersianCalendar pc = new PersianCalendar();
        if (!int.TryParse(subs[2], out var day)) return null;
        if (!int.TryParse(subs[1], out var month)) return null;
        if (!int.TryParse(subs[0], out var year)) return null;
        DateOnly dt = new DateOnly(year, month, day, pc);
        return dt;
    }
}