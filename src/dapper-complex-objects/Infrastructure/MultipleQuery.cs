// See https://aka.ms/new-console-template for more information
using Dapper;
using Microsoft.Data.SqlClient;

namespace DapperComplexObjects.Infrastructure;

public class MultipleQuery
{
    private readonly SqlConnection sqlConnection;

    public MultipleQuery(SqlConnection sqlConnection)
    {
        this.sqlConnection = sqlConnection;
    }

    public List<Account> GetAllAccounts()
    {
        var res = sqlConnection.QueryMultiple(SqlConstants.MultipleQuery);

        var acc = res.Read<Account>();
        var installments = res.Read<Installment>();

        foreach (var account in acc)
        {
            account.Installments = installments.Where(i => i.AccountId == account.Id).ToList();
        }

        return acc.AsList();
    }
}
