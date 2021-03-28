using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Timekit.Availability
{
    /// <summary>
    /// Query for availability of resources
    /// </summary>
    public class TimekitAvailabilityRequest
    {
        public TimekitAvailabilityRequest()
        {
            this.resources = new List<string>();
        }

        /// <summary>
        /// If you're using our projects model, specify the project ID and all parameters to the endpoint will be retrieved dynamically from the project. Any additional parameters in the request are not required and will override those set in the project.
        /// </summary>
        public string project_id { get; set; }

        /// <summary>
        /// Specify the type of availability you want calculated and returned, valid modes are: "mutual", "roundrobin_random", "roundrobin_prioritized"
        /// </summary>
        public string mode { get; set; }

        /// <summary>
        /// Array of resource IDs that should be included in the availability search
        /// </summary>
        public List<string> resources { get; set; }

        ///// <summary>
        ///// Array of constraint objects that either whitelist or blacklist timespans. These constraints are dynamically applied for each resource.
        ///// </summary>
        //public List<Constraint> constraints { get; set; }

        /// <summary>
        /// How long each available time-slot should be. "Null" simply returns timeslots in dynamic intervals without a fixed length.
        /// </summary>
        public string length { get; set; }

        /// <summary>
        /// Defines the beginning of the search-space written is human language, eg. "1 day", "2 weeks" or "tomorrow"
        /// </summary>
        public string from { get; set; }

        /// <summary>
        /// Defines the end of the search-space written is human language, eg. "1 day", "2 weeks" or "tomorrow" (max 6 months)
        /// </summary>
        public string to { get; set; }

        /// <summary>
        /// The amount of buffer time you want to pad around existing events (see note)
        /// </summary>
        public string buffer { get; set; }

        /// <summary>
        /// If you want the outputted time-slots to be formatted to a particular timezone, set it with this parameter. Please note that this parameter will not affect the resource-relative timezones of the constraints. Please see our constraints section for further information.
        /// </summary>
        public string output_timezone { get; set; }

        /// <summary>
        /// Define at which time increments the time-slots should start. Please se the note
        /// </summary>
        public string timeslot_increments { get; set; }

        /// <summary>
        /// If you dont want us to round up to nearest hour (for pretty timestamps) you can set this to false
        /// </summary>
        public bool round_to_nearest_hour { get; set; }

        public string future { get; set; }

        public DateTime date { get; set; }
    }
}
