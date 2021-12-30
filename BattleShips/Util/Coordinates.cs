using System;
using System.Linq;

namespace BattleShips.Util
{
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

    public static class CoordinatesLetterNumber
    {
        public static string IntToAlphas(int i)
        {
            if (i < 0) throw new ArgumentException();
            if (i < 26) return ((char)('A' + i)).ToString();
            return IntToAlphas((i / 26) - 1) + IntToAlphas(i % 26);
        }

        public static int AlphasToInt(string s)
            => s.Select(AlphaToInt).Aggregate((v, n) => (v + 1) * 26 + n);

        public static int AlphaToInt(char c)
            => (c - 'A');

        public static bool ValidAlphaKey(ConsoleKey key)
            => key >= ConsoleKey.A && key <= ConsoleKey.Z;
        public static int AlphaToInt(ConsoleKey key)
        {
            if (!ValidAlphaKey(key)) throw new ArgumentException();
            return key - ConsoleKey.A;
        }

        public static string IntToDigits(int i)
            => (i + 1).ToString();

        public static bool ValidDigitKey(ConsoleKey key)
            => key >= ConsoleKey.D0 && key <= ConsoleKey.D9;
        public static int DigitToInt(ConsoleKey key)
        {
            if (!ValidDigitKey(key)) throw new ArgumentException();
            if (key == ConsoleKey.D0) return 9;  // use 0 as 10, which is index 9
            return (key - ConsoleKey.D1);
        }

        public static string ToLetterNumber(this Coordinates coordinates)
            => IntToAlphas(coordinates.X) + coordinates.Y.ToString();
    }
}