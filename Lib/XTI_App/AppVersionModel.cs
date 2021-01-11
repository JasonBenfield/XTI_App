using System;

namespace XTI_App
{
    public sealed class AppVersionModel
    {
        public int ID { get; set; }
        public string VersionKey { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public AppVersionType VersionType { get; set; }
        public AppVersionStatus Status { get; set; }
        public DateTimeOffset TimeAdded { get; set; }
    }
}
