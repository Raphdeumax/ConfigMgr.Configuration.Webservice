using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConfigMgr.Configuration.Webservice.utils;
using ConfigMgr.Configuration.Webservice.utils.SMS;

namespace ConfigMgr.Configuration.Webservice.utils
{
    /// <summary>
    /// 
    /// </summary>
    public class ReturnsPackages
    {
        /// <summary>
        /// 
        /// </summary>
        public int ReturnCode;

        /// <summary>
        /// 
        /// </summary>
        public string Message;

        /// <summary>
        /// 
        /// </summary>
        public List<SMS_Package> ListPackage;
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReturnsBoundaries
    {
        /// <summary>
        /// 
        /// </summary>
        public int ReturnCode;

        /// <summary>
        /// 
        /// </summary>
        public string Message;

        /// <summary>
        /// 
        /// </summary>
        public List<SMS_Package> ListPackage;
    }
}