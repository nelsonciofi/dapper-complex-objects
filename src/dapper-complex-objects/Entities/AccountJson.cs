using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;
using System.Text.Json;


namespace DapperComplexObjects;

[Table("AccountsJson")]
public class AccountJson
{
    [ExplicitKey] public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public List<InstallmentJson> Installments { get; set; } = new List<InstallmentJson>();
}

public struct InstallmentJson : IEquatable<InstallmentJson>
{
    public DateTime DueDate { get; set; }
    public decimal Value { get; set; }

    public override readonly bool Equals(object? obj)
    {
        return obj is InstallmentJson json && Equals(json);
    }

    public readonly bool Equals(InstallmentJson other)
    {
        return DueDate == other.DueDate &&
               Value == other.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DueDate, Value);
    }

    public static bool operator ==(InstallmentJson left, InstallmentJson right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(InstallmentJson left, InstallmentJson right)
    {
        return !(left == right);
    }
}


public class InstallmentJsonTypeMapper : SqlMapper.TypeHandler<List<InstallmentJson>>
{
    public override List<InstallmentJson> Parse(object value)
    {
        if (value is null) return new List<InstallmentJson>();

        var json = value.ToString();

        if (string.IsNullOrWhiteSpace(json)) return new List<InstallmentJson>();

        var res = JsonSerializer.Deserialize<List<InstallmentJson>>(json);

        if (res is null) return new List<InstallmentJson>();

        return res;
    }

    public override void SetValue(IDbDataParameter parameter, List<InstallmentJson> value)
    {
        parameter.Value = value is null ? null : JsonSerializer.Serialize(value);
    }
}
