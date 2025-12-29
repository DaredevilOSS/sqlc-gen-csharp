```

BenchmarkDotNet v0.13.12, macOS 15.6.1 (24G90) [Darwin 24.6.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD
  Job-ZZDYFR : .NET 8.0.22 (8.0.2225.52707), Arm64 RyuJIT AdvSIMD

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args          | Mean | Error | Ratio | RatioSD | Alloc Ratio |
|------------------------- |-------------- |-----:|------:|------:|--------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **R=300K, B=100** |   **NA** |    **NA** |     **?** |       **?** |           **?** |
|                          |               |      |       |       |         |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **R=300K, B=50**  |   **NA** |    **NA** |     **?** |       **?** |           **?** |
|                          |               |      |       |       |         |             |
| **&#39;EFCore - AddOrderItems&#39;** | **R=300K, B=500** |   **NA** |    **NA** |     **?** |       **?** |           **?** |

Benchmarks with issues:
  SqliteWriteBenchmark.'SQLC - AddOrderItems': Job-ZZDYFR(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [args=R=300K, B=100]
  SqliteWriteBenchmark.'SQLC - AddOrderItems': Job-ZZDYFR(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [args=R=300K, B=50]
  SqliteWriteBenchmark.'EFCore - AddOrderItems': Job-ZZDYFR(Runtime=.NET 8.0, InvocationCount=1, IterationCount=10, UnrollFactor=1, WarmupCount=2) [args=R=300K, B=500]
