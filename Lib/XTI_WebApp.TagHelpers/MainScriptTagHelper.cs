using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using XTI_Core;
using XTI_WebApp.Api;

namespace XTI_WebApp.TagHelpers;

[HtmlTargetElement("xti-main-script", TagStructure = TagStructure.WithoutEndTag)]
public sealed class MainScriptTagHelper : TagHelper
{
    private readonly XtiEnvironment xtiEnv;
    private readonly IUrlHelperFactory urlHelperFactory;
    private readonly CacheBust cacheBust;

    public MainScriptTagHelper(XtiEnvironment xtiEnv, IUrlHelperFactory urlHelperFactory, CacheBust cacheBust)
    {
        this.xtiEnv = xtiEnv;
        this.urlHelperFactory = urlHelperFactory;
        this.cacheBust = cacheBust;
    }

    public string PageName { get; set; } = "";

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext? ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "script";
        string path;
        if (xtiEnv.IsDevelopmentOrTest())
        {
            path = "~/js/dev/";
        }
        else
        {
            path = "~/js/dist/";
        }
        var urlHelper = urlHelperFactory.GetUrlHelper
        (
            ViewContext ?? throw new ArgumentNullException(nameof(ViewContext))
        );
        var pageUrl = await getPageUrl(urlHelper, path);
        output.Attributes.Add("src", pageUrl);
        output.TagMode = TagMode.StartTagAndEndTag;
    }

    private async Task<string> getPageUrl(IUrlHelper urlHelper, string path)
    {
        var query = await cacheBust.Query();
        if (!string.IsNullOrWhiteSpace(query))
        {
            query = $"?{query}";
        }
        return urlHelper.Content($"{path}{PageName}.js{query}");
    }
}