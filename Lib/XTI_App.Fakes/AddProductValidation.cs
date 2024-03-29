﻿using XTI_App.Api;
using XTI_Core;

namespace XTI_App.Fakes;

public sealed class AddProductValidation : AppActionValidation<AddProductModel>
{
    public Task Validate(ErrorList errors, AddProductModel model, CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            errors.Add("Name is required");
        }
        return Task.CompletedTask;
    }
}