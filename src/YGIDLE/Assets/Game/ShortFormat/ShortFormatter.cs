using System;
using System.Globalization;
using Game.BigNumberSpace;

namespace Game.ShortFormat
{
    public static class ShortFormatter
    {
        private static readonly string[] SUFFIXES =
        {
            "", "K", "M", "B", "T", "AA", "AB", "AC"
        };

        #region Public API

        public static string Format(float value) => Format((double)value);

        public static string Format(int value) => Format((double)value);

        public static string Format(long value) => Format((double)value);
        
        public static string Format(BigNumber value)
        {
            return value.ToShortString();
        }
        public static string Format(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return "0";

            bool isNegative = value < 0;
            value = Math.Abs(value);

            string result;

            if (value < 1000d)
            {
                result = FormatSmall(value);
            }
            else
            {
                result = FormatWithSuffix(value);
            }

            return isNegative ? "-" + result : result;
        }

        #endregion

        #region Core Logic

        private static string FormatWithSuffix(double value)
        {
            int suffixIndex = 0;

            while (value >= 1000d && suffixIndex < SUFFIXES.Length - 1)
            {
                value /= 1000d;
                suffixIndex++;
            }

            // 🔥 фикс округления (например 999.9 → 1000)
            if (value >= 999.5 && suffixIndex < SUFFIXES.Length - 1)
            {
                value /= 1000d;
                suffixIndex++;
            }

            string formatted = FormatCompact(value);

            return formatted + SUFFIXES[suffixIndex];
        }

        private static string FormatSmall(double value)
        {
            return TrimZeros(value.ToString("F2", CultureInfo.InvariantCulture));
        }

        private static string FormatCompact(double value)
        {
            string format =
                value >= 100 ? "F0" :
                value >= 10 ? "F1" : "F2";

            return TrimZeros(value.ToString(format, CultureInfo.InvariantCulture));
        }

        private static string TrimZeros(string str)
        {
            if (!str.Contains(".")) return str;
            return str.TrimEnd('0').TrimEnd('.');
        }

        #endregion
    }
}