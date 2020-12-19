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
            Name = name;
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

        public bool IsView() => ResultTemplate.DataType.Name == "AppActionViewResult";
        public bool IsRedirect() => ResultTemplate.DataType.Name == "AppActionRedirectResult";
        public bool HasEmptyModel() => ModelTemplate.DataType == typeof(EmptyRequest);

        public IEnumerable<ObjectValueTemplate> ObjectTemplates() =>
            ModelTemplate.ObjectTemplates()
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
