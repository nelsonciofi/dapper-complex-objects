

using Dapper.Contrib.Extensions;

namespace DapperComplexObjects;

[Table("Accounts")]
public class Account
{
   [ExplicitKey] public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    [Computed]public List<Installment> Installments { get; set; } = new List<Installment>();
}

[Table("Installments")]
public class Installment
{
    [ExplicitKey] public Guid Id { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Value { get; set; }
    public Guid AccountId { get; set; }
}

