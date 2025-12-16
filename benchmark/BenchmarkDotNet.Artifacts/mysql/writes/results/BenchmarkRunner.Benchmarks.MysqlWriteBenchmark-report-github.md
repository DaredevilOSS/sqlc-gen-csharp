```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-ZPDMLD : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | TotalRecords | batchSize | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|------------------------- |------------- |---------- |---------:|---------:|---------:|------:|--------:|------------:|-----------:|----------:|-----------:|------------:|
| **&#39;EFCore - AddOrderItems&#39;** | **200000**       | **500**       | **51.584 s** | **0.2704 s** | **0.1788 s** |     **?** |       **?** | **179000.0000** | **90000.0000** | **1000.0000** | **2849.37 MB** |           **?** |
|                          |              |           |          |          |          |       |         |             |            |           |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **200000**       | **1000**      |  **3.648 s** | **0.0621 s** | **0.0411 s** |  **1.00** |    **0.00** |   **4000.0000** |  **3000.0000** |         **-** |   **66.98 MB** |        **1.00** |
|                          |              |           |          |          |          |       |         |             |            |           |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **200000**       | **2000**      |  **3.083 s** | **0.0765 s** | **0.0455 s** |  **1.00** |    **0.00** |   **3000.0000** |  **1000.0000** |         **-** |   **59.47 MB** |        **1.00** |
