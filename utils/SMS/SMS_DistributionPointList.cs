using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System.Text;

namespace ConfigMgr.Configuration.Webservice.utils.SMS
{

    /// <summary> 
    /// 
    /// </summary> 
    public class SMS_DistributionPointList
    {
        /// <summary> 
        /// 
        /// </summary> 
        public List<SMS_DistributionPoint> SMS_DistributionPoints = new List<SMS_DistributionPoint>();
    }
    /// <summary> 
    /// 
    /// </summary> 
    public class SMS_DistributionPoint
    {
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
        public string Name { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string NALPath { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string GroupID { get; set; }
    }

} // Fin namespace sccm.configuration.service.utils                                //