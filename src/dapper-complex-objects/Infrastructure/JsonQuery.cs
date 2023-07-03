// See https://aka.ms/new-console-template for more information
using Dapper;
using Microsoft.Data.SqlClient;

namespace DapperComplexObjects.Infrastructure;

public class JsonQuery
{
    private readonly SqlConnection sqlConnection;

    public JsonQuery(SqlConnection sqlConnection)
    {
        this.sqlConnection = sqlConnection;
    }

    public List<AccountJson> GetAllAccounts()
    {
        return sqlConnection.Query<AccountJson>(SqlConstants.JsonQuery).AsList();
    }
}
