using System;
using XTI_Core;

namespace XTI_App.Entities
{
    public sealed class AppUserRecord
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime TimeAdded { get; set; } = Timestamp.MaxValue.Value;
    }
}
