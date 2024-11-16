namespace XTI_App.Fakes;

public sealed class FakeAppDB
{
    private static int appID = 1;
    private static int resourceGroupID = 101;
    private static int modCategoryID = 201;
    private static int resourceID = 301;
    private static int roleID = 401;
    private static int modifierID = 501;

    private static readonly Dictionary<AppKey, int> appKeyIDs = new();

    public static int GetAppID(AppKey appKey)
    {
        if (!appKeyIDs.TryGetValue(appKey, out var currentAppID))
        {
            currentAppID = appID;
            appKeyIDs.Add(appKey, appID);
            appID++;
        }
        return currentAppID;
    }

    public static int GenerateResourceGroupID()
    {
        var currentResourceGroupID = resourceGroupID;
        resourceGroupID++;
        return currentResourceGroupID;
    }

    public static int GenerateModCategoryID()
    {
        var currentModCategoryID = modCategoryID;
        modCategoryID++;
        return currentModCategoryID;
    }

    public static int GenerateResourceID()
    {
        var currentResourceID = resourceID;
        resourceID++;
        return currentResourceID;
    }

    public static int GenerateRoleID()
    {
        var currentRoleID = roleID;
        roleID++;
        return currentRoleID;
    }

    public static int GenerateModifierID()
    {
        var currentModifierID = modifierID;
        modifierID++;
        return currentModifierID;
    }
}
