using Microsoft.AspNetCore.Razor.TagHelpers;

namespace XTI_WebApp.TagHelpers;

[HtmlTargetElement("ko-ifnot")]
public sealed class KnockoutIfNotTagHelper : KnockoutControlFlowTagHelper
{
    public KnockoutIfNotTagHelper() : base("ifnot")
    {
    }

    public new string Name { get => base.Name; }
}