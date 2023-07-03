using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;
using System.Runtime.InteropServices;

namespace DapperComplexObjects;

[Table("AccountsSpan")]
public class AccountSpan
{
    [ExplicitKey] public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public List<InstallmentSpan> Installments { get; set; } = new List<InstallmentSpan>();
}


public struct InstallmentSpan : IEquatable<InstallmentSpan>
{
    public DateTime DueDate { get; set; }
    public decimal Value { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is InstallmentSpan span && Equals(span);
    }

    public bool Equals(InstallmentSpan other)
    {
        return DueDate == other.DueDate &&
               Value == other.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DueDate, Value);
    }

    public static bool operator ==(InstallmentSpan left, InstallmentSpan right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(InstallmentSpan left, InstallmentSpan right)
    {
        return !(left == right);
    }
}

public class InstallmentSpanTypeMapper 
    : SqlMapper.TypeHandler<List<InstallmentSpan>>
{
    public override List<InstallmentSpan> Parse(object value)
    {
        if (value is not byte[] bytes)
        {
            return new List<InstallmentSpan>();
        }

        var span = bytes.AsSpan();
        var structSpan = MemoryMarshal.Cast<byte, InstallmentSpan>(span);        
        return  structSpan.ToArray().ToList();       
    }

    public override void SetValue(IDbDataParameter parameter,
                                  List<InstallmentSpan> value)
    {
        var s = CollectionsMarshal.AsSpan(value);

        Span<byte> span = MemoryMarshal.AsBytes(s);

        parameter.Value = span.ToArray();
    }
}
