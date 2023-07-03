// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using DapperComplexObjects.Infrastructure;
using Microsoft.Data.SqlClient;

namespace DapperComplexObjects;


[MemoryDiagnoser(false)]
[HideColumns(Column.Error, Column.StdDev, Column.Median)]
public class BenchmarkCases
{
    private SqlConnection sqlConnection;
    private ClassicQuery classic;
    private MultipleQuery multiple;
    private MultipleQueryWithLookup multipleQueryWithLookup;
    private JsonQuery json;
    private SpanQuery spans;

    // [Params(1, 10, 100, 1000, 10000)]   
    [Params(1, 10, 100)]
    public int Accounts { get; set; }

    //[Params(1, 10, 100)]
    [Params(10, 100)]
    public int Installments { get; set; }


    [GlobalSetup]
    public async Task Initialize()
    {
        sqlConnection = await Seed.CreateSeededConnection(Accounts, Installments);

        classic = new ClassicQuery(sqlConnection);
        multiple = new MultipleQuery(sqlConnection);
        json = new JsonQuery(sqlConnection);
        spans = new SpanQuery(sqlConnection);
        multipleQueryWithLookup = new MultipleQueryWithLookup(sqlConnection);
    }

    [GlobalCleanup]
    public async Task Finish()
    {
        await sqlConnection.FinalizeConnection();
    }


    /*
    [Benchmark]
    public List<Account> ClassicQuery() => classic.GetAllAccounts();*/

    /*
    [Benchmark]
    public List<Account> MultipleQuery() => multiple.GetAllAccounts();*/

    [Benchmark]
    public List<AccountJson> JsonQuery() => json.GetAllAccounts();

    [Benchmark]
    public List<AccountSpan> SpanQuery() => spans.GetAllAccounts();

    [Benchmark]
    public List<Account> MultipleLookup() => multipleQueryWithLookup.GetAllAccounts();

}
