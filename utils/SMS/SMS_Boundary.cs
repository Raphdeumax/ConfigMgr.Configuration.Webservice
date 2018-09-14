using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ConfigurationManagement.ManagementProvider;


namespace ConfigMgr.Configuration.Webservice.utils.SMS
{

    /// <summary>
    /// 
    /// </summary>
    public class SMS_BoundaryList
    {
        /// <summary>
        /// 
        /// </summary>
        public List<SMS_Boundary> SMS_Boundaries = new List<SMS_Boundary>();
    }



    /// <summary>
    /// 
    /// </summary>
    public class SMS_Boundary
    {



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Boundary"></param>
        /// <returns></returns>
        public SMS_Boundary BoundaryFromIresultObject(IResultObject Boundary)
        {
            return new SMS_Boundary()
            {
                BoundaryFlags = (SMS_Boundary.Flags)Boundary["BoundaryFlags"].IntegerValue,
                BoundaryID = Boundary["BoundaryID"].IntegerValue,
                BoundaryType = (SMS_Boundary.Type)Boundary["BoundaryType"].IntegerValue,
                CreatedBy = Boundary["CreatedBy"].StringValue,
                CreatedOn = Boundary["CreatedOn"].DateTimeValue,
                DefaultSiteCode = Boundary["DefaultSiteCode"].StringArrayValue,
                DisplayName = Boundary["DisplayName"].StringValue,
                GroupCount = Boundary["GroupCount"].IntegerValue,
                ModifiedBy = Boundary["ModifiedBy"].StringValue,
                ModifiedOn = Boundary["ModifiedOn"].DateTimeValue,
                SiteSystems = Boundary["SiteSystems"].StringArrayValue,
                Value = Boundary["Value"].StringValue
            };
        }


        /// <summary>
        /// Specifies the connection type of the boundary.
        /// </summary>
        public Flags BoundaryFlags { get; set; }

        /// <summary>
        /// Connection flag to define boundary network
        /// </summary>
        public enum Flags
        {
            /// <summary>
            /// 
            /// </summary>
            FAST = 0,
            /// <summary>
            /// 
            /// </summary>
            SLOW = 1
        }

        /// <summary>
        /// Unique identifier of the boundary. 
        /// </summary>
        public int BoundaryID { get; set; }

        /// <summary>
        /// Boundary type. 
        /// </summary>
        public Type BoundaryType { get; set; }

        /// <summary>
        /// Type define for a boundary
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 
            /// </summary>    
            IPSUBNET = 0,
            /// <summary>
            /// 
            /// </summary>
            ADSITE = 1,
            /// <summary>
            /// 
            /// </summary>
            IPV6PREFIX = 2,
            /// <summary>
            /// 
            /// </summary>
            IPRANGE = 3
        }

        /// <summary>
        /// User that created the boundary. 
        /// </summary>
        public String CreatedBy { get; set; }

        /// <summary>
        /// Date the boundary was created. 
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Site code new clients will be auto assigned to. 
        /// </summary>
        public String[] DefaultSiteCode { get; set; }

        /// <summary>
        /// Display name of the boundary. 
        /// </summary>
        public String DisplayName { get; set; }

        /// <summary>
        /// Count of boundary groups that have this boundary. 
        /// </summary>
        public int GroupCount { get; set; }

        /// <summary>
        /// User that last modified the boundary. 
        /// </summary>
        public String ModifiedBy { get; set; }

        /// <summary>
        /// Date the boundary was last modified. 
        /// </summary>
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// Site system machines within the boundary. 
        /// </summary>
        public String[] SiteSystems { get; set; }

        /// <summary>
        /// Boundary value.
        /// </summary>
        public String Value { get; set; }
    };

}