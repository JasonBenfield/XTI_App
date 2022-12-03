using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed record MenuDefinition(string MenuName, params LinkModel[] Links);
