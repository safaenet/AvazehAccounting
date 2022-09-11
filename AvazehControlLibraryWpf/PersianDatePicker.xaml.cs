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
            persianDate = PC.GetPersianDate();
            Years = new();
            Months = new();
            Days = new();
            FillYearsAndMonths();
            //FillDays(1400, 1);
            DataContext = this;
        }

        private PersianCalendar PC;
        private string persianDate;
        public ObservableCollection<int> Years { get; init; }
        public ObservableCollection<KeyValuePair<int, string>> Months { get; init; }
        public ObservableCollection<int> Days { get; private set; }
        private int year = 1400;
        private int month;
        private int day;

        public string PersianDate
        {
            get { return persianDate; }
            set { persianDate = value; }
        }

        public int Day
        {
            get => day;
            set { day = value; }
        }

        public int Month
        {
            get => month;
            set { month = value; FillDays(year, month); }
        }

        public int Year
        {
            get => year;
            set { year = value; }
        }

        private void FillYearsAndMonths()
        {
            for (int i = 1390; i <= 1450; i++) Years.Add(i);

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

        private void FillDays(int year, int month)
        {
            int day = Day;
            Days.Clear();
            if (month <= 6)
            {
                for (int i = 1; i <= 31; i++) Days.Add(i);
                Day = day;
            }
            else if (month <= 11)
            {
                if (day == 31) day--;
                for (int i = 1; i <= 30; i++) Days.Add(i);
                Day = day;
            }
            else if (month == 12 && PC.IsLeapYear(year))
            {
                if (day == 31) day--;
                for (int i = 1; i <= 30; i++) Days.Add(i);
                Day = day;
            }
            else
            {
                if (day >= 30) day = 29;
                for (int i = 1; i <= 29; i++) Days.Add(i);
                Day = day;
            }
        }
    }
}