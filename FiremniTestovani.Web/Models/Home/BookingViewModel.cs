using ExpressiveAnnotations.Attributes;
using FiremniTestovani.Data.Enums;
using FiremniTestovani.Models;
using FiremniTestovani.Web.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Home
{
    public class BookingViewModel
    {
        public int? BookingID { get; set; }

        public int TimeSlotID { get; set; }

        [Display(Name = "PersonalNumber")]
        [Required(ErrorMessage = "{0} is required.")]
        public string PersonalNumber { get; set; }

        [Display(Name = "FirstName")]
        [Required(ErrorMessage = "{0} is required.")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        [Required(ErrorMessage = "{0} is required.")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "{0} is required.")]
        [EmailAddress(ErrorMessage = "Vyplňte platný email.")]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(Int32.MaxValue, MinimumLength = 9, ErrorMessage = "Phone_Validation")]
        public string Phone { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "{0} is required.")]
        public string City { get; set; }

        [Display(Name = "ZIPCode")]
        [Required(ErrorMessage = "{0} is required.")]
        public string ZIPCode { get; set; }

        [Display(Name = "DateOfBirth")]
        [Required(ErrorMessage = "{0} is required.")]
        public DateTime? DateOfBirth { get; set; }

        public string DateOfBirthString { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "{0} is required.")]
        public Gender Gender { get; set; }

        [Display(Name = "Nationality")]
        [Required(ErrorMessage = "{0} is required.")]
        public string Nationality { get; set; }

        [Display(Name = "Insurance")]
        [Required(ErrorMessage = "{0} is required.")]
        public InsuranceCompany Insurance { get; set; }

        [Display(Name = "AcceptTerms")]
        [AssertThat("AcceptTerms == true", ErrorMessage = "AcceptTerms_Validation")]
        public bool AcceptTerms { get; set; }

        /// <summary>
        /// CZ: rodné číslo
        /// </summary>
        [Display(Name = "PersonalIdentificationNumber")]
        [Required(ErrorMessage = "{0} is required.")]
        //[AssertThat("(Nationality == 'CZ' && IsRegexMatch(PersonalIdentificationNumber, '^\\s*(\\d\\d)(\\d\\d)(\\d\\d)[ /]*(\\d\\d\\d)(\\d?)\\s*$')) || Nationality != 'CZ'")]
        //[AssertThat(@"(Nationality == 'CZ' && IsRegexMatch(PersonalIdentificationNumber, '^(\d\d)(\d\d)(\d\d)[ /]*(\d\d\d)(\d?)\s*$')) || Nationality != 'CZ'")]
        public string PersonalIdentificationNumber { get; set; }

        /// <summary>
        /// Date of last test (if made)
        /// </summary>
        public DateTime? LastTestMade { get; set; }

        public DateTime EarliestDateForTest { get; set; }

        public string FullName => $"{this.FirstName} {this.LastName}";

        public ApplicationBooking LastTest { get; set; }

        public bool DisplayTestHistory { get; set; }

        public bool DisplayTestResults { get; set; }

        public static BookingViewModel FromApplicationBooking(ApplicationBooking applicationBooking)
        {
            BookingViewModel model = new BookingViewModel()
            {
                BookingID = applicationBooking.TimeSlotBookingID,
                TimeSlotID = applicationBooking.TimeSlotID,
                Email = applicationBooking.Email,
                FirstName = applicationBooking.FirstName,
                LastName = applicationBooking.LastName,
                PersonalNumber = applicationBooking.EmployeeID,
                Phone = applicationBooking.Phone,
            };

            Dictionary<string, object> additionalData = JsonConvert.DeserializeObject<Dictionary<string, object>>(applicationBooking.SysAdditionalData);

            model.City = (string)additionalData.TryReturnValue("city", model.City);
            model.ZIPCode = (string)additionalData.TryReturnValue("zip", model.ZIPCode);
            model.Nationality = (string)additionalData.TryReturnValue("nationality", model.Nationality);
            //model.Gender = Enum.Parse<Gender>((string)additionalData.TryReturnValue("gender", model.Gender));
            //model.Insurance = Enum.Parse<InsuranceCompany>((string)additionalData.TryReturnValue("insurance", model.Insurance));
            model.PersonalIdentificationNumber = (string)additionalData.TryReturnValue("personal_identification_number", model.PersonalIdentificationNumber);
            //model.DateOfBirth = (DateTime)additionalData.TryReturnValue("date_of_birth", model.DateOfBirth);

            object dateOfBirth = additionalData.TryReturnValue("date_of_birth", model.DateOfBirth);
            if (dateOfBirth is string)
            {
                if (DateTime.TryParse((string)dateOfBirth, out DateTime tempDateOfBirth))
                    model.DateOfBirth = tempDateOfBirth;
            }
            else if (dateOfBirth is DateTime)
            {
                model.DateOfBirth = (DateTime)dateOfBirth;
            }

            object gender = additionalData.TryReturnValue("gender", model.Gender);
            if (gender is string)
            {
                if (Enum.TryParse<Gender>((string)gender, out Gender tempGender))
                    model.Gender = tempGender;
            }
            else if (gender is int || gender is long)
            {
                model.Gender = (Gender)Convert.ToInt32(gender);
            }

            object insuranceCompany = additionalData.TryReturnValue("insurance", model.Insurance);
            if (insuranceCompany is string)
            {
                if (Enum.TryParse<InsuranceCompany>((string)gender, out InsuranceCompany tempInsurance))
                    model.Insurance = tempInsurance;
            }
            else if (insuranceCompany is int || insuranceCompany is long)
            {
                model.Insurance = (InsuranceCompany)Convert.ToInt32(insuranceCompany);
            }

            return model;
        }
    }
}
