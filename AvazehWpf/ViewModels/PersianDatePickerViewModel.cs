using AvazehApiClient.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehWpf.ViewModels
{
    public class PersianDatePickerViewModel
    {
        public PersianDatePickerViewModel()
        {
            PC = new();
            PersianDate = PC.GetPersianDate();
        }
        private PersianCalendar PC;
        private string persianDate;
        private int day;
        private int month;
        private int year;

        public string PersianDate
        {
            get { return persianDate; }
            set { persianDate = value; }
        }

        public int Day
        {
            get => string.IsNullOrEmpty(PersianDate) ? 0 : int.Parse(PersianDate[8..]);
            set { day = value; }
        }

        public int Month
        {
            get => string.IsNullOrEmpty(PersianDate) ? 0 : int.Parse(PersianDate.Substring(5, 2));
            set { month = value; }
        }

        public int Year
        {
            get => string.IsNullOrEmpty(PersianDate) ? 0 : int.Parse(PersianDate.Substring(0, 4));
            set { year = value; }
        }
    }
}