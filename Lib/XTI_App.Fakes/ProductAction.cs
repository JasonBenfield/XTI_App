﻿using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class ProductAction : AppAction<int, Product>
{
    public Task<Product> Execute(int id, CancellationToken stoppingToken)
    {
        return Task.FromResult(new Product { ID = id, Quantity = 2, Price = 23.42M });
    }
}