using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Timekit.Events
{
    public class TimekitEventInfo
    {
        public TimekitEventInfo()
        {
            this.participants = new List<string>();
        }

        public string calendar_id { get; set; }

        public string description { get; set; }

        public string start { get; set; }

        public string end { get; set; }

        public string what { get; set; }

        public string where { get; set; }

        public List<string> participants { get; set; }
    }
}
