using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.SettingsModels;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using SharedLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

    public class EnglishToPersianChequeEventTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            else if ((string)value == ChequeEventTypes.None.ToString()) return "عادی";
            else if ((string)value == ChequeEventTypes.Holding.ToString()) return "عادی";
            else if ((string)value == ChequeEventTypes.Sold.ToString()) return "منتقل شده";
            else if ((string)value == ChequeEventTypes.NonSufficientFund.ToString()) return "برگشت خورده";
            else if ((string)value == ChequeEventTypes.Cashed.ToString()) return "وصول شده";
            else return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
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

    public class DateToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 3 || values[0] == null || values[2] == null) return DependencyProperty.UnsetValue;
            if ((string)values[0] == (string)values[1]) return new SolidColorBrush(((string)values[2]).ToColor());
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    }

    public class AmountToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 4 || values[0] == null) return DependencyProperty.UnsetValue;
            switch ((double)values[0])
            {
                case 0:
                    if (values[1] is not string || string.IsNullOrEmpty((string)values[1])) return DependencyProperty.UnsetValue;
                    return new SolidColorBrush(((string)values[1]).ToColor());
                case > 0:
                    if (values[2] is not string || string.IsNullOrEmpty((string)values[2])) return DependencyProperty.UnsetValue;
                    return new SolidColorBrush(((string)values[2]).ToColor());
                case < 0:
                    if (values[3] is not string || string.IsNullOrEmpty((string)values[3])) return DependencyProperty.UnsetValue;
                    return new SolidColorBrush(((string)values[3]).ToColor());
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
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
}