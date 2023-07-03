// See https://aka.ms/new-console-template for more information
using Dapper;
using Microsoft.Data.SqlClient;

namespace DapperComplexObjects.Infrastructure;

public class ClassicQuery
{
    private readonly SqlConnection sqlConnection;

    public ClassicQuery(SqlConnection sqlConnection)
    {
        this.sqlConnection = sqlConnection;
    }

    public List<Account> GetAllAccounts()
    {
        var lookup = new Dictionary<Guid, Account>();

        _ = sqlConnection.Query<Account, Installment, Account>(SqlConstants.ClassicQuery,
            (acc, ins) =>
            {
                if (!lookup.TryGetValue(acc.Id, out var accEntry))
                {
                    accEntry = acc;
                    accEntry.Installments ??= new List<Installment>();
                    lookup.Add(acc.Id, accEntry);
                }

                accEntry.Installments.Add(ins);
                return accEntry;

            }, splitOn: "Id");

        return lookup.Values.AsList();
    }
}
