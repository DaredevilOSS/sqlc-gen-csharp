```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-WLBKHG : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=3  

```
| Method                   | Categories       | BatchSize | Mean      | Error      | StdDev     | Ratio | RatioSD | Gen0      | Allocated  | Alloc Ratio |
|------------------------- |----------------- |---------- |----------:|-----------:|-----------:|------:|--------:|----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write-OrderItems** | **100**       |  **1.887 ms** |  **0.1271 ms** |  **0.0840 ms** |  **1.00** |    **0.00** |         **-** |  **304.59 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | Write-OrderItems | 100       |  5.856 ms |  1.4144 ms |  0.8417 ms |  3.10 |    0.46 |         - |    1873 KB |        6.15 |
|                          |                  |           |           |            |            |       |         |           |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write-OrderItems** | **200**       |  **5.227 ms** |  **0.1973 ms** |  **0.1174 ms** |  **1.00** |    **0.00** |         **-** |  **661.15 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | Write-OrderItems | 200       | 11.968 ms |  4.0509 ms |  2.4106 ms |  2.29 |    0.46 |         - | 3738.32 KB |        5.65 |
|                          |                  |           |           |            |            |       |         |           |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write-OrderItems** | **500**       | **26.833 ms** |  **0.7024 ms** |  **0.4180 ms** |  **1.00** |    **0.00** |         **-** | **1780.44 KB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | Write-OrderItems | 500       | 46.535 ms | 22.6250 ms | 14.9650 ms |  1.67 |    0.54 | 1000.0000 | 9320.73 KB |        5.24 |
|                          |                  |           |           |            |            |       |         |           |            |             |
| **&#39;SQLC - AddOrders&#39;**       | **Write-Orders**     | **100**       |        **NA** |         **NA** |         **NA** |     **?** |       **?** |        **NA** |         **NA** |           **?** |
| &#39;EFCore - AddOrders&#39;     | Write-Orders     | 100       |        NA |         NA |         NA |     ? |       ? |        NA |         NA |           ? |
|                          |                  |           |           |            |            |       |         |           |            |             |
| **&#39;SQLC - AddOrders&#39;**       | **Write-Orders**     | **200**       |        **NA** |         **NA** |         **NA** |     **?** |       **?** |        **NA** |         **NA** |           **?** |
| &#39;EFCore - AddOrders&#39;     | Write-Orders     | 200       |        NA |         NA |         NA |     ? |       ? |        NA |         NA |           ? |
|                          |                  |           |           |            |            |       |         |           |            |             |
| **&#39;SQLC - AddOrders&#39;**       | **Write-Orders**     | **500**       |        **NA** |         **NA** |         **NA** |     **?** |       **?** |        **NA** |         **NA** |           **?** |
| &#39;EFCore - AddOrders&#39;     | Write-Orders     | 500       |        NA |         NA |         NA |     ? |       ? |        NA |         NA |           ? |
|                          |                  |           |           |            |            |       |         |           |            |             |
| **&#39;SQLC - AddProducts&#39;**     | **Write-Products**   | **100**       |  **1.587 ms** |  **0.0777 ms** |  **0.0462 ms** |  **1.00** |    **0.00** |         **-** |  **298.84 KB** |        **1.00** |
| &#39;EFCore - AddProducts&#39;   | Write-Products   | 100       |  6.832 ms |  1.1368 ms |  0.6765 ms |  4.31 |    0.45 |         - | 2383.55 KB |        7.98 |
|                          |                  |           |           |            |            |       |         |           |            |             |
| **&#39;SQLC - AddProducts&#39;**     | **Write-Products**   | **200**       |  **4.638 ms** |  **0.1766 ms** |  **0.1051 ms** |  **1.00** |    **0.00** |         **-** |  **650.49 KB** |        **1.00** |
| &#39;EFCore - AddProducts&#39;   | Write-Products   | 200       | 18.403 ms |  9.0890 ms |  6.0118 ms |  3.83 |    1.29 |         - | 4761.41 KB |        7.32 |
|                          |                  |           |           |            |            |       |         |           |            |             |
| **&#39;SQLC - AddProducts&#39;**     | **Write-Products**   | **500**       | **24.458 ms** |  **0.5326 ms** |  **0.3523 ms** |  **1.00** |    **0.00** |         **-** | **1755.14 KB** |        **1.00** |
| &#39;EFCore - AddProducts&#39;   | Write-Products   | 500       | 50.617 ms | 22.0260 ms | 14.5688 ms |  2.07 |    0.61 | 1000.0000 | 11871.2 KB |        6.76 |

Benchmarks with issues:
  SqliteWriteBenchmark.'SQLC - AddOrders': Job-WLBKHG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=3) [BatchSize=100]
  SqliteWriteBenchmark.'EFCore - AddOrders': Job-WLBKHG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=3) [BatchSize=100]
  SqliteWriteBenchmark.'SQLC - AddOrders': Job-WLBKHG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=3) [BatchSize=200]
  SqliteWriteBenchmark.'EFCore - AddOrders': Job-WLBKHG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=3) [BatchSize=200]
  SqliteWriteBenchmark.'SQLC - AddOrders': Job-WLBKHG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=3) [BatchSize=500]
  SqliteWriteBenchmark.'EFCore - AddOrders': Job-WLBKHG(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=3) [BatchSize=500]
