using System;
using XTI_Core;

namespace XTI_App.Entities
{
    public sealed class AppRequestRecord
    {
        public int ID { get; set; }
        public string RequestKey { get; set; } = "";
        public int SessionID { get; set; }
        public int VersionID { get; set; }
        public string Path { get; set; } = "";
        public int ResourceID { get; set; }
        public int ModifierID { get; set; }
        public DateTime TimeStarted { get; set; } = Timestamp.MinValue.Value;
        public DateTime TimeEnded { get; set; } = Timestamp.MaxValue.Value;
    }
}
