using FiremniTestovani.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FiremniTestovani.Data.Tables
{
    public class Source
    {
        [Key]
        public int SourceID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string URL { get; set; }

        public string InternalURL { get; set; }

        public string Password { get; set; }

        public string CompanyName { get; set; }

        public string CompanyStreet { get; set; }

        public string CompanyZIP { get; set; }

        public string CompanyCity { get; set; }

        public double TestDuration { get; set; }

        public bool AllowSlotCancelation { get; set; }

        public bool RequireSlotConfirmation { get; set; }

        public string DefaultCulture { get; set; }

        public string ClientConfigDir { get; set; }

        public EarliestNextPossibleTest EarliestNextPossibleTest { get; set; }

        public double? EarliestNextPossibleTestNumericValue { get; set; }

        public virtual ICollection<TimeSlot> C_TimeSlots { get; set; }

        public virtual ICollection<Settings> C_Settings { get; set; }
    }
}
