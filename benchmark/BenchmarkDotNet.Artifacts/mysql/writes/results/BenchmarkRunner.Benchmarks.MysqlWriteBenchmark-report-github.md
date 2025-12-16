```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-VAFIIN : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1       | Allocated  | Alloc Ratio |
|------------------------- |--------------------- |---------:|---------:|---------:|------:|--------:|------------:|-----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [62]** |  **3.213 s** | **0.0368 s** | **0.0243 s** |  **1.00** |    **0.00** |   **2000.0000** |  **1000.0000** |   **73.08 MB** |        **1.00** |
|                          |                      |          |          |          |       |         |             |            |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [61]** | **39.275 s** | **0.8065 s** | **0.5335 s** |     **?** |       **?** | **118000.0000** | **59000.0000** | **2842.29 MB** |           **?** |
|                          |                      |          |          |          |       |         |             |            |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)000 } [62]** |  **2.362 s** | **0.0140 s** | **0.0084 s** |  **1.00** |    **0.00** |   **1000.0000** |          **-** |   **61.62 MB** |        **1.00** |
