using XTI_App.Abstractions;
using XTI_Core;

namespace XTI_App.Extensions
{
    public static class XtiFolderExtensions
    {
        public static AppDataFolder AppDataFolder(this XtiFolder xtiFolder, AppKey appKey)
            => xtiFolder.AppDataFolder(appKey.Name.DisplayText, appKey.Type.DisplayText);

        public static string InstallPath(this XtiFolder xtiFolder, AppKey appKey)
            => xtiFolder.InstallPath(appKey.Name.DisplayText, appKey.Type.DisplayText, "");

        public static string InstallPath(this XtiFolder xtiFolder, AppKey appKey, AppVersionKey versionKey)
            => xtiFolder.InstallPath(appKey.Name.DisplayText, appKey.Type.DisplayText, versionKey.DisplayText);

        public static string PublishPath(this XtiFolder xtiFolder, AppKey appKey, AppVersionKey versionKey)
            => xtiFolder.PublishPath(appKey.Name.DisplayText, appKey.Type.DisplayText, versionKey.DisplayText);
    }
}
