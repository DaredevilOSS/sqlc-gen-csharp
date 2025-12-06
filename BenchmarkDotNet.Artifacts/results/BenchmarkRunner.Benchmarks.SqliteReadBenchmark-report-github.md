```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-MELAGO : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=3  
Categories=Read  

```
| Method                                      | Mean     | Error   | StdDev  | Ratio | Gen0     | Allocated | Alloc Ratio |
|-------------------------------------------- |---------:|--------:|--------:|------:|---------:|----------:|------------:|
| &#39;SQLC - GetCustomerOrders&#39;                  | 135.9 ms | 2.35 ms | 1.40 ms |  1.00 |        - |      2 MB |        1.00 |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 137.9 ms | 0.47 ms | 0.24 ms |  1.01 | 250.0000 |    2.4 MB |        1.20 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 138.3 ms | 0.76 ms | 0.50 ms |  1.02 | 250.0000 |   2.37 MB |        1.19 |
