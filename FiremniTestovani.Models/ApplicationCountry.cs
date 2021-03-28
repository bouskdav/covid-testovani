using FiremniTestovani.Data.Tables;
using FiremniTestovani.Models.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiremniTestovani.Models
{
    public class ApplicationCountry : IApplicationEnum
    {
        public ApplicationCountry(Country country)
        {
            this.ID = country.ID;
            this.Name = country.Name;
            this.NameLocalized = country.NameLocalized;
            this.ISOCode = country.ISOCode;
            this.DefaultInsuraceID = country.DefaultInsuraceID;
            this.PersonalIdentificationNumberPattern = country.PersonalIdentificationNumberPattern;
            this.IsImportant = country.IsImportant;
        }

        public int ID { get; set; }

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

        public string Value => this.ID.ToString();

        public string Text => this.NameLocalized ?? this.Name;
    }
}
