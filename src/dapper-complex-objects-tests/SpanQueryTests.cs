using Dapper;
using Dapper.Contrib.Extensions;
using DapperComplexObjects;
using DapperComplexObjects.Infrastructure;

namespace dapper_complex_objects_tests;

public class SpanQueryTests
{
    [Fact]
    public async Task Should_InsertAndRead_Installments_AsSpans()
    {
        var sqlConex = await Seed.CreateBaseConnection();
        await sqlConex.ExecuteAsync(SqlConstants.CreateAccountsSpan);

        var accounts = Seed.CreateAccounts(1, 3);

        SqlMapper.AddTypeHandler(new InstallmentSpanTypeMapper());
        var accountsSpan = Seed.ConvertSpanAccounts(accounts);
        await sqlConex.InsertAsync(accountsSpan);

        var query = new SpanQuery(sqlConex);

        var res = query.GetAllAccounts();

        var oneAccount = accountsSpan.Single();
        var oneRes = res.Single();

        Assert.Equal(oneAccount.Id, oneRes.Id);
        foreach (var i in oneAccount.Installments)
        {
            var exists = oneRes.Installments.Any(n => n == i);
            Assert.True(exists);
        }

        await sqlConex.ExecuteAsync(SqlConstants.DropAccountsSpan);
        await sqlConex.CloseAsync().ConfigureAwait(false);
    }
}
