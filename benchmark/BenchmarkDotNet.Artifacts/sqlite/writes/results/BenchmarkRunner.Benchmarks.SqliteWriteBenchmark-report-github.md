```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-UDNNXA : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0      | Gen1      | Allocated | Alloc Ratio |
|------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|----------:|----------:|----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)100 } [61]** | **121.2 ms** | **12.37 ms** |  **8.18 ms** |  **1.00** |    **0.00** |         **-** |         **-** |   **10.6 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |           |           |           |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...) 50 } [60]** | **159.0 ms** | **24.12 ms** | **15.95 ms** |  **1.00** |    **0.00** |         **-** |         **-** |  **10.69 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |           |           |           |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [61]** | **222.8 ms** | **18.49 ms** | **11.00 ms** |     **?** |       **?** | **3000.0000** | **1000.0000** |  **60.08 MB** |           **?** |
