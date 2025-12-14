```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host] : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                     | ConcurrentQueries | QueriesToRun | CustomerCount | Limit | Mean | Error | Ratio | RatioSD | Alloc Ratio |
|--------------------------- |------------------ |------------- |-------------- |------ |-----:|------:|------:|--------:|------------:|
| &#39;SQLC - GetCustomerOrders&#39; | 25                | 1000         | 500           | 100   |   NA |    NA |     ? |       ? |           ? |

Benchmarks with issues:
  PostgresqlReadBenchmark.'SQLC - GetCustomerOrders': Job-BKXKPP(Runtime=.NET 8.0, IterationCount=10, WarmupCount=2) [ConcurrentQueries=25, QueriesToRun=1000, CustomerCount=500, Limit=100]
