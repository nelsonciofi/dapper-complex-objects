// See https://aka.ms/new-console-template for more information
using Dapper;
using Microsoft.Data.SqlClient;

namespace DapperComplexObjects.Infrastructure;

public class MultipleQueryWithLookup
{
    private readonly SqlConnection sqlConnection;

    public MultipleQueryWithLookup(SqlConnection sqlConnection)
    {
        this.sqlConnection = sqlConnection;
    }

    public List<Account> GetAllAccounts()
    {
        var res = sqlConnection.QueryMultiple(SqlConstants.MultipleQuery);

        var accounts = res.Read<Account>(buffered: true).AsList();
        var installments = res.Read<Installment>();

        var lookup = new Dictionary<Guid, int>();

        for (int i = 0; i < accounts.Count; i++)
        {
            lookup.Add(accounts[i].Id, i);
        }

        foreach (var installment in installments)
        {
            if (lookup.TryGetValue(installment.AccountId, out int i))
            {
                accounts[i].Installments.Add(installment);
            }
        }

        return accounts;
    }
}