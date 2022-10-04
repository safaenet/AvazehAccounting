using AvazehApiClient.DataAccess;
using SharedLibrary.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AvazehWpf
{
    public class BooleanToReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
         => !(bool?)value ?? true;

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
         => !(value as bool?);
    }

    public class NameToBrushConverterForBackground : IValueConverter
    {
        private readonly string CurrentPersianDate = new PersianCalendar().GetPersianDate();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            string input = value.ToString();
            if (input == CurrentPersianDate) return Brushes.LightYellow;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class NameToBrushConverterForForeground : IValueConverter
    {
        private readonly string CurrentPersianDate = new PersianCalendar().GetPersianDate();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            string input = value.ToString();
            if (input == CurrentPersianDate) return Brushes.Black;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class AmountToBrushConverterForBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            double input = (double)value;
            return input switch
            {
                < 0 => Brushes.LightBlue,
                0 => Brushes.LightGreen,
                > 0 => Brushes.LightPink,
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class AmountToBrushConverterForBackgroundInverted : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            double input = (double)value;
            return input switch
            {
                < 0 => Brushes.LightPink,
                0 => Brushes.LightGreen,
                > 0 => Brushes.LightBlue,
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class AmountToBrushConverterForForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            //double input = (double)value;
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            Color input = ((string)value).ToColor();
            return input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            string input = ((Color)value).ToHex();
            return input;
        }
    }

    public class PersianOrderStringToEnglishOrderStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            if ((OrderType)value == OrderType.ASC) return "صعودی";
            return "نزولی";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            if ((string)value == "صعودی") return OrderType.ASC;
            return OrderType.DESC;
        }
    }

    public class EnglishToPersianInvoiceFinStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            else if ((InvoiceFinancialStatus)value == InvoiceFinancialStatus.Balanced) return "تسویه";
            else if ((InvoiceFinancialStatus)value == InvoiceFinancialStatus.Deptor) return "بدهکار";
            else if ((InvoiceFinancialStatus)value == InvoiceFinancialStatus.Creditor) return "بستانکار";
            else return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            if ((string)value == "صعودی") return OrderType.ASC;
            return OrderType.DESC;
        }
    }

    public class EnglishToPersianTransactionFinStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            else if ((TransactionFinancialStatus)value == TransactionFinancialStatus.Balanced) return "تسویه";
            else if ((TransactionFinancialStatus)value == TransactionFinancialStatus.Positive) return "مثبت";
            else if ((TransactionFinancialStatus)value == TransactionFinancialStatus.Negative) return "منفی";
            else return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            if ((string)value == "صعودی") return OrderType.ASC;
            return OrderType.DESC;
        }
    }

    public class EnglishToPersianChequeEventTypeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values[0] == DependencyProperty.UnsetValue || values[0] == null) return DependencyProperty.UnsetValue;
            var eventText = (values[1] == DependencyProperty.UnsetValue || string.IsNullOrEmpty((string)values[1])) ? string.Empty : (string)values[1];
            eventText = string.IsNullOrEmpty(eventText) ? string.Empty : $" - {eventText}";
            if ((string)values[0] == ChequeEventTypes.None.ToString()) return $"عادی{eventText}";
            else if ((string)values[0] == ChequeEventTypes.Holding.ToString()) return $"عادی{eventText}";
            else if ((string)values[0] == ChequeEventTypes.Sold.ToString()) return $"منتقل شده{eventText}";
            else if ((string)values[0] == ChequeEventTypes.NonSufficientFund.ToString()) return $"برگشت خورده{eventText}";
            else if ((string)values[0] == ChequeEventTypes.Cashed.ToString()) return $"وصول شده{eventText}";
            else return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    }

    public class ChequeEventToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 4 || values[0] == null) return DependencyProperty.UnsetValue;
            else if ((string)values[0] == ChequeEventTypes.Sold.ToString()) return new SolidColorBrush(((string)values[1]).ToColor());
            else if ((string)values[0] == ChequeEventTypes.NonSufficientFund.ToString()) return new SolidColorBrush(((string)values[2]).ToColor());
            else if ((string)values[0] == ChequeEventTypes.Cashed.ToString()) return new SolidColorBrush(((string)values[3]).ToColor());
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    }

    //public class DateToColorConverter : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (values == null || values.Length != 3 || values[0] == null || values[2] == null) return DependencyProperty.UnsetValue;
    //        if ((string)values[0] == (string)values[1]) return new SolidColorBrush(((string)values[2]).ToColor());
    //        return DependencyProperty.UnsetValue;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    //}

    public class DateToColorConverter : Freezable, IValueConverter
    {
        #region Overrides of Freezable    
        protected override Freezable CreateInstanceCore()
        {
            return new DateToColorConverter();
        }
        #endregion

        public string ItemColor
        {
            get { return (string)GetValue(ItemColorProperty); }
            set { SetValue(ItemColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemColorProperty =
            DependencyProperty.Register("ItemColor", typeof(string), typeof(DateToColorConverter), new PropertyMetadata(null));

        public string Date
        {
            get { return (string)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(string), typeof(DateToColorConverter), new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            if ((string)value == Date) return new SolidColorBrush(ItemColor.ToColor());
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    //public class AmountToColorConverter : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (values == null || values.Length != 4 || values[0] == null) return DependencyProperty.UnsetValue;
    //        switch ((double)values[0])
    //        {
    //            case 0:
    //                if (values[1] is not string || string.IsNullOrEmpty((string)values[1])) return DependencyProperty.UnsetValue;
    //                return new SolidColorBrush(((string)values[1]).ToColor());
    //            case > 0:
    //                if (values[2] is not string || string.IsNullOrEmpty((string)values[2])) return DependencyProperty.UnsetValue;
    //                return new SolidColorBrush(((string)values[2]).ToColor());
    //            case < 0:
    //                if (values[3] is not string || string.IsNullOrEmpty((string)values[3])) return DependencyProperty.UnsetValue;
    //                return new SolidColorBrush(((string)values[3]).ToColor());
    //        }
    //        return DependencyProperty.UnsetValue;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    //}

    public class AmountToColorConverter : Freezable, IValueConverter
    {
        #region Overrides of Freezable    
        protected override Freezable CreateInstanceCore()
        {
            return new AmountToColorConverter();
        }
        #endregion

        public string BalancedColor
        {
            get { return (string)GetValue(BalancedColorProperty); }
            set { SetValue(BalancedColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BalancedColorProperty =
            DependencyProperty.Register("BalancedColor", typeof(string), typeof(AmountToColorConverter), new PropertyMetadata(null));

        public string PositiveColor
        {
            get { return (string)GetValue(PositiveColorProperty); }
            set { SetValue(PositiveColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositiveColorProperty =
            DependencyProperty.Register("PositiveColor", typeof(string), typeof(AmountToColorConverter), new PropertyMetadata(null));

        public string NegativeColor
        {
            get { return (string)GetValue(NegativeColorProperty); }
            set { SetValue(NegativeColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NegativeColorProperty =
            DependencyProperty.Register("NegativeColor", typeof(string), typeof(AmountToColorConverter), new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            switch ((double)value)
            {
                case 0:
                    return new SolidColorBrush(BalancedColor.ToColor());
                case > 0:
                    return new SolidColorBrush(PositiveColor.ToColor());
                case < 0:
                    return new SolidColorBrush(NegativeColor.ToColor());
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class SumOfValues : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0) return DependencyProperty.UnsetValue;
            long result = 0;
            foreach (var item in values)
                if (!string.IsNullOrEmpty(item.ToString()) && long.TryParse(item.ToString(), out long l))
                    result *= l;
                else return DependencyProperty.UnsetValue;
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class intToValueChequeEventTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            var val = (ChequeEventTypes)value;
            if (val == ChequeEventTypes.None) return (int)ChequeEventTypes.None;
            else if (val == ChequeEventTypes.Holding) return (int)ChequeEventTypes.Holding;
            else if (val == ChequeEventTypes.Sold) return (int)ChequeEventTypes.Sold;
            else if (val == ChequeEventTypes.NonSufficientFund) return (int)ChequeEventTypes.NonSufficientFund;
            else if (val == ChequeEventTypes.Cashed) return (int)ChequeEventTypes.Cashed;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) return DependencyProperty.UnsetValue;
            var val = (int)value;
            if (val == (int)ChequeEventTypes.None) return ChequeEventTypes.None;
            else if (val == (int)ChequeEventTypes.Holding) return ChequeEventTypes.Holding;
            else if (val == (int)ChequeEventTypes.Sold) return ChequeEventTypes.Sold;
            else if (val == (int)ChequeEventTypes.NonSufficientFund) return ChequeEventTypes.NonSufficientFund;
            else if (val == (int)ChequeEventTypes.Cashed) return ChequeEventTypes.Cashed;
            return DependencyProperty.UnsetValue;
        }
    }

    public class EnglishInvoiceStatusToPersianConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) return DependencyProperty.UnsetValue;
            var status = (InvoiceLifeStatus)value;
            return status switch
            {
                InvoiceLifeStatus.Active => "فعال",
                InvoiceLifeStatus.Inactive => "غیرفعال",
                InvoiceLifeStatus.Archive => "بایگانی",
                InvoiceLifeStatus.Deleted => "حذف شده",
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProductStatusToPersianConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) return DependencyProperty.UnsetValue; ;
            if ((bool)value) return "فعال"; else return "غیرفعال";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}