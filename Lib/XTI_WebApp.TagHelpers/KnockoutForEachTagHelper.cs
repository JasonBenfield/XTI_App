using Microsoft.AspNetCore.Razor.TagHelpers;

namespace XTI_WebApp.TagHelpers;

[HtmlTargetElement("ko-foreach")]
public sealed class KnockoutForEachTagHelper : KnockoutControlFlowTagHelper
{
    public KnockoutForEachTagHelper() : base("foreach")
    {
    }

    public new string Name { get => base.Name; }
}