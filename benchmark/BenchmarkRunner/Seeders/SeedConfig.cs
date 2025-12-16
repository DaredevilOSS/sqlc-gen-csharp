public readonly record struct DatabaseSeedConfig(
    int CustomerCount,
    int ProductsPerCategory,
    int OrdersPerCustomer,
    int ItemsPerOrder
);