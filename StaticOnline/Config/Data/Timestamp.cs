namespace SilentOrbit.StaticOnline.Config.Data;

/// <summary>
/// Replaces <see cref="Value"/> with implicit cast from strings.
/// To simplify entry of <see cref="PageData"/> entries.
/// </summary>
public class Timestamp : IComparable<Timestamp>, IEquatable<Timestamp>
{
    public DateTimeOffset Value { get; }

    public Timestamp(DateTimeOffset value)
    {
        Value = value;
    }

    const string standardFormat = "yyyy-MM-dd'T'HH:mm:ss.fffK";
    public override string ToString() => ToString(standardFormat);
    public string ToString(string format, IFormatProvider? provider = null) => Value.ToString(format, provider);

    public string ToRFC3339() 
        => Value.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ssK", 
            CultureInfo.InvariantCulture);

    #region Implicit casts

    //String

    [return: NotNullIfNotNull(nameof(text))]
    public static implicit operator Timestamp?(string? text)
    {
        if (text == null)
            return null;

        var dt = DateTime.Parse(text, CultureInfo.InvariantCulture);
        if (dt.Kind != DateTimeKind.Unspecified)
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
        var dtz = TimeZoneInfo.ConvertTimeToUtc(dt, SiteBuilder.Instance.Config.TimeZone);
        var dto = (DateTimeOffset)dtz;
        var offset = SiteBuilder.Instance.Config.TimeZone.GetUtcOffset(dt);
        var dtoo = dto.ToOffset(offset);
        return new Timestamp(dtoo);
    }

    [return: NotNullIfNotNull(nameof(ts))]
    public static implicit operator string?(Timestamp? ts)
    {
        return ts?.Value.ToString(standardFormat, CultureInfo.InvariantCulture);
    }

    //DateTime

    [return: NotNullIfNotNull(nameof(date))]
    public static implicit operator Timestamp?(DateTime? date)
    {
        if (date == null)
            return null;

        return new Timestamp(date.Value);
    }

    [return: NotNullIfNotNull(nameof(ts))]
    public static implicit operator DateTime?(Timestamp? ts)
    {
        return ts?.Value.DateTime;
    }

    #endregion

    #region IComparable

    int IComparable<Timestamp>.CompareTo(Timestamp? other)
    {
        if (other is null)
            return 1; // Later than null / null first
        return Value.CompareTo(other.Value);
    }

    #endregion

    #region Equals

    bool IEquatable<Timestamp>.Equals(Timestamp? other)
    {
        if (other is null)
            return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is DateTime dt)
            return Value == dt;

        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(Timestamp? left, Timestamp? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Value == right.Value;
    }

    public static bool operator !=(Timestamp? left, Timestamp? right)
    {
        if (left is null && right is null) return false;
        if (left is null || right is null) return true;
        return left.Value != right.Value;
    }

    #endregion

    #region operator < > <= >= == != with Timestamp and DateTime

    public static bool? operator <(Timestamp? left, Timestamp? right)
    {
        if (left is null || right is null) return null;
        return left.Value < right.Value;
    }
    public static bool? operator >(Timestamp? left, Timestamp? right)
    {
        if (left is null || right is null) return null;
        return left.Value > right.Value;
    }
    public static bool? operator <=(Timestamp? left, Timestamp? right)
    {
        if (left is null || right is null) return null;
        return left.Value <= right.Value;
    }
    public static bool? operator >=(Timestamp? left, Timestamp? right)
    {
        if (left is null || right is null) return null;
        return left.Value >= right.Value;
    }

    #endregion

}
