```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-MYKBFJ : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | args                 | Mean    | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1       | Allocated  | Alloc Ratio |
|------------------------- |--------------------- |--------:|---------:|---------:|------:|--------:|------------:|-----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...)100 } [61]** | **5.536 s** | **0.0983 s** | **0.0650 s** |  **1.00** |    **0.00** |  **26000.0000** |  **3000.0000** |  **429.86 MB** |        **1.00** |
|                          |                      |         |          |          |       |         |             |            |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **Write(...) 50 } [60]** | **6.768 s** | **0.1268 s** | **0.0839 s** |  **1.00** |    **0.00** |  **26000.0000** |  **2000.0000** |  **433.45 MB** |        **1.00** |
|                          |                      |         |          |          |       |         |             |            |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **Write(...)500 } [61]** | **9.977 s** | **0.0604 s** | **0.0316 s** |     **?** |       **?** | **149000.0000** | **74000.0000** | **2393.84 MB** |           **?** |
