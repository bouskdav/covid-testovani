using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Home
{
    public class PersonalDataViewModel
    {
        [Display(Name = "PersonalNumber")]
        [Required(ErrorMessage = "{0} is required")]
        public string PersonalNumber { get; set; }

        [Display(Name = "FirstName")]
        [Required(ErrorMessage = "{0} is required")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        [Required(ErrorMessage = "{0} is required")]
        public string LastName { get; set; }

        public bool DisplayCardImage { get; set; }

        public bool DisplayCardNotification { get; set; }
    }
}
