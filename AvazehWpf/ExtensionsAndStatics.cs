using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AvazehWpf
{
    public static class ExtensionsAndStatics
    {
        public static string ToHex(this Color c) => $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
        public static Color ToColor(this string s)
        {
            if (long.TryParse(s.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
                return (Color)ColorConverter.ConvertFromString(s);
            return Colors.Transparent;
        }

        public static void ChangeLanguageToPersian()
        {
            if (InputLanguageManager.Current.AvailableInputLanguages.Cast<CultureInfo>().Where(x => x.Name == "fa-IR").Any())
                InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("fa-IR");
            else MessageBox.Show("زبان ورودی فارسی نصب نشده است. یا آن را نصب کنید یا از قسمت تنظیمات گزینه انتخاب خودکار زبان فارسی رو غیرفعال نمایید.", "زبان فارسی", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}