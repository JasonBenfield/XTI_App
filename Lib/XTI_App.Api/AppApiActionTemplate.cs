using System.Collections.Generic;
using System.Linq;

namespace XTI_App.Api
{
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

        public bool IsView() => ResultTemplate.DataType.Name == "WebViewResult";
        public bool IsPartialView() => ResultTemplate.DataType.Name == "WebPartialViewResult";
        public bool IsRedirect() => ResultTemplate.DataType.Name == "WebRedirectResult";
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
    }
}
