```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-NFWTJA : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=3  
Categories=Read  

```
| Method                                      | Mean     | Error    | StdDev   | Ratio | Gen0     | Gen1     | Gen2    | Allocated | Alloc Ratio |
|-------------------------------------------- |---------:|---------:|---------:|------:|---------:|---------:|--------:|----------:|------------:|
| &#39;SQLC - GetCustomerOrders&#39;                  | 17.36 ms | 0.088 ms | 0.058 ms |  1.00 | 187.5000 |  93.7500 | 93.7500 |   1.54 MB |        1.00 |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | 19.31 ms | 0.077 ms | 0.051 ms |  1.11 | 250.0000 | 125.0000 |       - |   2.01 MB |        1.30 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | 18.97 ms | 0.046 ms | 0.024 ms |  1.09 | 218.7500 |  62.5000 |       - |   1.98 MB |        1.29 |
