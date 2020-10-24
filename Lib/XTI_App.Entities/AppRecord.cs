using System;

namespace XTI_App.Entities
{
    public sealed class AppRecord
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public string Name { get; set; } = "xti_notfound";
        public string Title { get; set; } = "";
        public DateTime TimeAdded { get; set; }
    }
}
