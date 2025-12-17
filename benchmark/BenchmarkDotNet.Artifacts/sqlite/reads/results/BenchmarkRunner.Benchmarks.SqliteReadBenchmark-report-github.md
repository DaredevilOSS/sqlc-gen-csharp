```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-LIUWPF : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params             | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------- |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1.0K,C=50,Q=1.5K** |  **9.275 s** | **0.0605 s** | **0.0360 s** |  **1.00** | **49000.0000** | **48000.0000** | **6000.0000** | **1010.13 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1.0K,C=50,Q=1.5K | 11.224 s | 0.0340 s | 0.0203 s |  1.21 | 72000.0000 | 71000.0000 | 4000.0000 | 1129.38 MB |        1.12 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1.0K,C=50,Q=1.5K | 11.135 s | 0.1851 s | 0.1224 s |  1.20 | 72000.0000 | 71000.0000 | 4000.0000 | 1126.16 MB |        1.11 |
|                                             |                    |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50,C=100,Q=3.0K**  |  **9.145 s** | **0.0288 s** | **0.0191 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |   **127.4 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50,C=100,Q=3.0K  | 10.347 s | 0.1054 s | 0.0697 s |  1.13 | 22000.0000 |  8000.0000 | 2000.0000 |  343.28 MB |        2.69 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50,C=100,Q=3.0K  | 10.406 s | 0.0499 s | 0.0330 s |  1.14 | 21000.0000 |  8000.0000 | 2000.0000 |  337.74 MB |        2.65 |
