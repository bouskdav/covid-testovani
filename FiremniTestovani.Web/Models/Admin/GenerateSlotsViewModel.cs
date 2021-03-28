using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Admin
{
    public class GenerateSlotsViewModel
    {
        [Display(Name = "Datum")]
        public DateTime Date { get; set; }

        [Display(Name = "Čas od")]
        public DateTime From { get; set; }

        [Display(Name = "Čas do")]
        public DateTime? To { get; set; }

        [Display(Name = "Délka slotu (minut)")]
        [Range(1, 3600)]
        public double Duration { get; set; }

        [Display(Name = "Počet slotů")]
        public int? Count { get; set; }

        [Display(Name = "Počet zaměstnanců")]
        public int? EmployeeCount { get; set; }

        public SlotsGenerationMethod? GenerationMethod { get; set; }

        [Display(Name = "Kapacita jednoho slotu")]
        public int? Capacity { get; set; }

        [Display(Name = "Umožnit storno")]
        public bool? AllowSlotCancelation { get; set; }

        [Display(Name = "Vyžadovat potvrzení")]
        public bool? RequireSlotConfirmation { get; set; }
    }

    public enum SlotsGenerationMethod
    {
        EndTime = 0,
        Count = 1,
        EmployeeCount = 2
    }
}
