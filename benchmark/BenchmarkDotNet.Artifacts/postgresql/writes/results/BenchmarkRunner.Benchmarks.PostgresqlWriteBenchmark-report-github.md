```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-MYGTGH : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  IterationCount=10  
UnrollFactor=1  WarmupCount=2  Categories=Write  

```
| Method                   | TotalRecords | batchSize | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0        | Gen1        | Gen2       | Allocated  | Alloc Ratio |
|------------------------- |------------- |---------- |---------:|---------:|---------:|------:|--------:|------------:|------------:|-----------:|-----------:|------------:|
| **&#39;SQLC - AddOrderItems&#39;**   | **500000**       | **500**       |  **8.071 s** | **0.1118 s** | **0.0740 s** |  **1.00** |    **0.00** |   **3000.0000** |           **-** |          **-** |   **55.32 MB** |        **1.00** |
| &#39;EFCore - AddOrderItems&#39; | 500000       | 500       | 39.927 s | 0.1559 s | 0.1031 s |  4.95 |    0.04 | 325000.0000 | 321000.0000 | 22000.0000 | 5323.36 MB |       96.23 |
|                          |              |           |          |          |          |       |         |             |             |            |            |             |
| **&#39;SQLC - AddOrderItems&#39;**   | **500000**       | **1000**      |  **8.447 s** | **0.0128 s** | **0.0067 s** |  **1.00** |    **0.00** |   **3000.0000** |           **-** |          **-** |   **54.39 MB** |        **1.00** |
