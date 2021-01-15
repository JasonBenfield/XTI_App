using System;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppEventModel
    {
        public int ID { get; set; }
        public int RequestID { get; set; }
        public DateTimeOffset TimeOccurred { get; set; }
        public AppEventSeverity Severity { get; set; }
        public string Caption { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
    }
}
