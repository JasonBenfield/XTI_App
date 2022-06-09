using Microsoft.AspNetCore.Razor.TagHelpers;

namespace XTI_WebApp.TagHelpers;

[HtmlTargetElement("xti-component")]
public class XtiComponentTagHelper : TagHelper
{
    public XtiComponentTagHelper() : this("")
    {
    }

    public XtiComponentTagHelper(string name)
    {
        Name = name;
    }

    [HtmlAttributeName("name")]
    public string Name { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "";
        output.PreContent.AppendHtml($"<!-- ko with: {Name} -->");
        output.PreContent.AppendHtml("<!-- ko component: { name: componentName, params: $data } --><!-- /ko -->");
        output.PostContent.AppendHtml("<!-- /ko -->");
    }
}