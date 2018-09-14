using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigMgr.Configuration.Webservice.utils.SMS
{
    public class SMS_TaskSequencePackage
    {
        public int ActionInProgress;
        public string AlternateContentProviders;
        public string BootImageID;
        public string Category;
        public string CustomProgressMsg;
        public string DependentProgram;
        public string Description;
        public int Duration;
        public byte[] ExtendedData;
        public int ExtendedDataSize;
        public int ForcedDisconnectDelay;
        public bool ForcedDisconnectEnabled;
        public int ForcedDisconnectNumRetries;
        public byte[] Icon;
        public int IconSize;
        public bool IgnoreAddressSchedule;
        public byte[] ISVData;
        public int ISVDataSize;
        public string Language;
        public DateTime LastRefreshTime;
        public string[] LocalizedCategoryInstanceNames;
        public string Manufacturer;
        public string MIFFilename;
        public string MIFName;
        public string MIFPublisher;
        public string MIFVersion;
        public string Name;
        public int NumOfPrograms;
        public string PackageID;
        public int PackageSize;
        public int PackageType;
        public int PkgFlags;
        public int PkgSourceFlag;
        public string PkgSourcePath;
        public string PreferredAddressType;
        public int Priority;
        public int ProgramFlags;
        public SMS_TaskSequence_Reference[] References;
        public bool RefreshPkgSourceFlag;
        public SMS_ScheduleToken[] RefreshSchedule;
        public string[] SecuredScopeNames;
        public string SedoObjectVersion;
        public int ReferencesCount;
        public string Reserved;
        public string Sequence;
        public string ShareName;
        public int ShareType;
        public DateTime SourceDate;
        public string SourceSite;
        public int SourceVersion;
        public string StoredPkgPath;
        public int StoredPkgVersion;
        public SMS_OS_Details[] SupportedOperatingSystems;
        public int TaskSequenceFlags;
        public int Type;
        public string Version;
    };

    public class SMS_TaskSequence_Reference
    {
        public string Package;
        public string Program;
        public int Type;
    };

    public class SMS_OS_Details
    {
        public string MaxVersion;
        public string MinVersion;
        public string Name;
        public string Platform;
    };


}