using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class AppRequestExpandedModel
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string GroupName { get; set; }
        public string ActionName { get; set; }
        public ResourceResultType ResultType { get; set; }
        public DateTimeOffset TimeStarted { get; set; }
        public DateTimeOffset TimeEnded { get; set; }
    }
}
