```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-MELAGO : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  IterationCount=10  WarmupCount=3  
Categories=Read  

```
| Method                                      | Mean      | Error     | StdDev    | Ratio | Gen0    | Gen1    | Gen2    | Allocated | Alloc Ratio |
|-------------------------------------------- |----------:|----------:|----------:|------:|--------:|--------:|--------:|----------:|------------:|
| &#39;SQLC - GetCustomerOrders&#39;                  | 23.914 ms | 0.0833 ms | 0.0496 ms |  1.00 | 93.7500 | 31.2500 | 31.2500 | 647.94 KB |        1.00 |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   |  4.252 ms | 0.1710 ms | 0.1017 ms |  0.18 | 78.1250 |  7.8125 |       - | 675.29 KB |        1.04 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; |  4.180 ms | 0.0199 ms | 0.0118 ms |  0.17 | 78.1250 | 15.6250 |       - | 664.64 KB |        1.03 |
