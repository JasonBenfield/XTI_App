using System;
using XTI_Core;

namespace XTI_App.Entities
{
    public sealed class AppSessionRecord
    {
        public int ID { get; set; }
        public string SessionKey { get; set; } = "";
        public int UserID { get; set; }
        public string RequesterKey { get; set; } = "";
        public DateTime TimeStarted { get; set; } = Timestamp.MinValue.Value;
        public DateTime TimeEnded { get; set; } = Timestamp.MaxValue.Value;
        public string RemoteAddress { get; set; } = "";
        public string UserAgent { get; set; } = "";
    }
}
