﻿using XTI_Core;

namespace XTI_App.Api;

public interface AppActionValidation<TModel>
{
    Task Validate(ErrorList errors, TModel model, CancellationToken stoppingToken);
}