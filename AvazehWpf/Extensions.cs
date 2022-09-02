using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AvazehWpf
{
    public static class Extensions
    {
        public static string ToHex(this Color c) => $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
        public static Color ToColor(this string s)
        {
            if (int.TryParse(s, out int i))
                return (Color)ColorConverter.ConvertFromString(s);
            return (Color)ColorConverter.ConvertFromString("#00000000");
        }
    }
}