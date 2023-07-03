// See https://aka.ms/new-console-template for more information
using Dapper;
using Microsoft.Data.SqlClient;

namespace DapperComplexObjects.Infrastructure;

public class SpanQuery
{
    private readonly SqlConnection sqlConnection;

    public SpanQuery(SqlConnection sqlConnection)
    {
        this.sqlConnection = sqlConnection;
    }

    public List<AccountSpan> GetAllAccounts()
    {
        return sqlConnection.Query<AccountSpan>(SqlConstants.SpanQuery).AsList();
    }
}