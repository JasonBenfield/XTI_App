using Microsoft.AspNetCore.Razor.TagHelpers;

namespace XTI_WebApp.TagHelpers;

[HtmlTargetElement("ko-with")]
public sealed class KnockoutWithTagHelper : KnockoutControlFlowTagHelper
{
    public KnockoutWithTagHelper() : base("with")
    {
    }

    public new string Name { get => base.Name; }
}