using System;
using ConfigMgr.Configuration.Webservice.utils;

namespace ConfigMgr.Configuration.Webservice.utils.SMS
{

    /// <summary>
    /// 
    /// </summary>
    /// <summary> 
    /// 
    /// </summary> 
    public class SMS_Package
    {

        /// <summary>
        /// 
        /// </summary>
        /// <summary> 
        /// 
        /// </summary> 
        public int ActionInProgress { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string AlternateContentProviders { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int DefaultImageFlags { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string Description { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int[] ExtendedData { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int ExtendedDataSize { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int ForcedDisconnectDelay { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public Boolean ForcedDisconnectEnabled { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int ForcedDisconnectNumRetries { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int[] Icon { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int IconSize { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public Boolean IgnoreAddressSchedule { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public Boolean IsPredefinedPackage { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int[] ISVData { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int ISVDataSize { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string Language { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public DateTime LastRefreshTime { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string[] LocalizedCategoryInstanceNames { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string Manufacturer { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string MIFFilename { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string MIFName { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string MIFPublisher { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string MIFVersion { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string Name { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int NumOfPrograms { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string PackageID { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int PackageSize { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int PackageType { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int PkgFlags { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int PkgSourceFlag { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string PkgSourcePath { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string PreferredAddressType { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int Priority { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public Boolean RefreshPkgSourceFlag { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public SMS_ScheduleToken[] RefreshSchedule { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string[] SecuredScopeNames { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string SedoObjectVersion { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string ShareName { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int ShareType { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public DateTime SourceDate { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string SourceSite { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int SourceVersion { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string StoredPkgPath { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int StoredPkgVersion { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public DateTime TransformAnalysisDate { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int TransformReadiness { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public string Version { get; set; }
        /// <summary> 
        /// 
        /// </summary> 
        public int Status { get; set; }
    }



}