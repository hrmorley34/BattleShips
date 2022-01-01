using System;
using System.Linq;

namespace BattleShips.Util
{
    /// <summary>Represents a 2D vector</summary>
    public class Vector : IEquatable<Vector>
    {
        public readonly int X;
        public readonly int Y;

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector operator +(Vector a, Vector b)
            => new Vector(a.X + b.X, a.Y + b.Y);
        public static Vector operator -(Vector a, Vector b)
            => new Vector(a.X - b.X, a.Y - b.Y);
        public static Vector operator -(Vector v)
            => new Vector(-v.X, -v.Y);
        public static Vector operator *(Vector a, int b)
            => new Vector(a.X * b, a.Y * b);

        #region Equality
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals((Vector)obj);
        }
        public bool Equals(Vector? obj)
            => !ReferenceEquals(obj, null) && X == obj.X && Y == obj.Y;

        public override int GetHashCode()
            => (X, Y).GetHashCode();

        public static bool operator ==(Vector? a, Vector? b)
            => ReferenceEquals(a, b) || (!ReferenceEquals(a, null) && a.Equals(b));
        public static bool operator !=(Vector? a, Vector? b)
            => !ReferenceEquals(a, b) && (ReferenceEquals(a, null) || !a.Equals(b));
        #endregion

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }
    }

    /// <summary>Represents a 2D absolute position</summary>
    public class Coordinates : Vector
    {
        public Coordinates(int x, int y) : base(x, y) { }

        public static Coordinates operator +(Coordinates a, Vector b)
            => new Coordinates(a.X + b.X, a.Y + b.Y);
        public static Coordinates operator +(Vector a, Coordinates b)
            => new Coordinates(a.X + b.X, a.Y + b.Y);
        public static Coordinates operator -(Coordinates a, Vector b)
            => new Coordinates(a.X - b.X, a.Y - b.Y);

        public static readonly Coordinates Origin = new Coordinates(0, 0);
    }

    /// <summary>Converters for coordinates</summary>
    public static class CoordinatesLetterNumber
    {
        /// <summary>Convert a column index into a letter (0 => A)</summary>
        public static string IntToAlphas(int i)
        {
            if (i < 0) throw new ArgumentException();
            if (i < 26) return ((char)('A' + i)).ToString();
            return IntToAlphas((i / 26) - 1) + IntToAlphas(i % 26);
        }

        /// <summary>Convert letters into a column index (A => 0, AA => 26)</summary>
        public static int AlphasToInt(string s)
            => s.Select(AlphaToInt).Aggregate((v, n) => (v + 1) * 26 + n);

        /// <summary>Convert a letter into a column index (A => 0)</summary>
        public static int AlphaToInt(char c)
        {
            if ('A' <= c && c <= 'Z')
                return (c - 'A');
            else if ('a' <= c && c <= 'z')
                return (c - 'a');
            else
                throw new ArgumentException();
        }

        /// <summary>Check if a key can be converted into a column index</summary>
        public static bool ValidAlphaKey(ConsoleKey key)
            => key >= ConsoleKey.A && key <= ConsoleKey.Z;
        /// <summary>Convert a letter key into a colum index (A => 0)</summary>
        public static int AlphaToInt(ConsoleKey key)
        {
            if (!ValidAlphaKey(key)) throw new ArgumentException();
            return key - ConsoleKey.A;
        }

        /// <summary>Convert a row index into a row number (0 => 1)</summary>
        public static string IntToDigits(int i)
            => (i + 1).ToString();

        /// <summary>Check if a key can be converted into a row index</summary>
        public static bool ValidDigitKey(ConsoleKey key)
            => key >= ConsoleKey.D0 && key <= ConsoleKey.D9;
        /// <summary>Convert a number key into a row index (1 => 0)</summary>
        public static int DigitToInt(ConsoleKey key)
        {
            if (!ValidDigitKey(key)) throw new ArgumentException();
            if (key == ConsoleKey.D0) return 9;  // use 0 as 10, which is index 9
            return (key - ConsoleKey.D1);
        }

        /// <summary>Convert coordinates into a letters-numbers pair</summary>
        public static string ToLetterNumber(this Coordinates coordinates)
            => IntToAlphas(coordinates.X) + IntToDigits(coordinates.Y);
    }
}