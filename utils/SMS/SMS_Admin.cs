using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigMgr.Configuration.Webservice.utils.SMS
{
    public class SMS_Admin
    {
        public int AccountType { get; set; }
        public int AdminID { get; set; }
        public string AdminSid { get; set; }
        public string[] Categories { get; set; }
        public string[] CategoryNames { get; set; }
        public string[] CollectionNames { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public bool IsCovered { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsGroup { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LogonName { get; set; }
        public SMS_APermission[] Permissions { get; set; }
        public string[] RoleNames { get; set; }
        public string[] Roles { get; set; }
        public string SKey { get; set; }
        public string SourceSite { get; set; }
    }
}

public class SMS_APermission
{
    public string CategoryID { get; set; }
    public string CategoryName { get; set; }
    public int CategoryTypeID { get; set; }
    public string RoleID { get; set; }
    public string RoleName { get; set; }
}