using FiremniTestovani.Data.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiremniTestovani.Models
{
    public class ApplicationSettings
    {
        public ApplicationSettings(Settings settings)
        {
            //this.ID = settings.ID;
            this.SourceID = settings.SourceID;
            this.Name = settings.Name;
            this.Description = settings.Description;
            this.Value = settings.Value;
            this.ValueType = settings.ValueType;
        }

        //public int ID { get; set; }

        public int SourceID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public string ValueType { get; set; }
    }
}
