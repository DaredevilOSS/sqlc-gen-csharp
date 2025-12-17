```

BenchmarkDotNet v0.13.12, Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.416
  [Host]     : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2
  Job-EBKUDR : .NET 8.0.22 (8.0.2225.52707), X64 RyuJIT AVX2

Runtime=.NET 8.0  IterationCount=10  WarmupCount=2  
Categories=Read  

```
| Method                                      | Params             | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated  | Alloc Ratio |
|-------------------------------------------- |------------------- |---------:|---------:|---------:|------:|--------:|-----------:|-----------:|----------:|-----------:|------------:|
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=1.0K,C=75,Q=2.0K** |  **7.851 s** | **0.1197 s** | **0.0792 s** |  **1.00** |    **0.00** | **39000.0000** | **38000.0000** | **2000.0000** |  **910.02 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=1.0K,C=75,Q=2.0K |  8.735 s | 0.1029 s | 0.0680 s |  1.11 |    0.02 | 65000.0000 | 64000.0000 | 3000.0000 | 1026.72 MB |        1.13 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=1.0K,C=75,Q=2.0K |  8.741 s | 0.1139 s | 0.0678 s |  1.11 |    0.01 | 65000.0000 | 64000.0000 | 3000.0000 | 1023.44 MB |        1.12 |
|                                             |                    |          |          |          |       |         |            |            |           |            |             |
| **&#39;SQLC - GetCustomerOrders&#39;**                  | **L=50,C=150,Q=4.0K**  | **10.925 s** | **0.0832 s** | **0.0550 s** |  **1.00** |    **0.00** |  **7000.0000** |  **6000.0000** | **2000.0000** |  **131.27 MB** |        **1.00** |
| &#39;EFCore (NoTracking) - GetCustomerOrders&#39;   | L=50,C=150,Q=4.0K  | 11.493 s | 0.0892 s | 0.0590 s |  1.05 |    0.01 | 26000.0000 | 10000.0000 | 2000.0000 |  388.34 MB |        2.96 |
| &#39;EFCore (WithTracking) - GetCustomerOrders&#39; | L=50,C=150,Q=4.0K  | 11.467 s | 0.1347 s | 0.0891 s |  1.05 |    0.01 | 25000.0000 | 10000.0000 | 2000.0000 |  380.79 MB |        2.90 |
