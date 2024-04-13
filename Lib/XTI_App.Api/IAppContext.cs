using XTI_App.Abstractions;

namespace XTI_App.Api;

public interface IAppContext
{
    Task<AppContextModel> App();
    Task<ModifierModel> Modifier(ModifierCategoryModel category, ModifierKey modKey);
}