using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiActionTemplate
{
    public AppApiActionTemplate
    (
        string name,
        string friendlyName,
        ResourceAccess access,
        ValueTemplate modelTemplate,
        ValueTemplate resultTemplate
    )
    {
        Name = name.Replace(" ", "");
        FriendlyName = friendlyName;
        Access = access;
        ModelTemplate = modelTemplate;
        ResultTemplate = resultTemplate;
    }

    public string Name { get; }
    public string FriendlyName { get; }
    public ResourceAccess Access { get; }
    public ValueTemplate ModelTemplate { get; }
    public ValueTemplate ResultTemplate { get; }

    public ResourceResultType ResultType()
    {
        ResourceResultType type;
        if (ResultTemplate.DataType.Name == "WebViewResult")
        {
            type = ResourceResultType.Values.View;
        }
        else if (ResultTemplate.DataType.Name == "WebPartialViewResult")
        {
            type = ResourceResultType.Values.PartialView;
        }
        else if (ResultTemplate.DataType.Name == "WebRedirectResult")
        {
            type = ResourceResultType.Values.Redirect;
        }
        else if (ResultTemplate.DataType.Name == "WebFileResult")
        {
            type = ResourceResultType.Values.File;
        }
        else if (ResultTemplate.DataType.Name == "WebContentResult")
        {
            type = ResourceResultType.Values.Content;
        }
        else if 
        (
            ResultTemplate.DataType.IsGenericType &&
            ResultTemplate.DataType.GetGenericTypeDefinition() == typeof(IQueryable<>)
        )
        {
            type = ResourceResultType.Values.Query;
        }
        else
        {
            type = ResourceResultType.Values.Json;
        }
        return type;
    }
    public bool IsView() => ResultType().Equals(ResourceResultType.Values.View);
    public bool IsPartialView() => ResultType().Equals(ResourceResultType.Values.PartialView);
    public bool IsRedirect() => ResultType().Equals(ResourceResultType.Values.Redirect);
    public bool IsFile() => ResultType().Equals(ResourceResultType.Values.File);
    public bool IsContent() => ResultType().Equals(ResourceResultType.Values.Content);
    public bool IsQuery() => ResultType().Equals(ResourceResultType.Values.Query);
    public bool HasEmptyModel() => ModelTemplate.DataType == typeof(EmptyRequest);

    public IEnumerable<FormValueTemplate> FormTemplates()
    {
        var formTemplates = new List<FormValueTemplate>();
        if (ModelTemplate is FormValueTemplate modelFormTempl)
        {
            formTemplates.Add(modelFormTempl);
        }
        if (ResultTemplate is FormValueTemplate resultFormTempl)
        {
            formTemplates.Add(resultFormTempl);
        }
        return formTemplates.Distinct();
    }

    public IEnumerable<ObjectValueTemplate> ObjectTemplates() =>
        IsView() || IsPartialView() || IsRedirect()
            ? ModelTemplate.ObjectTemplates()
            : ModelTemplate.ObjectTemplates()
                .Union
                (
                    ResultTemplate.ObjectTemplates()
                )
                .Distinct();

    public IEnumerable<NumericValueTemplate> NumericValueTemplates()
    {
        var numericTemplates = new List<NumericValueTemplate>();
        if (ModelTemplate is NumericValueTemplate modelNumTempl)
        {
            numericTemplates.Add(modelNumTempl);
        }
        if (ResultTemplate is NumericValueTemplate resultNumTempl)
        {
            numericTemplates.Add(resultNumTempl);
        }
        foreach (var objTempl in ObjectTemplates())
        {
            var propNumTempls = objTempl.PropertyTemplates.Select(pt => pt.ValueTemplate).OfType<NumericValueTemplate>();
            numericTemplates.AddRange(propNumTempls);
        }
        return numericTemplates.Distinct();
    }

    public IEnumerable<EnumValueTemplate> EnumValueTemplates()
    {
        var enumTemplates = new List<EnumValueTemplate>();
        if (ModelTemplate is EnumValueTemplate modelNumTempl)
        {
            enumTemplates.Add(modelNumTempl);
        }
        if (ResultTemplate is EnumValueTemplate resultNumTempl)
        {
            enumTemplates.Add(resultNumTempl);
        }
        foreach (var objTempl in ObjectTemplates())
        {
            var propNumTempls = objTempl.PropertyTemplates.Select(pt => pt.ValueTemplate).OfType<EnumValueTemplate>();
            enumTemplates.AddRange(propNumTempls);
        }
        return enumTemplates.Distinct();
    }

    public AppApiActionTemplateModel ToModel()
        => new AppApiActionTemplateModel
        {
            Name = Name,
            IsAnonymousAllowed = Access.IsAnonymousAllowed,
            Roles = Access.Allowed.Select(r => r.Value).ToArray(),
            ResultType = ResultType()
        };
}