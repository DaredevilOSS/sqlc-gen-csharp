```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-OANQBP : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=3  
Categories=Read  

```
| Method                                      | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|-------------------------------------------- |---------:|----------:|----------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| &#39;SQLC - GetCustomerOrders&#39;                  | 5.627 ms | 0.0588 ms | 0.0350 ms |  1.00 |    0.00 | 546.8750 | 546.8750 | 156.2500 |   1.77 MB |        1.00 |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 6.246 ms | 0.0437 ms | 0.0289 ms |  1.11 |    0.01 | 203.1250 |  70.3125 |        - |   1.68 MB |        0.95 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 6.335 ms | 0.2516 ms | 0.1497 ms |  1.13 |    0.02 | 203.1250 |  78.1250 |        - |   1.67 MB |        0.94 |
