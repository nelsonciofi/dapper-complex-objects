using BenchmarkDotNet.Running;
using DapperComplexObjects;

Console.WriteLine("The best way to store and retrieve complex objects with dapper.");

await Seed.CreateSeededConnection(1,1);

BenchmarkRunner.Run<BenchmarkCases>();

Console.ReadLine();





