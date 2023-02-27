using AvazehApiClient.DataAccess;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AvazehWpf;

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

public class ChequeEventToColorConverter : Freezable, IValueConverter
{
    #region Overrides of Freezable    
    protected override Freezable CreateInstanceCore()
    {
        return new ChequeEventToColorConverter();
    }
    #endregion

    public string SoldColor
    {
        get { return (string)GetValue(SoldColorProperty); }
        set { SetValue(SoldColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ItemColor.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SoldColorProperty =
        DependencyProperty.Register("SoldColor", typeof(string), typeof(ChequeEventToColorConverter), new PropertyMetadata(null));

    public string NonSufficientFundColor
    {
        get { return (string)GetValue(NonSufficientFundColorProperty); }
        set { SetValue(NonSufficientFundColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty NonSufficientFundColorProperty =
        DependencyProperty.Register("NonSufficientFundColor", typeof(string), typeof(ChequeEventToColorConverter), new PropertyMetadata(null));

    public string CashedColor
    {
        get { return (string)GetValue(CashedColorProperty); }
        set { SetValue(CashedColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CashedColorProperty =
        DependencyProperty.Register("CashedColor", typeof(string), typeof(ChequeEventToColorConverter), new PropertyMetadata(null));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return DependencyProperty.UnsetValue;
        else if ((string)value == ChequeEventTypes.Sold.ToString())
        {
            if (string.IsNullOrEmpty(SoldColor)) return DependencyProperty.UnsetValue; else return new SolidColorBrush(SoldColor.ToColor());
        }
        else if ((string)value == ChequeEventTypes.NonSufficientFund.ToString())
        {
            if (string.IsNullOrEmpty(NonSufficientFundColor)) return DependencyProperty.UnsetValue; else return new SolidColorBrush(NonSufficientFundColor.ToColor());
        }
        else if ((string)value == ChequeEventTypes.Cashed.ToString())
        {
            if (string.IsNullOrEmpty(CashedColor)) return DependencyProperty.UnsetValue; else return new SolidColorBrush(CashedColor.ToColor());
        }
        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class DateToColorConverter : Freezable, IValueConverter //Used
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
        if (value == null || string.IsNullOrEmpty(ItemColor)) return DependencyProperty.UnsetValue;
        if (((DateTime)value).ToPersianDate() == Date) return new SolidColorBrush(ItemColor.ToColor());
        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

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
                if (string.IsNullOrEmpty(BalancedColor)) return DependencyProperty.UnsetValue; else return new SolidColorBrush(BalancedColor.ToColor());
            case > 0:
                if (string.IsNullOrEmpty(PositiveColor)) return DependencyProperty.UnsetValue; else return new SolidColorBrush(PositiveColor.ToColor());
            case < 0:
                if (string.IsNullOrEmpty(NegativeColor)) return DependencyProperty.UnsetValue; else return new SolidColorBrush(NegativeColor.ToColor());
        }
        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class DueDateToColorConverter : Freezable, IValueConverter
{
    #region Overrides of Freezable    
    protected override Freezable CreateInstanceCore()
    {
        return new DueDateToColorConverter();
    }
    #endregion

    public string NotifColor
    {
        get { return (string)GetValue(NotifColorProperty); }
        set { SetValue(NotifColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ItemColor.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty NotifColorProperty =
        DependencyProperty.Register("NotifColor", typeof(string), typeof(DueDateToColorConverter), new PropertyMetadata(null));

    public int NotifDays
    {
        get { return (int)GetValue(NotifDaysProperty); }
        set { SetValue(NotifDaysProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty NotifDaysProperty =
        DependencyProperty.Register("NotifDays", typeof(int), typeof(DueDateToColorConverter), new PropertyMetadata(2));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || string.IsNullOrEmpty(NotifColor)) return DependencyProperty.UnsetValue;
        var today = PersianCalendarHelper.GetCurrentRawPersianDate();
        var toDate = PersianCalendarHelper.GetRawPersianDate(NotifDays);
        var dueDate = ((string)value).Replace("/", "");
        if (string.Compare(dueDate, toDate) <= 0 && string.Compare(dueDate, today) >= 0) return new SolidColorBrush(NotifColor.ToColor());
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
        if (value == null) return DependencyProperty.UnsetValue;
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
        if (value == null) return DependencyProperty.UnsetValue;
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

public class ProductStatusToPersianConverter : IValueConverter //Used
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return DependencyProperty.UnsetValue;
        if ((bool)value) return "فعال"; else return "غیرفعال";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class NumberToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (int.TryParse(value.ToString(), out int val) == false) return DependencyProperty.UnsetValue;
        if (val != 0) return Visibility.Visible; else return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class DateTimeToPersianDateConverter : IValueConverter //Used
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return DependencyProperty.UnsetValue;
        return ((DateTime)value).ToPersianDate();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class DateTimeToPersianDateTimeConverter : IValueConverter //Used
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return DependencyProperty.UnsetValue;
        return ((DateTime)value).ToPersianDateTime();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class DateTimeToTimeOnlyConverter : IValueConverter //Used
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return DependencyProperty.UnsetValue;
        return ((DateTime)value).ToTime();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}