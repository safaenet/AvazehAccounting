﻿using AvazehApiClient.DataAccess;
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

    public class EnglishToPersianFinStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            if ((InvoiceFinancialStatus)value == InvoiceFinancialStatus.Balanced) return "تسویه";
            if ((InvoiceFinancialStatus)value == InvoiceFinancialStatus.Deptor) return "بدهکار";
            return "بستانکار";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            if ((string)value == "صعودی") return OrderType.ASC;
            return OrderType.DESC;
        }
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
                    if (string.IsNullOrEmpty((string)values[1])) return DependencyProperty.UnsetValue;
                    return new SolidColorBrush(((string)values[1]).ToColor());
                case > 0:
                    if (string.IsNullOrEmpty((string)values[2])) return DependencyProperty.UnsetValue;
                    return new SolidColorBrush(((string)values[2]).ToColor());
                case < 0:
                    if (string.IsNullOrEmpty((string)values[3])) return DependencyProperty.UnsetValue;
                    return new SolidColorBrush(((string)values[3]).ToColor());
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    }
}