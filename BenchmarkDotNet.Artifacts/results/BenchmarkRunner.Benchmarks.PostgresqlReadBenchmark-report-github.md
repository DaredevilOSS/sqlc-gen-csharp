```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-OANQBP : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=3  
Categories=Read  

```
| Method                                      | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0     | Allocated | Alloc Ratio |
|-------------------------------------------- |---------:|--------:|--------:|------:|--------:|---------:|----------:|------------:|
| &#39;SQLC - GetCustomerOrders&#39;                  | 189.5 ms | 2.75 ms | 1.64 ms |  1.00 |    0.00 |        - |   1.48 MB |        1.00 |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 274.2 ms | 3.75 ms | 2.23 ms |  1.45 |    0.02 | 500.0000 |   5.11 MB |        3.45 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 278.3 ms | 3.68 ms | 2.44 ms |  1.47 |    0.02 | 500.0000 |    5.1 MB |        3.44 |
