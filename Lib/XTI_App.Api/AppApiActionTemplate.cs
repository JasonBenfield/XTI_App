using System.Runtime.InteropServices;
using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class AppApiActionTemplate
{
    private readonly ResourceResultType resultType;

    public AppApiActionTemplate
    (
        string name,
        string friendlyName,
        ResourceAccess access,
        ValueTemplate modelTemplate,
        ValueTemplate resultTemplate,
        ResourceResultType? resultType = null
    )
    {
        Name = name.Replace(" ", "");
        FriendlyName = friendlyName;
        Access = access;
        ModelTemplate = modelTemplate;
        ResultTemplate = resultTemplate;
        this.resultType = resultType ?? ResultTypeFromResultTemplate(resultTemplate);
    }

    private static ResourceResultType ResultTypeFromResultTemplate(ValueTemplate resultTemplate)
    {
        ResourceResultType type;
        if (resultTemplate.DataType.Name == "WebViewResult")
        {
            type = ResourceResultType.Values.View;
        }
        else if (resultTemplate.DataType.Name == "WebPartialViewResult")
        {
            type = ResourceResultType.Values.PartialView;
        }
        else if (resultTemplate.DataType.Name == "WebRedirectResult")
        {
            type = ResourceResultType.Values.Redirect;
        }
        else if (resultTemplate.DataType.Name == "WebFileResult")
        {
            type = ResourceResultType.Values.File;
        }
        else if (resultTemplate.DataType.Name == "WebContentResult")
        {
            type = ResourceResultType.Values.Content;
        }
        else if
        (
            resultTemplate.DataType.IsGenericType &&
            resultTemplate.DataType.GetGenericTypeDefinition() == typeof(IQueryable<>)
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

    public string Name { get; }
    public string FriendlyName { get; }
    public ResourceAccess Access { get; }
    public ValueTemplate ModelTemplate { get; }
    public ValueTemplate ResultTemplate { get; }

    public bool IsView() => resultType.Equals(ResourceResultType.Values.View);
    public bool IsPartialView() => resultType.Equals(ResourceResultType.Values.PartialView);
    public bool IsRedirect() => resultType.Equals(ResourceResultType.Values.Redirect);
    public bool IsFile() => resultType.Equals(ResourceResultType.Values.File);
    public bool IsContent() => resultType.Equals(ResourceResultType.Values.Content);
    public bool IsQuery() => resultType.Equals(ResourceResultType.Values.Query);
    public bool IsQueryToExcel() => resultType.Equals(ResourceResultType.Values.QueryToExcel);
    public bool HasEmptyModel() => ModelTemplate.DataType == typeof(EmptyRequest);

    public bool HasFileUploadTemplates() => HasFileUploadTemplates(ModelTemplate);

    private bool HasFileUploadTemplates(ValueTemplate valueTemplate)
    {
        var hasFileUploads = false;
        if (valueTemplate is FileUploadValueTemplate modelFileUploadTempl)
        {
            hasFileUploads = true;
        }
        else if (valueTemplate is ArrayValueTemplate arrTempl && arrTempl.ElementTemplate is FileUploadValueTemplate arrElFileUploadTempl)
        {
            hasFileUploads = true;
        }
        else
        {
            foreach (var objTempl in valueTemplate.ObjectTemplates())
            {
                var propFileUploadTempls = objTempl.PropertyTemplates
                    .Select(pt => pt.ValueTemplate)
                    .OfType<FileUploadValueTemplate>();
                if (propFileUploadTempls.Any())
                {
                    hasFileUploads = true;
                    break;
                }
                else
                {
                    var arrObjectTempls = objTempl.PropertyTemplates
                        .Select(pt => pt.ValueTemplate)
                        .OfType<ArrayValueTemplate>();
                    foreach (var propObjectTempl in arrObjectTempls)
                    {
                        if (HasFileUploadTemplates(propObjectTempl))
                        {
                            hasFileUploads = true;
                            break;
                        }
                    }
                    if (!hasFileUploads)
                    {
                        var propObjectTempls = objTempl.PropertyTemplates
                            .Select(pt => pt.ValueTemplate)
                            .OfType<ObjectValueTemplate>();
                        foreach (var propObjectTempl in propObjectTempls)
                        {
                            if (HasFileUploadTemplates(propObjectTempl))
                            {
                                hasFileUploads = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
        return hasFileUploads;
    }

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

    public IEnumerable<QueryableValueTemplate> QueryableTemplates() =>
        ResultTemplate is QueryableValueTemplate queryable
            ? new[] { queryable }
            : new QueryableValueTemplate[0];

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
        if (ModelTemplate is FormValueTemplate formTempl)
        {
            numericTemplates.AddRange(formTempl.NumericValueTemplates());
        }
        if (ResultTemplate is FormValueTemplate formResultTempl)
        {
            numericTemplates.AddRange(formResultTempl.NumericValueTemplates());
        }
        foreach (var objTempl in ObjectTemplates())
        {
            var propNumTempls = objTempl.PropertyTemplates.Select(pt => pt.ValueTemplate).OfType<NumericValueTemplate>();
            numericTemplates.AddRange(propNumTempls);
            var actualNumTempls = objTempl.PropertyTemplates
                .Select(pt => pt.NumericValueTemplate)
                .Where(nt => nt != null);
            numericTemplates.AddRange(actualNumTempls!);
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
            ResultType = resultType
        };
}