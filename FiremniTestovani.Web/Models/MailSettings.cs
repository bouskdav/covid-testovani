using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models
{
    public class MailSettings
    {
        /// <summary>
        /// General switch for emails
        /// </summary>
        public bool EnableMailNotifications { get; set; }
        /// <summary>
        /// MailDeliveryOptions - PickupDelivery / SMTP
        /// </summary>
        public string MailDelivery { get; set; }
        /// <summary>
        /// PickupDerectoryPath - path for sending the emails
        /// </summary>
        public string PickupDirectoryPath { get; set; }
        /// <summary>
        /// Sender - email from
        /// </summary>
        public string Mail { get; set; }
        /// <summary>
        /// Sender - display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// SMTP user
        /// </summary>
        public string SMTPUser { get; set; }
        /// <summary>
        /// SMTP password
        /// </summary>
        public string SMTPPassword { get; set; }
        /// <summary>
        /// SMTP host
        /// </summary>
        public string SMTPHost { get; set; }
        /// <summary>
        /// SMTP port
        /// </summary>
        public int SMTPPort { get; set; }

        /// <summary>
        /// Whether to enable saving sent emails
        /// </summary>
        public bool EnableIMAP { get; set; }
        /// <summary>
        /// IMAP user
        /// </summary>
        public string IMAPUser { get; set; }
        /// <summary>
        /// IMAP password
        /// </summary>
        public string IMAPPassword { get; set; }
        /// <summary>
        /// IMAP host
        /// </summary>
        public string IMAPHost { get; set; }
        /// <summary>
        /// IMAP port
        /// </summary>
        public int IMAPPort { get; set; }
    }
}
