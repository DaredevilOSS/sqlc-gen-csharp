```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-OANQBP : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=3  
Categories=Read  

```
| Method                                      | Mean     | Error    | StdDev   | Ratio | Gen0     | Gen1    | Gen2    | Allocated | Alloc Ratio |
|-------------------------------------------- |---------:|---------:|---------:|------:|---------:|--------:|--------:|----------:|------------:|
| &#39;SQLC - GetCustomerOrders&#39;                  | 17.28 ms | 0.132 ms | 0.079 ms |  1.00 | 187.5000 | 93.7500 | 93.7500 |   1.54 MB |        1.00 |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 19.06 ms | 0.056 ms | 0.033 ms |  1.10 | 250.0000 | 93.7500 |       - |   2.02 MB |        1.31 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 18.86 ms | 0.085 ms | 0.051 ms |  1.09 | 218.7500 | 62.5000 |       - |   1.98 MB |        1.29 |
