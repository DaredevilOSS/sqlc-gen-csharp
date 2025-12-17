```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-MFOVVY : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1        | Allocated  | Alloc Ratio |
|------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|------------:|------------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [68]** |  **5.016 s** | **0.0782 s** | **0.0465 s** |  **1.00** |    **0.00** |   **6000.0000** |   **3000.0000** |   **109.6 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |             |             |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [67]** | **76.680 s** | **0.2022 s** | **0.1203 s** |     **?** |       **?** | **267000.0000** | **136000.0000** | **4260.07 MB** |           **?** |
|                          |                      |          |          |          |       |         |             |             |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [68]** |  **3.929 s** | **0.0718 s** | **0.0475 s** |  **1.00** |    **0.00** |   **4000.0000** |   **1000.0000** |   **91.79 MB** |        **1.00** |
