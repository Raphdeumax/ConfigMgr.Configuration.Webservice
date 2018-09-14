using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigMgr.Configuration.Webservice.utils.SMS
{
    /// <summary> 
    /// 
    /// </summary> 
    public class SMS_Site
    {

        /// <summary> 
        /// 
        /// </summary> 
        public int BuildNumber { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string Features { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string InstallDir { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public SiteMode Mode { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string ReportingSiteCode { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int RequestedStatus { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string ServerName { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string SiteCode { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string SiteName { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public SiteStatus Status { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string TimeZoneInfo { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public SiteType Type { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string Version { get; set; }

        /// <summary> 
        /// 
        /// </summary> 
        public enum SiteMode
        {
            /// <summary>
            /// 
            /// </summary>
            Normal,
            /// <summary>
            /// 
            /// </summary>
            Maintenance,
            /// <summary>
            /// 
            /// </summary>
            Recovery,
            /// <summary>
            /// 
            /// </summary>
            Upgrade
        }

        /// <summary> 
        /// 
        /// </summary> 
        public enum SiteStatus
        {
            /// <summary>
            /// 
            /// </summary>
            UNKOWN,
            /// <summary>
            /// 
            /// </summary>
            ACTIVE,
            /// <summary>
            /// 
            /// </summary>
            PENDING,
            /// <summary>
            /// 
            /// </summary>
            FAILED,
            /// <summary>
            /// 
            /// </summary>
            DELETED,
            /// <summary>
            /// 
            /// </summary>
            UPGRADE
        }

        /// <summary> 
        /// 
        /// </summary> 
        public enum SiteType
        {
            /// <summary>
            /// 
            /// </summary>
            UNKOWN,
            /// <summary>
            /// 
            /// </summary>
            SECONDARY,
            /// <summary>
            /// 
            /// </summary>
            PRIMARY,
            /// <summary>
            /// 
            /// </summary>
            CAS = 4,
        }

    }
}