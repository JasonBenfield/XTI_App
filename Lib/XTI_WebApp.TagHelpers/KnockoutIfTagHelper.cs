using Microsoft.AspNetCore.Razor.TagHelpers;

namespace XTI_WebApp.TagHelpers;

[HtmlTargetElement("ko-if")]
public sealed class KnockoutIfTagHelper : KnockoutControlFlowTagHelper
{
    public KnockoutIfTagHelper() : base("if")
    {
    }

    public new string Name { get => base.Name; }
}