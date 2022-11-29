using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Api;

public sealed record AppMenuDefinitions(params MenuDefinition[] MenuDefinitions)
{
    public LinkModel[] GetLinks(string menuName)
    {
        var menuDefinition = MenuDefinitions.FirstOrDefault(md => md.MenuName == menuName);
        if(menuDefinition == null)
        {
            throw new ArgumentNullException(nameof(menuDefinition), $"Menu '{menuName}' was not found");
        }
        return menuDefinition.Links;
    }
}
