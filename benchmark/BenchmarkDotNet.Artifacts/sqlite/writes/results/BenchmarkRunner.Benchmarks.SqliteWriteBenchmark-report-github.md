```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-LMZXQC : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0      | Gen1      | Allocated | Alloc Ratio |
|------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|----------:|----------:|----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)100 } [61]** | **134.4 ms** | **19.59 ms** | **12.96 ms** |  **1.00** |    **0.00** |         **-** |         **-** |   **10.6 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |           |           |           |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...) 50 } [60]** | **164.0 ms** | **21.99 ms** | **14.55 ms** |  **1.00** |    **0.00** |         **-** |         **-** |  **10.69 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |           |           |           |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [61]** | **213.9 ms** | **13.76 ms** |  **7.20 ms** |     **?** |       **?** | **3000.0000** | **1000.0000** |  **60.08 MB** |           **?** |
