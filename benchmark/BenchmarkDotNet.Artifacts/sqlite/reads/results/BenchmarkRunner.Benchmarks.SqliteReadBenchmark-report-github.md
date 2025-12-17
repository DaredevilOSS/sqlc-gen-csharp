```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-DQEAZA : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params               | Mean     | Error    | StdDev   | Ratio | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |--------------------- |---------:|---------:|---------:|------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **Limit(...)es=1K [36]** |  **9.351 s** | **0.0208 s** | **0.0124 s** |  **1.00** | **49000.0000** | **48000.0000** | **6000.0000** | **1010.07 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | Limit(...)es=1K [36] | 11.170 s | 0.0466 s | 0.0277 s |  1.19 | 72000.0000 | 71000.0000 | 4000.0000 | 1128.96 MB |        1.12 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | Limit(...)es=1K [36] | 11.237 s | 0.0600 s | 0.0357 s |  1.20 | 72000.0000 | 71000.0000 | 4000.0000 | 1126.54 MB |        1.12 |
|                                             |                      |          |          |          |       |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **Limit(...)es=3K [37]** |  **9.250 s** | **0.0314 s** | **0.0207 s** |  **1.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **127.37 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | Limit(...)es=3K [37] | 10.365 s | 0.0208 s | 0.0138 s |  1.12 | 22000.0000 |  8000.0000 | 2000.0000 |  345.35 MB |        2.71 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | Limit(...)es=3K [37] | 10.278 s | 0.0219 s | 0.0145 s |  1.11 | 22000.0000 |  8000.0000 | 2000.0000 |  338.85 MB |        2.66 |
