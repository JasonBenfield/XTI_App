namespace XTI_App.Abstractions;

public interface InstallationIDAccessor
{
    Task<int> Value();
}
