using SharedLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AvazehUserControlLibraryWpf
{
    /// <summary>
    /// Interaction logic for PersianDatePicker.xaml
    /// </summary>
    public partial class PersianDatePicker : UserControl
    {
        public PersianDatePicker()
        {
            InitializeComponent();
            PC = new();
            Years = new();
            Months = new();
            Days = new();
            for (int i = 1; i <= 31; i++) Days.Add(new KeyValuePair<int, string>(i, i.ToString()));
            for (int i = 1360; i <= 1450; i++) Years.Add(new KeyValuePair<int, string>(i, i.ToString()));
            //PersianDate = PC.GetPersianDate();
        }

        private PersianCalendar PC;
        private const string wildcard = "__";
        public ObservableCollection<KeyValuePair<int, string>> Years { get; private set; }
        public ObservableCollection<KeyValuePair<int, string>> Months { get; private set; }
        public ObservableCollection<KeyValuePair<int, string>> Days { get; private set; }

        public string PersianDate
        {
            get { return (string)GetValue(PersianDateProperty); }
            set
            {
                SetValue(PersianDateProperty, value);
            }
        }

        public static DependencyProperty PersianDateProperty =
            DependencyProperty.Register(nameof(PersianDate), typeof(string), typeof(PersianDatePicker), new PropertyMetadata("1355/02/28", InitialDateChanged));


        public int YearWidth
        {
            get { return (int)GetValue(YearWidthProperty); }
            set { SetValue(YearWidthProperty, value); }
        }
        public int DayWidth
        {
            get { return (int)GetValue(DayWidthProperty); }
            set { SetValue(DayWidthProperty, value); }
        }
        public int MonthFontSize
        {
            get { return (int)GetValue(MonthFontSizeProperty); }
            set { SetValue(MonthFontSizeProperty, value); }
        }
        public MonthType MonthDisplayType
        {
            get { return (MonthType)GetValue(MonthDisplayTypeProperty); }
            set { SetValue(MonthDisplayTypeProperty, value); }
        }

        public bool CanUserSelectAll
        {
            get { return (bool)GetValue(CanUserSelectAllProperty); }
            set { SetValue(CanUserSelectAllProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YearWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YearWidthProperty =
            DependencyProperty.Register(nameof(YearWidth), typeof(int), typeof(PersianDatePicker), new PropertyMetadata(55));

        // Using a DependencyProperty as the backing store for YearWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DayWidthProperty =
            DependencyProperty.Register(nameof(DayWidth), typeof(int), typeof(PersianDatePicker), new PropertyMetadata(41));

        // Using a DependencyProperty as the backing store for YearWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonthFontSizeProperty =
            DependencyProperty.Register(nameof(MonthFontSize), typeof(int), typeof(PersianDatePicker), new PropertyMetadata(12));

        // Using a DependencyProperty as the backing store for YearWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonthDisplayTypeProperty =
            DependencyProperty.Register(nameof(MonthDisplayType), typeof(MonthType), typeof(PersianDatePicker), new PropertyMetadata(MonthType.None, MonthTypeChanged));

        // Using a DependencyProperty as the backing store for canUserSelectAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanUserSelectAllProperty =
            DependencyProperty.Register(nameof(CanUserSelectAll), typeof(bool), typeof(PersianDatePicker), new PropertyMetadata(false, CanUserSelectAllChanged));

        private static void CanUserSelectAllChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not PersianDatePicker pdp) return;
            if ((e.NewValue as bool?) == true)
            {
                pdp.Days.Insert(0, new KeyValuePair<int, string>(0, "همه"));
                pdp.Months.Insert(0, new KeyValuePair<int, string>(0, "همه"));
                pdp.Years.Insert(0, new KeyValuePair<int, string>(0, "همه"));
            }
        }

        private static void MonthTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pdp = d as PersianDatePicker;
            if (e.NewValue is null || pdp is null) return;
            pdp.Months.Clear();
            if (pdp.CanUserSelectAll) pdp.Months.Add(new(0, "همه"));
            if (pdp.MonthDisplayType == MonthType.Full)
            {
                pdp.Months.Add(new(1, "(فروردین (1"));
                pdp.Months.Add(new(2, "(اردیبهشت (2"));
                pdp.Months.Add(new(3, "(خرداد (3"));
                pdp.Months.Add(new(4, "(تیر (4"));
                pdp.Months.Add(new(5, "(مرداد (5"));
                pdp.Months.Add(new(6, "(شهریور (6"));
                pdp.Months.Add(new(7, "(مهر (7"));
                pdp.Months.Add(new(8, "(آبان (8"));
                pdp.Months.Add(new(9, "(آذر (9"));
                pdp.Months.Add(new(10, "(دی (10"));
                pdp.Months.Add(new(11, "(بهمن (11"));
                pdp.Months.Add(new(12, "(اسفند (12"));
            }
            else if (pdp.MonthDisplayType == MonthType.Letters)
            {
                pdp.Months.Add(new(1, "فروردین"));
                pdp.Months.Add(new(2, "اردیبهشت "));
                pdp.Months.Add(new(3, "خرداد "));
                pdp.Months.Add(new(4, "تیر"));
                pdp.Months.Add(new(5, "مرداد"));
                pdp.Months.Add(new(6, "شهریور"));
                pdp.Months.Add(new(7, "مهر"));
                pdp.Months.Add(new(8, "آبان"));
                pdp.Months.Add(new(9, "آذر"));
                pdp.Months.Add(new(10, "دی"));
                pdp.Months.Add(new(11, "بهمن"));
                pdp.Months.Add(new(12, "اسفند"));
            }
            else if (pdp.MonthDisplayType == MonthType.Numbers)
            {
                for (int i = 1; i <= 12; i++) pdp.Months.Add(new(i, i.ToString()));
            }
        }

        private static void InitialDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is null || d is not PersianDatePicker pdp) return;
            var subs = ((string)e.NewValue).Split('/');
            if (subs.Length != 3) throw new InvalidCastException();
            int day;
            int month;
            int year;
            if (subs[2] == wildcard) day = 0; else if (!int.TryParse(subs[2], NumberStyles.Any, CultureInfo.InvariantCulture, out day)) throw new InvalidCastException();
            if (subs[1] == wildcard) month = 0; else if (!int.TryParse(subs[1], NumberStyles.Any, CultureInfo.InvariantCulture, out month)) throw new InvalidCastException();
            if (subs[0] == wildcard + wildcard) year = 0; else if (!int.TryParse(subs[0], NumberStyles.Any, CultureInfo.InvariantCulture, out year)) throw new InvalidCastException();
            if (pdp != null)
            {
                pdp.Day = day;
                pdp.Month = month;
                pdp.Year = year;
            }
        }

        public int Day
        {
            get { return (int)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }

        public static DependencyProperty DayProperty =
            DependencyProperty.Register("Day", typeof(int), typeof(PersianDatePicker), new PropertyMetadata(1, OnSubDateChanged));

        public int Month
        {
            get { return (int)GetValue(MonthProperty); }
            set { SetValue(MonthProperty, value); CorrectDaysOnLeapYear(); }
        }

        public static DependencyProperty MonthProperty =
            DependencyProperty.Register("Month", typeof(int), typeof(PersianDatePicker), new PropertyMetadata(1, OnSubDateChanged));
        public int Year
        {
            get { return (int)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); CorrectDaysOnLeapYear(); }
        }

        public static DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof(int), typeof(PersianDatePicker), new PropertyMetadata(1390, OnSubDateChanged));

        private static void OnSubDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pdp = d as PersianDatePicker;
            if (pdp is not null && e.Property == YearProperty) pdp.PersianDate = string.Format("{0}/{1}/{2}", (int)e.NewValue == 0 ? wildcard + wildcard : string.Format("{0:0000}", e.NewValue), (int)pdp.Month == 0 ? wildcard : string.Format("{0:00}", pdp.Month), (int)pdp.Day == 0 ? wildcard : string.Format("{0:00}", pdp.Day));
            if (pdp is not null && e.Property == MonthProperty) pdp.PersianDate = string.Format("{0}/{1}/{2}", (int)pdp.Year == 0 ? wildcard + wildcard : string.Format("{0:0000}", pdp.Year), (int)e.NewValue == 0 ? wildcard : string.Format("{0:00}", e.NewValue), (int)pdp.Day == 0 ? wildcard : string.Format("{0:00}", pdp.Day));
            if (pdp is not null && e.Property == DayProperty) pdp.PersianDate = string.Format("{0}/{1}/{2}", (int)pdp.Year == 0 ? wildcard + wildcard : string.Format("{0:0000}", pdp.Year), (int)pdp.Month == 0 ? wildcard : string.Format("{0:00}", pdp.Month), (int)e.NewValue == 0 ? wildcard : string.Format("{0:00}", e.NewValue));
        }

        private void CorrectDaysOnLeapYear()
        {
            //if (Month == 12 && PC.IsLeapYear(Year) && !Days.Where(d => d.Key == 30).Any()) Days.Add(new KeyValuePair<int, string>(30, "30"));
            //if (Month <= 11 && !Days.Where(d => d.Key == 30).Any()) Days.Add(new KeyValuePair<int, string>(30, "30"));
            //if (Month == 12 && !PC.IsLeapYear(Year)) Days.Remove(new KeyValuePair<int, string>(30, "30"));
            //if (Month >= 7) Days.Remove(new KeyValuePair<int, string>(31, "31"));
            //if (Month <= 6 && !Days.Where(d => d.Key == 31).Any()) Days.Add(new KeyValuePair<int, string>(31, "31"));
            if (Month == 12 && (Year==0 || PC.IsLeapYear(Year)) && !Days.Where(d => d.Key == 30).Any()) Days.Add(new KeyValuePair<int, string>(30, "30"));
            if (Month <= 11 && !Days.Where(d => d.Key == 30).Any()) Days.Add(new KeyValuePair<int, string>(30, "30"));
            if (Month == 12 && Year!=0 && !PC.IsLeapYear(Year)) Days.Remove(new KeyValuePair<int, string>(30, "30"));
            if (Month >= 7) Days.Remove(new KeyValuePair<int, string>(31, "31"));
            if (Month <= 6 && !Days.Where(d => d.Key == 31).Any()) Days.Add(new KeyValuePair<int, string>(31, "31"));
        }

        public enum MonthType
        {
            None,
            Full,
            Numbers,
            Letters
        }
    }
}