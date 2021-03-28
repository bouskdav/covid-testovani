using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models
{
    public class SMSSettings
    {
        public bool EnableSMSNotifications { get; set; }

        public string Mail { get; set; }

        public string SMTPUser { get; set; }

        public string SMTPPassword { get; set; }

        public string SMTPHost { get; set; }

        public int SMTPPort { get; set; }
    }
}
