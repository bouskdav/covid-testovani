using FiremniTestovani.Models;
using FiremniTestovani.Web.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Timekit.Booking
{
    public class TimekitBookingRequest
    {
        public Dictionary<string, JsonElement> customer { get; set; }

        public string description { get; set; }

        public DateTime start { get; set; }

        public DateTime end { get; set; }

        public string graph { get; set; }

        public string resource_id { get; set; }

        public string what { get; set; }

        public string where { get; set; }

        public Dictionary<string, object> Calc_EmployeeData
        {
            get
            {
                Dictionary<string, object> temp = new Dictionary<string, object>();

                foreach (var item in this.customer)
                {
                    temp.Add(item.Key, item.Value.GetValueAsObject());
                }

                return temp;
            }
        }

        internal ApplicationBooking AsNewApplicationBooking()
        {
            Dictionary<string, object> temp = this.Calc_EmployeeData;
            //Dictionary<string, object> temp = this.Calc_EmployeeData.ToDictionary(i => i.Key, i => i.Value);

            // parse employee data
            temp.TryGetValue("name", out object name);
            temp.TryGetValue("surname", out object surname);
            temp.TryGetValue("email", out object email);
            temp.TryGetValue("phone", out object phone);
            temp.TryGetValue("personal_number", out object personalNumber);
            
            if (!temp.ContainsKey("data_version"))
            {
                temp.Add("data_version", "v1");
            }

            string employeeData = JsonConvert.SerializeObject(temp);

            ApplicationBooking applicationBooking = new ApplicationBooking()
            {
                TimeSlotID = Convert.ToInt32(this.resource_id),
                FromExpected = start,
                ToExpected = end,

                Name = $"{name} {surname}",
                FirstName = (string)name,
                LastName = (string)surname,
                Email = (string)email,
                Phone = (string)phone,
                EmployeeID = (string)personalNumber,
                SysAdditionalData = employeeData
            };

            return applicationBooking;
        }
    }
}
