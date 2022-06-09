using Microsoft.AspNetCore.Razor.TagHelpers;

namespace XTI_WebApp.TagHelpers;

[HtmlTargetElement("ko-let")]
public sealed class KnockoutLetTagHelper : KnockoutControlFlowTagHelper
{
    public KnockoutLetTagHelper() : base("let")
    {
    }

    public new string Name { get => base.Name; }
}