using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace LazyApiPack.Wpf.Controls.Converters
{
    public class ColorToHexConverter : IValueConverter
    {
        Dictionary<string, Color?> _colors;
        public ColorToHexConverter()
        {
            _colors = new Dictionary<string, Color?>(
                typeof(Colors)
                .GetProperties()
                .Select(p =>
                    new KeyValuePair<string, Color?>(p.Name, (Color?)p.GetValue(null, null)))
                .ToList());

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color c)
            {
                return $"{c}";
            }
            throw new FormatException("The given value is not a color.");
        }

        private bool TryGetColor(string[] parts, out Color result)
        {
            if (parts.Length == 3)
            {
                if (TryGetNumber(parts[0].Trim(), out var r) &&
                    TryGetNumber(parts[1].Trim(), out var g) &&
                    TryGetNumber(parts[2].Trim(), out var b))
                {
                    result = Color.FromRgb(r, g, b);
                    return true;
                }
            }
            else if (parts.Length == 4)
            {
                if (TryGetNumber(parts[0].Trim(), out var a) &&
                   TryGetNumber(parts[1].Trim(), out var r) &&
                   TryGetNumber(parts[2].Trim(), out var g) &&
                   TryGetNumber(parts[3].Trim(), out var b))
                {
                    result = Color.FromArgb(a, r, g, b);
                    return true;
                }
            }
            return false;

        }

        private bool TryGetNumber(string number, out byte result)
        {
            if (!byte.TryParse(number, out result))
            {
                if (!byte.TryParse(number, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result))
                {
                    return false;
                }
            }
            return true;
        }
        private bool TryGetColor(string hexNumber, out Color result)
        {
            if (hexNumber.Length == 8)
            {
                if (byte.TryParse(hexNumber.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var a) &&
                    byte.TryParse(hexNumber.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r) &&
                    byte.TryParse(hexNumber.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) &&
                    byte.TryParse(hexNumber.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
                {
                    result= Color.FromArgb(a, r, g, b);
                    return true;
                }
            }
            else if (hexNumber.Length == 6)
            {
                if (byte.TryParse(hexNumber.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r) &&
                   byte.TryParse(hexNumber.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) &&
                   byte.TryParse(hexNumber.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
                {
                    result= Color.FromRgb(r, g, b);
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && string.IsNullOrWhiteSpace(s))
            {
                byte a, r, g, b;
                // trim trailing and leading delimiters and following spaces
                s = s.Trim().Trim(',', ';').Trim();
                if (!s.StartsWith("#"))
                {
                    // Try (a)rgb with delimiters ,; (a;r;g;b / r;g;b)
                    var delimiters = s.Count(s => s == ',' || s == ';');
                    if (delimiters == 2 || delimiters == 3)
                    {
                        var split = s.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                        if (TryGetColor(split, out var rgbColor))
                        {
                            return rgbColor;
                        }
                    }
                    // Try named colors
                    if (_colors.Any(c => string.Compare(c.Key, s, true) == 0))
                    {
                        return _colors.First(c => string.Compare(c.Key, s, true) == 0);
                    }
                }
                else
                {
                    // Try hex (#aarrggbb #rrggbb)
                    s = s.Trim()
                   .TrimStart('#')
                   .Trim();

                    if (TryGetColor(s, out var hexColor))
                    {
                        return hexColor;
                    }
                }
            }

            throw new FormatException("Enter a valid color (#AARRGGBB, #RRGGBB, aaa,rrr,ggg,bbb, rrr,ggg,bbb, Known Color Name)");


        }
    }
}
