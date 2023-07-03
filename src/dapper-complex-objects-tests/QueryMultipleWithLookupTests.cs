using Dapper;
using Dapper.Contrib.Extensions;
using DapperComplexObjects;
using DapperComplexObjects.Infrastructure;

namespace dapper_complex_objects_tests;

public class QueryMultipleWithLookupTests
{
    [Fact]
    public async Task Should_InsertAndRead_Installments_FromQueryMultiple_AndMapUsingLookup()
    {
        var sqlConex = await Seed.CreateBaseConnection();
        await sqlConex.ExecuteAsync(SqlConstants.CreateAccounts);
        await sqlConex.ExecuteAsync(SqlConstants.CreateInstallments);

        var accounts = Seed.CreateAccounts(1, 3);
        await sqlConex.InsertAsync(accounts);
        await sqlConex.InsertAsync(accounts.SelectMany(a => a.Installments));

        var query = new MultipleQueryWithLookup(sqlConex);

        var res = query.GetAllAccounts();

        var oneAccount = accounts.Single();
        var oneRes = res.Single();

        Assert.Equal(oneAccount.Id, oneRes.Id);
        foreach (var i in oneAccount.Installments)
        {
            var exists = oneRes.Installments.Any(n => n.Id == i.Id);
            Assert.True(exists);
        }

        await sqlConex.ExecuteAsync(SqlConstants.DropAccountAndInstallments);
        await sqlConex.CloseAsync().ConfigureAwait(false);
    }
}