using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FiremniTestovani.Data.Enums
{
    public enum Roles
    {
        SuperAdmin = 3,
        Admin = 2,
        TestSite = 1,
        Basic = 0
    }

    public enum EarliestNextPossibleTest
    {
        Unlimited = 0,
        FixedDays = 1,
        FloatingWeeks = 2,
        FloatingWeeksAndThreeDays = 3
    }

    public enum Gender
    {
        [Display(Name = "Muž")]
        Male = 1,
        [Display(Name = "Žena")]
        Female = 2
    }

    public enum InsuranceCompany
    {
        [Display(Name = "[111] Všeobecná zdravotní pojišťovna ČR (VZP)")]
        VZP = 111,
        [Display(Name = "[201] Vojenská zdravotní pojišťovna ČR (VoZP)")]
        VOZP = 201,
        [Display(Name = "[205] Česká průmyslová zdravotní pojišťovna (ČPZP)")]
        CPZP = 205,
        [Display(Name = "[207] Oborová zdravotní pojišťovna zam. bank, poj. a stav. (OZP)")]
        OZP = 207,
        [Display(Name = "[209] Zaměstnanecká pojišťovna Škoda (ZPŠ)")]
        ZPS = 209,
        [Display(Name = "[211] Zdravotní pojišťovna ministerstva vnitra ČR (ZPMV)")]
        ZPMV = 211,
        [Display(Name = "[213] Revírní bratrská pokladna, zdrav. pojišťovna (RBP)")]
        RBP = 213
    }
}
