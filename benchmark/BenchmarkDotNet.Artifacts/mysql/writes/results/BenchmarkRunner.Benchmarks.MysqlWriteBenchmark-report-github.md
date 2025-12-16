```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-ZWCWIG : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0      | Gen1      | Allocated | Alloc Ratio |
|------------------------- |--------------------- |------------:|----------:|----------:|------:|--------:|----------:|----------:|----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [62]** |    **97.07 ms** |  **7.224 ms** |  **3.778 ms** |  **1.00** |    **0.00** |         **-** |         **-** |   **1.71 MB** |        **1.00** |
|                          |                      |             |           |           |       |         |           |           |           |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [61]** | **1,229.49 ms** | **24.156 ms** | **15.978 ms** |     **?** |       **?** | **4000.0000** | **1000.0000** |  **71.27 MB** |           **?** |
|                          |                      |             |           |           |       |         |           |           |           |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [62]** |    **78.93 ms** |  **5.947 ms** |  **3.110 ms** |  **1.00** |    **0.00** |         **-** |         **-** |   **1.63 MB** |        **1.00** |
