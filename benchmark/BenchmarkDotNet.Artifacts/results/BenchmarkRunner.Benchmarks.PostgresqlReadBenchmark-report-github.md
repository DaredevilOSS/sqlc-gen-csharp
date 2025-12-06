```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-NFWTJA : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=3  
Categories=Read  

```
| Method                                      | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0     | Allocated | Alloc Ratio |
|-------------------------------------------- |---------:|--------:|--------:|------:|--------:|---------:|----------:|------------:|
| &#39;SQLC - GetCustomerOrders&#39;                  | 184.9 ms | 3.51 ms | 2.09 ms |  1.00 |    0.00 |        - |   1.48 MB |        1.00 |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 271.4 ms | 2.23 ms | 1.48 ms |  1.47 |    0.02 | 500.0000 |   5.12 MB |        3.44 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 273.2 ms | 3.80 ms | 2.26 ms |  1.48 |    0.02 | 500.0000 |   5.11 MB |        3.44 |
