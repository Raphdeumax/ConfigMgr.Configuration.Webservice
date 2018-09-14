using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigMgr.Configuration.Webservice.utils.SMS
{
    /// <summary>
    /// 
    /// </summary>
    public class SMS_Application
    {

        /// <summary> 
        /// 
        /// </summary> 
        public string ApplicabilityCondition;
        /// <summary> 
        /// 
        /// </summary> 
        public string[] CategoryInstance_UniqueIDs;
        /// <summary> 
        /// 
        /// </summary> 
        public int CI_ID;
        /// <summary> 
        /// 
        /// </summary> 
        public string CI_UniqueID;
        /// <summary> 
        /// 
        /// </summary> 
        public int CIType_ID;
        /// <summary> 
        /// 
        /// </summary> 
        public int CIVersion;
        /// <summary> 
        /// 
        /// </summary> 
        public string CreatedBy;
        /// <summary> 
        /// 
        /// </summary> 
        public DateTime DateCreated;
        /// <summary> 
        /// 
        /// </summary> 
        public DateTime DateLastModified;
        /// <summary> 
        /// 
        /// </summary> 
        public DateTime EffectiveDate;
        /// <summary> 
        /// 
        /// </summary> 
        public int EULAAccepted;
        /// <summary> 
        /// 
        /// </summary> 
        public bool EULAExists;
        /// <summary> 
        /// 
        /// </summary> 
        public DateTime EULASignoffDate;
        /// <summary> 
        /// 
        /// </summary> 
        public string EULASignoffUser;
        /// <summary> 
        /// 
        /// </summary> 
        public int ExecutionContext;
        /// <summary> 
        /// 
        /// </summary> 
        public int Featured;
        /// <summary> 
        /// 
        /// </summary> 
        public bool HasContent;
        /// <summary> 
        /// 
        /// </summary> 
        public bool IsBundle;
        /// <summary> 
        /// 
        /// </summary> 
        public bool IsDeployable;
        /// <summary> 
        /// 
        /// </summary> 
        public bool IsDeployed;
        /// <summary> 
        /// 
        /// </summary> 
        public bool IsDigest;
        /// <summary> 
        /// 
        /// </summary> 
        public bool IsEnabled;
        /// <summary> 
        /// 
        /// </summary> 
        public bool IsExpired;
        /// <summary> 
        /// 
        /// </summary> 
        public bool IsHidden;
        /// <summary> 
        /// 
        /// </summary> 
        public bool IsLatest;
        /// <summary> 
        /// 
        /// </summary> 
        public bool IsQuarantined;
        /// <summary> 
        /// 
        /// </summary> 
        public bool IsSuperseded;
        /// <summary> 
        /// 
        /// </summary> 
        public bool IsSuperseding;
        /// <summary> 
        /// 
        /// </summary> 
        public bool IsUserDefined;
        /// <summary> 
        /// 
        /// </summary> 
        public string LastModifiedBy;
        /// <summary> 
        /// 
        /// </summary> 
        public string[] LocalizedCategoryInstanceNames;
        /// <summary> 
        /// 
        /// </summary> 
        public string LocalizedDescription;
        /// <summary> 
        /// 
        /// </summary> 
        public string LocalizedDisplayName;
        /// <summary> 
        /// 
        /// </summary> 
        public string LocalizedInformativeURL;
        /// <summary> 
        /// 
        /// </summary> 
        public int LocalizedPropertyLocaleID;
        /// <summary> 
        /// 
        /// </summary> 
        public int LogonRequirement;
        /// <summary> 
        /// 
        /// </summary> 
        public string Manufacturer;
        /// <summary> 
        /// 
        /// </summary> 
        public string ModelName;
        /// <summary> 
        /// 
        /// </summary> 
        public int ModelID;
        /// <summary> 
        /// 
        /// </summary> 
        public int NumberOfDependentDTs;
        /// <summary> 
        /// 
        /// </summary> 
        public int NumberOfDependentTS;
        /// <summary> 
        /// 
        /// </summary> 
        public int NumberOfDeployments;
        /// <summary> 
        /// 
        /// </summary> 
        public int NumberOfDeploymentTypes;
        /// <summary> 
        /// 
        /// </summary> 
        public int NumberOfDevicesWithApp;
        /// <summary> 
        /// 
        /// </summary> 
        public int NumberOfDevicesWithFailure;
        /// <summary> 
        /// 
        /// </summary> 
        public int NumberOfUsersWithApp;
        /// <summary> 
        /// 
        /// </summary> 
        public int NumberOfUsersWithFailure;
        /// <summary> 
        /// 
        /// </summary> 
        public int NumberOfUsersWithRequest;
        /// <summary> 
        /// 
        /// </summary> 
        public int NumberOfVirtualEnvironments;
        /// <summary> 
        /// 
        /// </summary> 
        public string PackageID;
        /// <summary> 
        /// 
        /// </summary> 
        public int PermittedUses;
        /// <summary> 
        /// 
        /// </summary> 
        public string[] PlatformCategoryInstance_UniqueIDs;
        /// <summary> 
        /// 
        /// </summary> 
        public int PlatformType;
        /// <summary> 
        /// 
        /// </summary> 
        public SMS_SDMPackageLocalizedData[] SDMPackageLocalizedData;
        /// <summary> 
        /// 
        /// </summary> 
        public int SDMPackageVersion;
        /// <summary> 
        /// 
        /// </summary> 
        public string SDMPackageXML;
        /// <summary> 
        /// 
        /// </summary> 
        public string[] SecuredScopeNames;
        /// <summary> 
        /// 
        /// </summary> 
        public string SedoObjectVersion;
        /// <summary> 
        /// 
        /// </summary> 
        public string SoftwareVersion;
        /// <summary> 
        /// 
        /// </summary> 
        public int SourceCIVersion;
        /// <summary> 
        /// 
        /// </summary> 
        public string SourceModelName;
        /// <summary> 
        /// 
        /// </summary> 
        public string SourceSite;
        /// <summary> 
        /// 
        /// </summary> 
        public DateTime SummarizationTime;
    };


    /// <summary> 
    /// 
    /// </summary> 
    public class SMS_SDMPackageLocalizedData
    {
        /// <summary> 
        /// 
        /// </summary> 
        public int LocaleID;
        /// <summary> 
        /// 
        /// </summary> 
        public string LocalizedData;
    };





}