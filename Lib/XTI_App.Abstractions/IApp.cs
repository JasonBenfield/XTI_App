namespace XTI_App.Abstractions;

public interface IApp
{
    int ID { get; }
    string Title { get; }
    Task<IAppVersion> Version(AppVersionKey versionKey);
    Task<IAppRole[]> Roles();
    Task<IModifierCategory> ModCategory(ModifierCategoryName name);
    Task<ModifierKey> ModKeyInHubApps();
}