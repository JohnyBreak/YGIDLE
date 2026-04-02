using System;

namespace Game.BigNumberSpace
{
    [Serializable]
    public struct BigNumber : IComparable<BigNumber>
    {
        public double Mantissa; // от 1 до 10 (или -1 до -10)
        public int Exponent;

        private const double EPSILON = 1e-12;

        #region Constructors

        public BigNumber(double value)
        {
            if (Math.Abs(value) < EPSILON)
            {
                Mantissa = 0;
                Exponent = 0;
                return;
            }

            Exponent = (int)Math.Floor(Math.Log10(Math.Abs(value)));
            Mantissa = value / Math.Pow(10, Exponent);

            Normalize();
        }

        public BigNumber(double mantissa, int exponent)
        {
            Mantissa = mantissa;
            Exponent = exponent;
            Normalize();
        }

        #endregion

        #region Normalize

        private void Normalize()
        {
            if (Math.Abs(Mantissa) < EPSILON)
            {
                Mantissa = 0;
                Exponent = 0;
                return;
            }

            double sign = Math.Sign(Mantissa);
            Mantissa = Math.Abs(Mantissa);

            while (Mantissa >= 10)
            {
                Mantissa /= 10;
                Exponent++;
            }

            while (Mantissa < 1)
            {
                Mantissa *= 10;
                Exponent--;
            }

            Mantissa *= sign;

            // ограничение точности (важно для idle)
            Mantissa = Math.Round(Mantissa, 6);
        }

        #endregion

        #region Operators

        public static BigNumber operator +(BigNumber a, BigNumber b)
        {
            if (a.Mantissa == 0) return b;
            if (b.Mantissa == 0) return a;

            int diff = a.Exponent - b.Exponent;

            if (diff > 15) return a;
            if (diff < -15) return b;

            if (diff >= 0)
            {
                return new BigNumber(
                    a.Mantissa + b.Mantissa / Math.Pow(10, diff),
                    a.Exponent
                );
            }
            else
            {
                return new BigNumber(
                    a.Mantissa / Math.Pow(10, -diff) + b.Mantissa,
                    b.Exponent
                );
            }
        }

        public static BigNumber operator -(BigNumber a, BigNumber b)
        {
            return a + b.Negate();
        }

        public static BigNumber operator *(BigNumber a, BigNumber b)
        {
            return new BigNumber(
                a.Mantissa * b.Mantissa,
                a.Exponent + b.Exponent
            );
        }

        public static BigNumber operator /(BigNumber a, BigNumber b)
        {
            return new BigNumber(
                a.Mantissa / b.Mantissa,
                a.Exponent - b.Exponent
            );
        }

        #endregion

        #region Utility

        public BigNumber Negate()
        {
            return new BigNumber(-Mantissa, Exponent);
        }

        public static BigNumber Max(BigNumber a, BigNumber b)
        {
            return a.CompareTo(b) >= 0 ? a : b;
        }

        public static BigNumber Min(BigNumber a, BigNumber b)
        {
            return a.CompareTo(b) <= 0 ? a : b;
        }

        public BigNumber ClampToZero()
        {
            return CompareTo(new BigNumber(0)) < 0 ? new BigNumber(0) : this;
        }

        #endregion

        #region Compare

        public int CompareTo(BigNumber other)
        {
            if (Mantissa == 0 && other.Mantissa == 0) return 0;

            if (Exponent != other.Exponent)
                return Exponent.CompareTo(other.Exponent);

            return Mantissa.CompareTo(other.Mantissa);
        }

        public static bool operator >(BigNumber a, BigNumber b) => a.CompareTo(b) > 0;
        public static bool operator <(BigNumber a, BigNumber b) => a.CompareTo(b) < 0;
        public static bool operator >=(BigNumber a, BigNumber b) => a.CompareTo(b) >= 0;
        public static bool operator <=(BigNumber a, BigNumber b) => a.CompareTo(b) <= 0;

        #endregion

        #region Formatting

        private static readonly string[] SUFFIXES =
        {
            "", "K", "M", "B", "T", "AA", "AB", "AC", "AD", "AE"
        };

        public string ToShortString()
        {
            if (Mantissa == 0) return "0";

            if (Exponent < 3)
            {
                double full = Mantissa * Math.Pow(10, Exponent);
                return FormatSmall(full);
            }

            int group = Exponent / 3;
            int remainder = Exponent % 3;

            double value = Mantissa * Math.Pow(10, remainder);

            string suffix = group < SUFFIXES.Length
                ? SUFFIXES[group]
                : "e" + Exponent;

            return FormatCompact(value) + suffix;
        }

        private string FormatSmall(double value)
        {
            string str = value.ToString("F2");
            return TrimZeros(str);
        }

        private string FormatCompact(double value)
        {
            string format =
                value >= 100 ? "F0" :
                value >= 10 ? "F1" : "F2";

            return TrimZeros(value.ToString(format));
        }

        private string TrimZeros(string str)
        {
            if (!str.Contains(".")) return str;
            return str.TrimEnd('0').TrimEnd('.');
        }

        #endregion

        #region Debug

        public override string ToString()
        {
            return $"{Mantissa}e{Exponent}";
        }

        #endregion
    }
}