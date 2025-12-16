```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-SIVYTC : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1       | Allocated  | Alloc Ratio |
|------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|------------:|-----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [62]** |  **3.235 s** | **0.2118 s** | **0.1401 s** |  **1.00** |    **0.00** |   **4000.0000** |  **2000.0000** |   **73.06 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |             |            |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [61]** | **52.190 s** | **0.5455 s** | **0.3608 s** |     **?** |       **?** | **178000.0000** | **90000.0000** | **2839.27 MB** |           **?** |
|                          |                      |          |          |          |       |         |             |            |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [62]** |  **2.608 s** | **0.0750 s** | **0.0496 s** |  **1.00** |    **0.00** |   **2000.0000** |          **-** |   **61.29 MB** |        **1.00** |
