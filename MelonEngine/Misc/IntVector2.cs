using System.Globalization;
using System.Text;

namespace MelonEngine;

public struct IntVector2 : IEquatable<IntVector2>, IFormattable
{
    //i've just learnt why float is fucked up
    #region Public Static Properties
    public static IntVector2 Zero { get { return new IntVector2(); } }
    public static IntVector2 One { get { return new IntVector2(1, 1); } }
    public static IntVector2 UnitX { get { return new IntVector2(1, 0); } }
    public static IntVector2 UnitY { get { return new IntVector2(0, 1); } }
    #endregion Public Static Properties

    public int X { get; set; }
    public int Y { get; set; }

    public IntVector2(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public bool Equals(IntVector2 other)
    {
        return X == other.X && Y == other.Y;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(this.X.ToString(format, formatProvider));
        sb.Append(",");
        sb.Append(this.Y.ToString(format, formatProvider));
        return sb.ToString();
    }

    public static IntVector2 operator+(IntVector2 left, IntVector2 right)
    {
        return new IntVector2(left.X + right.X, left.Y + right.Y);
    }

    public static IntVector2 operator-(IntVector2 left, IntVector2 right)
    {
        return new IntVector2(left.X - right.X, left.Y - right.Y);
    }

    public static IntVector2 operator*(IntVector2 left, IntVector2 right)
    {
        return new IntVector2(left.X * right.X, left.Y * right.Y);
    }

    public static IntVector2 operator/(IntVector2 left, IntVector2 right)
    {
        return new IntVector2(left.X / right.X, left.Y / right.Y);
    }

    public static bool operator==(IntVector2 left, IntVector2 right)
    {
        return left.X == right.X && left.X == right.Y;
    }

    public static bool operator!=(IntVector2 left, IntVector2 right)
    {
        return left.X != right.X || left.X != right.Y;
    }

    public static bool operator>=(IntVector2 left, IntVector2 right)
    {
        return left.X >= right.X && left.X >= right.Y;
    }

    public static bool operator<=(IntVector2 left, IntVector2 right)
    {
        return left.X <= right.X && left.X <= right.Y;
    }

    public static bool operator<(IntVector2 left, IntVector2 right)
    {
        return left.X < right.X && left.X < right.Y;
    }

    public static bool operator>(IntVector2 left, IntVector2 right)
    {
        return left.X > right.X && left.X > right.Y;
    }
}