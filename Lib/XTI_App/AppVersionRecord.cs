using System;

namespace XTI_App
{
    public sealed class AppVersionRecord
    {
        public int ID { get; set; }
        public string VersionKey { get; set; } = "";
        public int AppID { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public string Description { get; set; } = "";
        public DateTime TimeAdded { get; set; } = Timestamp.MaxValue.Value;
    }
}
