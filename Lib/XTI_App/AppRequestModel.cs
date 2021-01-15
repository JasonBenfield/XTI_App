using System;

namespace XTI_App
{
    public sealed class AppRequestModel
    {
        public int ID { get; set; }
        public int SessionID { get; set; }
        public string Path { get; set; }
        public int ResourceID { get; set; }
        public int ModifierID { get; set; }
        public DateTimeOffset TimeStarted { get; set; }
        public DateTimeOffset TimeEnded { get; set; }
    }
}
