using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FiremniTestovani.Data.Tables
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? SourceID { get; set; }

        [ForeignKey("SourceID")]
        public virtual Source R_Source { get; set; }
    }
}
