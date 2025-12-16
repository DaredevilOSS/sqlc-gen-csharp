```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-XJYJBN : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0      | Gen1      | Allocated | Alloc Ratio |
|------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|----------:|----------:|----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)100 } [61]** | **132.4 ms** | **17.79 ms** | **11.77 ms** |  **1.00** |    **0.00** |         **-** |         **-** |   **10.6 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |           |           |           |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...) 50 } [60]** | **175.6 ms** | **22.83 ms** | **15.10 ms** |  **1.00** |    **0.00** |         **-** |         **-** |  **10.69 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |           |           |           |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [61]** | **227.5 ms** | **27.58 ms** | **16.41 ms** |     **?** |       **?** | **3000.0000** | **1000.0000** |  **60.08 MB** |           **?** |
