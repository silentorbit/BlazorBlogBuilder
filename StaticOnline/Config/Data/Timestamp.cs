namespace SilentOrbit.StaticOnline.Config.Data;

/// <summary>
/// Replaces <see cref="DateTime"/> with implicit cast from strings.
/// To simplify entry of <see cref="PageData"/> entries.
/// </summary>
public class Timestamp : IComparable<Timestamp>, IEquatable<Timestamp>
{
    public DateTime DateTime { get; init; }

    const string standardFormat = "yyyy-MM-dd'T'HH:mm:ss.fffK";
    public override string ToString() => ToString(standardFormat);
    public string ToString(string format, IFormatProvider? provider = null) => DateTime.ToString(format, provider);

    #region Implicit casts

    //String

    [return: NotNullIfNotNull(nameof(text))]
    public static implicit operator Timestamp?(string? text)
    {
        if (text == null)
            return null;

        return new Timestamp() { DateTime = DateTime.Parse(text) };
    }

    [return: NotNullIfNotNull(nameof(ts))]
    public static implicit operator string?(Timestamp? ts)
    {
        return ts?.DateTime.ToString(standardFormat, CultureInfo.InvariantCulture);
    }

    //DateTime

    [return: NotNullIfNotNull(nameof(date))]
    public static implicit operator Timestamp?(DateTime? date)
    {
        if (date == null)
            return null;

        return new Timestamp() { DateTime = date.Value };
    }

    [return: NotNullIfNotNull(nameof(ts))]
    public static implicit operator DateTime?(Timestamp? ts)
    {
        return ts?.DateTime;
    }

    #endregion

    #region IComparable

    int IComparable<Timestamp>.CompareTo(Timestamp? other)
    {
        if (other is null)
            return 1; // Later than null / null first
        return DateTime.CompareTo(other.DateTime);
    }

    #endregion

    #region Equals

    bool IEquatable<Timestamp>.Equals(Timestamp? other)
    {
        if (other is null)
            return false;
        return DateTime == other.DateTime;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is DateTime dt)
            return DateTime == dt;

        return false;
    }

    public override int GetHashCode()
    {
        return DateTime.GetHashCode();
    }

    public static bool operator ==(Timestamp? left, Timestamp? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.DateTime == right.DateTime;
    }

    public static bool operator !=(Timestamp? left, Timestamp? right)
    {
        if (left is null && right is null) return false;
        if (left is null || right is null) return true;
        return left.DateTime != right.DateTime;
    }

    #endregion

    #region operator < > <= >= == != with Timestamp and DateTime

    public static bool? operator <(Timestamp? left, Timestamp? right)
    {
        if (left is null || right is null) return null;
        return left.DateTime < right.DateTime;
    }
    public static bool? operator >(Timestamp? left, Timestamp? right)
    {
        if (left is null || right is null) return null;
        return left.DateTime > right.DateTime;
    }
    public static bool? operator <=(Timestamp? left, Timestamp? right)
    {
        if (left is null || right is null) return null;
        return left.DateTime <= right.DateTime;
    }
    public static bool? operator >=(Timestamp? left, Timestamp? right)
    {
        if (left is null || right is null) return null;
        return left.DateTime >= right.DateTime;
    }

    #endregion

}
