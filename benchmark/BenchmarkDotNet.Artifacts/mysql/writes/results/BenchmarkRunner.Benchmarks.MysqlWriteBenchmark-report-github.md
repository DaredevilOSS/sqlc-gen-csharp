```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-VPXQSM : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0      | Gen1      | Allocated | Alloc Ratio |
|------------------------- |--------------------- |------------:|----------:|----------:|------:|--------:|----------:|----------:|----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [62]** |   **108.33 ms** |  **3.374 ms** |  **2.232 ms** |  **1.00** |    **0.00** |         **-** |         **-** |   **1.71 MB** |        **1.00** |
|                          |                      |             |           |           |       |         |           |           |           |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [61]** | **1,291.55 ms** | **29.754 ms** | **17.706 ms** |     **?** |       **?** | **4000.0000** | **1000.0000** |  **71.24 MB** |           **?** |
|                          |                      |             |           |           |       |         |           |           |           |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [62]** |    **92.17 ms** | **10.973 ms** |  **6.530 ms** |  **1.00** |    **0.00** |         **-** |         **-** |   **2.63 MB** |        **1.00** |
