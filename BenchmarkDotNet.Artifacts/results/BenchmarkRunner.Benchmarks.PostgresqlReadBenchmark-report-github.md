```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-GPQXQZ : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=3  
Categories=Read  

```
| Method                       | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|----------------------------- |---------:|---------:|---------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| &#39;SQLC - GetCustomerOrders&#39;   | 30.66 ms | 0.795 ms | 0.473 ms |  1.00 |    0.00 | 437.5000 | 437.5000 | 125.0000 |   1.48 MB |        1.00 |
| &#39;EFCore - GetCustomerOrders&#39; | 33.28 ms | 1.193 ms | 0.710 ms |  1.09 |    0.02 | 200.0000 |  66.6667 |        - |   1.86 MB |        1.25 |
