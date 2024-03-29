﻿using XTI_App.Api;

namespace XTI_App.Fakes;

public sealed class AddEmployeeAction : AppAction<AddEmployeeModel, int>
{
    public Task<int> Execute(AddEmployeeModel model, CancellationToken stoppingToken)
    {
        return Task.FromResult(1);
    }
}