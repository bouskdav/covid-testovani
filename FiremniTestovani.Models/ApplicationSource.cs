using FiremniTestovani.Data.Enums;
using FiremniTestovani.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Models
{
    public class ApplicationSource
    {
        public ApplicationSource(Source source)
        {
            this.SourceID = source.SourceID;
            this.Name = source.Name;
            this.Description = source.Description;
            this.ClientConfigDir = source.ClientConfigDir;
            this.URL = source.URL;
            this.InternalURL = source.InternalURL;
            this.Password = source.Password;
            this.CompanyName = source.CompanyName;
            this.CompanyStreet = source.CompanyStreet;
            this.CompanyZIP = source.CompanyZIP;
            this.CompanyCity = source.CompanyCity;
            this.TestDuration = source.TestDuration;
            this.AllowSlotCancelation = source.AllowSlotCancelation;
            this.RequireSlotConfirmation = source.RequireSlotConfirmation;
            this.DefaultCulture = source.DefaultCulture;
            this.EarliestNextPossibleTest = source.EarliestNextPossibleTest;
            this.EarliestNextPossibleTestNumericValue = source.EarliestNextPossibleTestNumericValue;
        }

        public int SourceID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ClientConfigDir { get; set; }

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

        public EarliestNextPossibleTest EarliestNextPossibleTest { get; set; }

        public double? EarliestNextPossibleTestNumericValue { get; set; }

        public string GetClientConfigDir() => this.ClientConfigDir ?? "default";
    }
}
