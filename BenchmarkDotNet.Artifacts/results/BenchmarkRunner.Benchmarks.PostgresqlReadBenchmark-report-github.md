```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-MELAGO : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=3  
Categories=Read  

```
| Method                                      | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|-------------------------------------------- |---------:|---------:|---------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| &#39;SQLC - GetCustomerOrders&#39;                  | 31.30 ms | 0.554 ms | 0.290 ms |  1.00 |    0.00 | 281.2500 | 281.2500 | 125.0000 |   1.48 MB |        1.00 |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 33.08 ms | 0.922 ms | 0.610 ms |  1.06 |    0.02 | 218.7500 |  62.5000 |        - |   1.86 MB |        1.26 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 31.88 ms | 1.631 ms | 0.971 ms |  1.02 |    0.03 | 187.5000 |  62.5000 |        - |   1.84 MB |        1.25 |
