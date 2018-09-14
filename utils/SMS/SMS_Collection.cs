using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigMgr.Configuration.Webservice.utils.SMS
{
    public class SMS_Collection
    {
        public string CollectionID { get; set; }
        public SMS_CollectionRule[] CollectionRules { get; set; }
        public int CollectionType { get; set; }
        public int CollectionVariablesCount { get; set; }
        public string Comment { get; set; }
        public int CurrentStatus { get; set; }
        public bool HasProvisionedMember { get; set; }
        public int IncludeExcludeCollectionsCount { get; set; }
        public bool IsBuiltIn { get; set; }
        public bool IsReferenceCollection { get; set; }
        public byte[] ISVData { get; set; }
        public int ISVDataSize { get; set; }
        public DateTime LastChangeTime { get; set; }
        public DateTime LastMemberChangeTime { get; set; }
        public DateTime LastRefreshTime { get; set; }
        public string LimitToCollectionID { get; set; }
        public string LimitToCollectionName { get; set; }
        public int LocalMemberCount { get; set; }
        public string MemberClassName { get; set; }
        public int MemberCount { get; set; }
        public int MonitoringFlags { get; set; }
        public string Name { get; set; }
        public bool OwnedByThisSite { get; set; }
        public int PowerConfigsCount { get; set; }
        public SMS_ScheduleToken[] RefreshSchedule { get; set; }
        public int RefreshType { get; set; }
        public bool ReplicateToSubSites { get; set; }
        public int ServiceWindowsCount { get; set; }
    }

    public class SMS_CollectionRule
    {
        public string RuleName { get; set; }
    }
}