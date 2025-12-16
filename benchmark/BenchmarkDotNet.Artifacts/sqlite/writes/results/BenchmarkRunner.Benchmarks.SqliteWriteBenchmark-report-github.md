```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-XMSADD : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | TotalRecords | batchSize | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1      | Allocated  | Alloc Ratio |
|------------------------- |------------- |---------- |---------:|---------:|---------:|------:|--------:|------------:|----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **200000**       | **50**        |  **7.328 s** | **0.1512 s** | **0.0791 s** |  **1.00** |    **0.00** |  **26000.0000** | **2000.0000** |  **427.34 MB** |        **1.00** |
|                          |              |           |          |          |          |       |         |             |           |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **200000**       | **100**       |  **5.552 s** | **0.1530 s** | **0.1012 s** |  **1.00** |    **0.00** |  **26000.0000** | **3000.0000** |  **423.75 MB** |        **1.00** |
|                          |              |           |          |          |          |       |         |             |           |            |             |
| **&#39;EFCore - AddOrderItems&#39;** | **200000**       | **500**       | **10.377 s** | **0.3049 s** | **0.2017 s** |     **?** |       **?** | **150000.0000** | **1000.0000** | **2402.99 MB** |           **?** |
