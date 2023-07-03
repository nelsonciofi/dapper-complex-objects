using Dapper;
using Dapper.Contrib.Extensions;
using DapperComplexObjects;
using DapperComplexObjects.Infrastructure;

namespace dapper_complex_objects_tests;

public class JsonQueryTests
{
    [Fact]
    public async Task Should_InsertAndRead_Installments_AsJson()
    {
        var sqlConex = await Seed.CreateBaseConnection();
        await sqlConex.ExecuteAsync(SqlConstants.CreateAccountsJson);

        var accounts = Seed.CreateAccounts(1, 3);

        SqlMapper.AddTypeHandler(new InstallmentJsonTypeMapper());
        var accountsSpan = Seed.ConvertJsonAccounts(accounts);
        await sqlConex.InsertAsync(accountsSpan);

        var query = new JsonQuery(sqlConex);

        var res = query.GetAllAccounts();

        var oneAccount = accountsSpan.Single();
        var oneRes = res.Single();

        Assert.Equal(oneAccount.Id, oneRes.Id);
        foreach (var i in oneAccount.Installments)
        {
            var exists = oneRes.Installments.Any(n => n == i);
            Assert.True(exists);
        }

        await sqlConex.ExecuteAsync(SqlConstants.DropAccountsJson);
        await sqlConex.CloseAsync().ConfigureAwait(false);
    }
}
