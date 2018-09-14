using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigMgr.Configuration.Webservice.utils.SMS
{
    /// <summary>
    /// 
    /// </summary>
    public class SMS_ADSite
    {
        public string ADSiteDescription{ get; set; }
        public string ADSiteLocation{ get; set; }
        public string ADSiteName{ get; set; }
        public int Flags{ get; set; }
        public int ForestID{ get; set; }
        public DateTime LastDiscoveryTime{ get; set; }
        public int SiteID{ get; set; }
        public SMS_ADForest Forests { get; set; }
        public  List<SMS_ADSubnet> Subnets{ get; set; }
        public List<SMS_ADDomain> Domains { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SMS_ADSubnet
    {
        public string ADSubnetDescription{ get; set; }
        public string ADSubnetLocation{ get; set; }
        public string ADSubnetName{ get; set; }
        public int Flags{ get; set; }
        public int ForestID{ get; set; }
        public DateTime LastDiscoveryTime{ get; set; }
        public int SiteID{ get; set; }
        public int SubnetID{ get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SMS_ADForest
    {
        public string Account{ get; set; }
        public string CreatedBy{ get; set; }
        public DateTime CreatedOn{ get; set; }
        public string Description{ get; set; }
        public int DiscoveredADSites{ get; set; }
        public int DiscoveredDomains{ get; set; }
        public int DiscoveredIPSubnets{ get; set; }
        public int DiscoveredTrusts{ get; set; }
        public int DiscoveryStatus{ get; set; }
        public bool EnableDiscovery{ get; set; }
        public string ForestFQDN{ get; set; }
        public int ForestID{ get; set; }
        public string ModifiedBy{ get; set; }
        public DateTime ModifiedOn{ get; set; }
        public string PublishingPath{ get; set; }
        public int PublishingStatus{ get; set; }
    };


    public class SMS_ADDomain
    {
        public int DomainID { get; set; }
        public string DomainMode { get; set; }
        public string DomainName { get; set; }
        public int Flags { get; set; }
        public int ForestID { get; set; }
        public DateTime LastDiscoveryTime { get; set; }
    };

}