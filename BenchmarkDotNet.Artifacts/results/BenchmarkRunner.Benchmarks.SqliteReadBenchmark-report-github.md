```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-VQDPTZ : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=3  
Categories=Read  

```
| Method                       | Mean     | Error   | StdDev  | Ratio | Gen0     | Allocated | Alloc Ratio |
|----------------------------- |---------:|--------:|--------:|------:|---------:|----------:|------------:|
| &#39;SQLC - GetCustomerOrders&#39;   | 135.0 ms | 1.79 ms | 1.19 ms |  1.00 |        - |      2 MB |        1.00 |
| &#39;EFCore - GetCustomerOrders&#39; | 136.7 ms | 1.18 ms | 0.70 ms |  1.01 | 250.0000 |   2.36 MB |        1.18 |
