using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace XTI_WebApp.TagHelpers;

[HtmlTargetElement("ko-control-flow")]
public class KnockoutControlFlowTagHelper : TagHelper
{
    public KnockoutControlFlowTagHelper() : this("")
    {
    }

    public KnockoutControlFlowTagHelper(string name)
    {
        Name = name;
    }

    [HtmlAttributeName("name")]
    public string Name { get; set; } = "";

    [HtmlAttributeName("value")]
    public string Value { get; set; } = "";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "";
        output.PreContent.AppendHtml($"<!-- ko {Name}: {Value} -->");
        var childContent = await output.GetChildContentAsync();
        output.Content.SetHtmlContent(childContent.GetContent());
        output.PostContent.AppendHtml("<!-- /ko -->");
    }
}