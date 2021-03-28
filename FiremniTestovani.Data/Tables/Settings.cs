using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FiremniTestovani.Data.Tables
{
    public class Settings
    {
        public int SourceID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public string ValueType { get; set; }

        [ForeignKey("SourceID")]
        public virtual Source R_Source { get; set; }
    }
}
