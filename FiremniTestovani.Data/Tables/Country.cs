using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FiremniTestovani.Data.Tables
{
    public class Country
    {
        [Key]
        public int ID { get; set; }
    
        /// <summary>
        /// Country name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Country name in localized version
        /// </summary>
        public string NameLocalized { get; set; }

        /// <summary>
        /// Country ISO code
        /// </summary>
        public string ISOCode { get; set; }

        /// <summary>
        /// Default insurance company
        /// </summary>
        public int? DefaultInsuraceID { get; set; }

        /// <summary>
        /// validation pattern for PIN (cz: rodné číslo)
        /// </summary>
        public string PersonalIdentificationNumberPattern { get; set; }

        public bool IsImportant { get; set; }
    }
}
