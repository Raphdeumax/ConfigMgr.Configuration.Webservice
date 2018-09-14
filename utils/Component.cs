using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System.Text;


namespace ConfigMgr.Configuration.Webservice.utils
{

    /// <summary> 
    /// 
    /// </summary> 
    public class ListComponent
    {
        /// <summary> 
        /// 
        /// </summary> 
        public List<Component> Components = new List<Component>();
    }

    /// <summary> 
    /// 
    /// </summary> 
    public class Component
    {
        /// <summary> 
        /// 
        /// </summary> 
        public string SiteCode { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string Server { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string Status { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string ComponentName { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string State { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string Startup { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string Availability { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string LastStartup { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string LastStatus { get; set; }
    }

} // Fin namespace sccm.configuration.service.utils                                //