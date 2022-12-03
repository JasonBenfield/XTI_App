namespace XTI_WebApp.Api;

public sealed class DefaultMenuDefinitionBuilder : IMenuDefinitionBuilder
{
    private readonly UserMenuDefinition userMenuDefinition;

    public DefaultMenuDefinitionBuilder(UserMenuDefinition userMenuDefinition)
    {
        this.userMenuDefinition = userMenuDefinition;
    }

    public AppMenuDefinitions Build() =>
        new AppMenuDefinitions(userMenuDefinition.Value);
}
