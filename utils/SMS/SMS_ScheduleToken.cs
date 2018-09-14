using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigMgr.Configuration.Webservice.utils.SMS
{
    /// <summary>
    /// 
    /// </summary>
    public class SMS_ScheduleToken
    {
        /// <summary> 
        /// 
        /// </summary> 
        public int DayDuration { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int HourDuration { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public Boolean IsGMT { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int MinuteDuration { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public DateTime StartTime { get; set; }
    };

}