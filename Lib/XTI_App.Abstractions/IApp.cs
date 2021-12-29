namespace XTI_App.Abstractions;

public interface IApp
{
    EntityID ID { get; }
    string Title { get; }
    Task<IAppVersion> Version(AppVersionKey versionKey);
    Task<IAppRole[]> Roles();
    Task<IModifierCategory> ModCategory(ModifierCategoryName name);
}