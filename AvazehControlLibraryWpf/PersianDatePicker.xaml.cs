using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            for (int i = 1; i <= 31; i++) Days.Add(i);
            FillYears();
            //PersianDate = PC.GetPersianDate();
        }

        private void FillMonths()
        {
            if (MonthDisplayType == MonthType.Full)
            {
                Months.Add(new(1, "(فروردین (1"));
                Months.Add(new(2, "(اردیبهشت (2"));
                Months.Add(new(3, "(خرداد (3"));
                Months.Add(new(4, "(تیر (4"));
                Months.Add(new(5, "(مرداد (5"));
                Months.Add(new(6, "(شهریور (6"));
                Months.Add(new(7, "(مهر (7"));
                Months.Add(new(8, "(آبان (8"));
                Months.Add(new(9, "(آذر (9"));
                Months.Add(new(10, "(دی (10"));
                Months.Add(new(11, "(بهمن (11"));
                Months.Add(new(12, "(اسفند (12"));
            }
            else if (MonthDisplayType == MonthType.Letters)
            {
                Months.Add(new(1, "فروردین"));
                Months.Add(new(2, "اردیبهشت "));
                Months.Add(new(3, "خرداد "));
                Months.Add(new(4, "تیر"));
                Months.Add(new(5, "مرداد"));
                Months.Add(new(6, "شهریور"));
                Months.Add(new(7, "مهر"));
                Months.Add(new(8, "آبان"));
                Months.Add(new(9, "آذر"));
                Months.Add(new(10, "دی"));
                Months.Add(new(11, "بهمن"));
                Months.Add(new(12, "اسفند"));
            }
            else if (MonthDisplayType == MonthType.Numbers)
            {
                for (int i = 1; i <= 12; i++) Months.Add(new(i, i.ToString()));
            }
        }

        private PersianCalendar PC;
        public ObservableCollection<int> Years { get; private set; }
        public ObservableCollection<KeyValuePair<int, string>> Months { get; private set; }
        public ObservableCollection<int> Days { get; private set; }

        public string PersianDate
        {
            get { return (string)GetValue(PersianDateProperty); }
            set
            {
                SetValue(PersianDateProperty, value);
            }
        }

        public static DependencyProperty PersianDateProperty =
            DependencyProperty.Register("PersianDate", typeof(string), typeof(PersianDatePicker), new PropertyMetadata("1395/02/28", OnMainDateChanged));


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

        // Using a DependencyProperty as the backing store for YearWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YearWidthProperty =
            DependencyProperty.Register("YearWidth", typeof(int), typeof(PersianDatePicker), new PropertyMetadata(55));

        // Using a DependencyProperty as the backing store for YearWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DayWidthProperty =
            DependencyProperty.Register("DayWidth", typeof(int), typeof(PersianDatePicker), new PropertyMetadata(41));

        // Using a DependencyProperty as the backing store for YearWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonthFontSizeProperty =
            DependencyProperty.Register("MonthFontSize", typeof(int), typeof(PersianDatePicker), new PropertyMetadata(12));

        // Using a DependencyProperty as the backing store for YearWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonthDisplayTypeProperty =
            DependencyProperty.Register("MonthDisplayType", typeof(MonthType), typeof(PersianDatePicker), new PropertyMetadata(MonthType.None, MonthTypeChanged));

        private static void MonthTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pdp = d as PersianDatePicker;
            if (e.NewValue is null || pdp is null) return;
            pdp.Months.Clear();
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

        private static void OnMainDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pdp = d as PersianDatePicker;
            if (e.NewValue is null || pdp is null) return;
            var subs = ((string)e.NewValue).Split('/');
            if (subs.Length != 3) throw new InvalidCastException();
            if (!int.TryParse(subs[2], out var day)) throw new InvalidCastException();
            if (!int.TryParse(subs[1], out var month)) throw new InvalidCastException();
            if (!int.TryParse(subs[0], out var year)) throw new InvalidCastException();

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
            set { SetValue(MonthProperty, value); FillDays(); }
        }

        public static DependencyProperty MonthProperty =
            DependencyProperty.Register("Month", typeof(int), typeof(PersianDatePicker), new PropertyMetadata(1, OnSubDateChanged));
        public int Year
        {
            get { return (int)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); FillDays(); }
        }

        public static DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof(int), typeof(PersianDatePicker), new PropertyMetadata(1390, OnSubDateChanged));

        private static void OnSubDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pdp = d as PersianDatePicker;
            if (pdp is not null && e.Property == YearProperty) pdp.PersianDate = string.Format("{0:0000}/{1:00}/{2:00}", e.NewValue, pdp.Month, pdp.Day);
            if (pdp is not null && e.Property == MonthProperty) pdp.PersianDate = string.Format("{0:0000}/{1:00}/{2:00}", pdp.Year, e.NewValue, pdp.Day);
            if (pdp is not null && e.Property == DayProperty) pdp.PersianDate = string.Format("{0:0000}/{1:00}/{2:00}", pdp.Year, pdp.Month, e.NewValue);
        }

        private void FillYears()
        {
            for (int i = 1390; i <= 1450; i++) Years.Add(i);
        }

        private void FillDays()
        {
            if (Month == 12 && PC.IsLeapYear(Year) && !Days.Where(d => d == 30).Any()) Days.Add(30);
            if (Month <= 11 && !Days.Where(d => d == 30).Any()) Days.Add(30);
            if (Month == 12 && !PC.IsLeapYear(Year)) Days.Remove(30);
            if (Month >= 7) Days.Remove(31);
            if (Month <= 6 && !Days.Where(d => d == 31).Any()) Days.Add(31);
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