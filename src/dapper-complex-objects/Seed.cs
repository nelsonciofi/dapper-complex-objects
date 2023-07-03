using AutoBogus;
using Dapper;
using Dapper.Contrib.Extensions;
using DapperComplexObjects.Infrastructure;
using Microsoft.Data.SqlClient;

namespace DapperComplexObjects;

public static class Seed
{
    public static async Task<SqlConnection> CreateBaseConnection()
    {
        var sqlConex = new SqlConnection(Environment.GetEnvironmentVariable("NelsonBlogs"));
        await sqlConex.OpenAsync().ConfigureAwait(false);
        return sqlConex;
    }


    public static async Task<SqlConnection> CreateSeededConnection(int accountsAmount, int installmentsPerAccountAmount) 
    {
        var sqlConex = await CreateBaseConnection().ConfigureAwait(false);

        await sqlConex.ExecuteAsync(SqlConstants.CreateAccounts);
        await sqlConex.ExecuteAsync(SqlConstants.CreateInstallments);
        await sqlConex.ExecuteAsync(SqlConstants.CreateAccountsJson);
        await sqlConex.ExecuteAsync(SqlConstants.CreateAccountsSpan);

        var accounts = CreateAccounts(accountsAmount, installmentsPerAccountAmount);
        await sqlConex.InsertAsync(accounts);
        await sqlConex.InsertAsync(accounts.SelectMany(a => a.Installments));

        SqlMapper.AddTypeHandler(new InstallmentJsonTypeMapper());
        var accountsJson = ConvertJsonAccounts(accounts);
        await sqlConex.InsertAsync(accountsJson);

        SqlMapper.AddTypeHandler(new InstallmentSpanTypeMapper());
        var accountsBinary = ConvertSpanAccounts(accounts);
        await sqlConex.InsertAsync(accountsBinary);

        return sqlConex;
    }

    public static async Task FinalizeConnection(this SqlConnection sqlConex)
    {
        await sqlConex.ExecuteAsync(SqlConstants.DropAccountAndInstallments);
        await sqlConex.ExecuteAsync(SqlConstants.DropAccountsJson);
        await sqlConex.ExecuteAsync(SqlConstants.DropAccountsSpan);

        await sqlConex.CloseAsync().ConfigureAwait(false);
    }

    public static IEnumerable<Account> CreateAccounts(int amount, int installments)
    {
        var installmentFaker = new AutoFaker<Installment>();
        var accFaker = new AutoFaker<Account>().Ignore(a => a.Installments)                                                                                              
                                               .RuleFor(a => a.TotalValue, f => f.Finance.Amount(0, 10, 2));

        var accounts = accFaker.Generate(amount);
        accounts.ForEach(a => a.Installments = installmentFaker.RuleFor(i => i.AccountId, () => a.Id)
                                                               .RuleFor(i => i.Value, f => f.Finance.Amount(0, 10, 2)) 
                                                               .Generate(installments));
        return accounts;
    }

    public static IEnumerable<AccountJson> ConvertJsonAccounts(IEnumerable<Account> accounts)
    {
        if (accounts is null) yield break;

        foreach (var account in accounts)
        {
            var accJson = new AccountJson
            {
                Description = account.Description,
                Id = account.Id,
                TotalValue = account.TotalValue,
            };

            if (account.Installments is not null && account.Installments.Count > 0)
            {
                foreach (var installment in account.Installments)
                {
                    accJson.Installments.Add(new InstallmentJson
                    {
                        DueDate = installment.DueDate,
                        Value = installment.Value,
                    });
                }
            }

            yield return accJson;
        }
    }


    public static IEnumerable<AccountSpan> ConvertSpanAccounts(IEnumerable<Account> accounts)
    {
        if (accounts is null) yield break;

        foreach (var account in accounts)
        {
            var accJson = new AccountSpan
            {
                Description = account.Description,
                Id = account.Id,
                TotalValue = account.TotalValue,
            };

            if (account.Installments is not null && account.Installments.Count > 0)
            {
                foreach (var installment in account.Installments)
                {
                    accJson.Installments.Add(new InstallmentSpan
                    {
                        DueDate = installment.DueDate,
                        Value = installment.Value,
                    });
                }
            }

            yield return accJson;
        }
    }
}
