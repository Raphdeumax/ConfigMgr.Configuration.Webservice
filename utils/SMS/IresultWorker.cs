using System;

using System.Linq;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.AdminConsole.Schema;
using System.Management;

namespace ConfigMgr.Configuration.Webservice.utils.SMS
{
    /// <summary>
    /// 
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int IfNullThenZero(this object value)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return (int)value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string IfNullThenNull(this object value)
        {
            if (value == null)
            {
                return "";
            }
            else
            {
                return (string)value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string IfNullThenDateTime(this object value)
        {
            if (value == null)
            {
                return DateTime.MinValue.ToString("yyyyMMddHHmmss.000000+***");
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string[] IfNullThenArrayString(this object value)
        {
            if (value == null)
            {
                return new string[0];
            }
            else
            {
                return (string[])value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int[] IfNullThenArrayInteger(this object value)
        {
            if (value == null)
            {
                return new int[0];
            }
            else
            {
                return (int[])value;
            }
        }


    }

    /// <summary>
    /// 
    /// </summary>
    public static class IresultWorker
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SystemOS"></param>
        /// <returns></returns>
        public static SMS_G_System_OS SystemOSFromIresultObject(IResultObject SystemOS)
        {

            SystemOS = CompleteIresultOject(SystemOS);

            return new SMS_G_System_OS()
            {
                BootDevice = SystemOS["BootDevice"].StringValue,
                BuildNumber = SystemOS["BuildNumber"].StringValue,
                BuildType = SystemOS["BuildType"].StringValue,
                Caption = SystemOS["Caption"].StringValue,
                CodeSet = SystemOS["CodeSet"].StringValue,
                CountryCode = SystemOS["CountryCode"].StringValue,
                CSDVersion = SystemOS["CSDVersion"].StringValue,
                CurrentTimeZone = SystemOS["CurrentTimeZone"].IntegerValue,
                Debug = SystemOS["Debug"].IntegerValue,
                Description = SystemOS["Description"].StringValue,
                Distributed = SystemOS["Distributed"].IntegerValue,
                ForegroundApplicationBoost = SystemOS["ForegroundApplicationBoost"].IntegerValue,
                FreePhysicalMemory = SystemOS["FreePhysicalMemory"].IntegerValue,
                FreeSpaceInPagingFiles = SystemOS["FreeSpaceInPagingFiles"].IntegerValue,
                FreeVirtualMemory = SystemOS["FreeVirtualMemory"].IntegerValue,
                GroupID = SystemOS["GroupID"].IntegerValue,
                InstallDate = SystemOS["InstallDate"].DateTimeValue,
                LastBootUpTime = SystemOS["LastBootUpTime"].DateTimeValue,
                LocalDateTime = SystemOS["LocalDateTime"].DateTimeValue,
                Locale = SystemOS["Locale"].StringValue,
                Manufacturer = SystemOS["Manufacturer"].StringValue,
                MaxNumberOfProcesses = SystemOS["MaxNumberOfProcesses"].IntegerValue,
                MaxProcessMemorySize = SystemOS["MaxProcessMemorySize"].IntegerValue,
                MUILanguages = SystemOS["MUILanguages"].StringValue,
                Name = SystemOS["Name"].StringValue,
                NumberOfLicensedUsers = SystemOS["NumberOfLicensedUsers"].IntegerValue,
                NumberOfProcesses = SystemOS["NumberOfProcesses"].IntegerValue,
                NumberOfUsers = SystemOS["NumberOfUsers"].IntegerValue,
                OperatingSystemSKU = SystemOS["OperatingSystemSKU"].IntegerValue,
                Organization = SystemOS["Organization"].StringValue,
                OSArchitecture = SystemOS["OSArchitecture"].StringValue,
                OSLanguage = SystemOS["OSLanguage"].IntegerValue,
                OSProductSuite = SystemOS["OSProductSuite"].IntegerValue,
                OSType = SystemOS["OSType"].IntegerValue,
                OtherTypeDescription = SystemOS["OtherTypeDescription"].StringValue,
                PlusProductID = SystemOS["PlusProductID"].StringValue,
                PlusVersionNumber = SystemOS["PlusVersionNumber"].StringValue,
                Primary = SystemOS["Primary"].IntegerValue,
                ProductType = SystemOS["ProductType"].IntegerValue,
                RegisteredUser = SystemOS["RegisteredUser"].StringValue,
                ResourceID = SystemOS["ResourceID"].IntegerValue,
                RevisionID = SystemOS["RevisionID"].IntegerValue,
                SerialNumber = SystemOS["SerialNumber"].StringValue,
                ServicePackMajorVersion = SystemOS["ServicePackMajorVersion"].IntegerValue,
                ServicePackMinorVersion = SystemOS["ServicePackMinorVersion"].IntegerValue,
                SizeStoredInPagingFiles = SystemOS["SizeStoredInPagingFiles"].IntegerValue,
                Status = SystemOS["Status"].StringValue,
                SystemDevice = SystemOS["SystemDevice"].StringValue,
                SystemDirectory = SystemOS["SystemDirectory"].StringValue,
                TimeStamp = SystemOS["TimeStamp"].DateTimeValue,
                TotalSwapSpaceSize = SystemOS["TotalSwapSpaceSize"].IntegerValue,
                TotalVirtualMemorySize = SystemOS["TotalVirtualMemorySize"].IntegerValue,
                TotalVisibleMemorySize = SystemOS["TotalVisibleMemorySize"].IntegerValue,
                Version = SystemOS["Version"].StringValue,
                WindowsDirectory = SystemOS["WindowsDirectory"].StringValue,
            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Boundary"></param>
        /// <returns></returns>
        public static SMS_Boundary BoundaryFromIresultObject(IResultObject Boundary)
        {

            Boundary = CompleteIresultOject(Boundary);

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
        /// 
        /// </summary>
        /// <param name="Admin"></param>
        /// <returns></returns>
        public static SMS_Admin AdminFromIresultObject(IResultObject Admin)
        {
            Admin.Get();
            Admin = CompleteIresultOject(Admin);
            return new SMS_Admin()
            {

                AccountType = Admin["AccountType"].IntegerValue,
                AdminID = Admin["AdminID"].IntegerValue,
                AdminSid = Admin["AdminSid"].StringValue,
                Categories = Admin["Categories"].StringArrayValue,
                CategoryNames = Admin["CategoryNames"].StringArrayValue,
                CollectionNames = Admin["CollectionNames"].StringArrayValue,
                CreatedBy = Admin["CreatedBy"].StringValue,
                CreatedDate = Admin["CreatedDate"].DateTimeValue,
                DisplayName = Admin["DisplayName"].StringValue,
                DistinguishedName = Admin["DistinguishedName"].StringValue,
                IsCovered = Admin["IsCovered"].BooleanValue,
                IsDeleted = Admin["IsDeleted"].BooleanValue,
                IsGroup = Admin["IsGroup"].BooleanValue,
                LastModifiedBy = Admin["LastModifiedBy"].StringValue,
                LastModifiedDate = Admin["LastModifiedDate"].DateTimeValue,
                LogonName = Admin["LogonName"].StringValue,
                Permissions = APermissionFromIresultObject((ManagementBaseObject[])Admin["Permissions"].ObjectArrayValue),
                RoleNames = Admin["RoleNames"].StringArrayValue,
                Roles = Admin["Roles"].StringArrayValue,
                SKey = Admin["SKey"].StringValue,
                SourceSite = Admin["SourceSite"].StringValue,

            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Application"></param>
        /// <returns></returns>
        public static SMS_ADSite ADSiteFromIresultObject(IResultObject Application)
        {
            Application = CompleteIresultOject(Application);
            return new SMS_ADSite()
            {

                ADSiteDescription = Application["ADSiteDescription"].StringValue,
                ADSiteLocation = Application["ADSiteLocation"].StringValue,
                ADSiteName = Application["ADSiteName"].StringValue,
                Flags = Application["Flags"].IntegerValue,
                ForestID = Application["ForestID"].IntegerValue,
                LastDiscoveryTime = Application["LastDiscoveryTime"].DateTimeValue,
                SiteID = Application["SiteID"].IntegerValue,

            };
        }


        public static SMS_ADDomain ADDomainFromIresultObject(IResultObject Application)
        {
            Application = CompleteIresultOject(Application);
            return new SMS_ADDomain()
            {
                DomainID = Application["DomainID"].IntegerValue,
                DomainMode = Application["DomainMode"].StringValue,
                DomainName = Application["DomainName"].StringValue,
                Flags = Application["Flags"].IntegerValue,
                ForestID = Application["ForestID"].IntegerValue,
                LastDiscoveryTime = Application["LastDiscoveryTime"].DateTimeValue,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Application"></param>
        /// <returns></returns>
        public static SMS_ADSubnet ADSubnetFromIresultObject(IResultObject Application)
        {
            Application = CompleteIresultOject(Application);
            return new SMS_ADSubnet()
            {

                ADSubnetDescription = Application["ADSubnetDescription"].StringValue,
                ADSubnetLocation = Application["ADSubnetLocation"].StringValue,
                ADSubnetName = Application["ADSubnetName"].StringValue,
                Flags = Application["Flags"].IntegerValue,
                ForestID = Application["ForestID"].IntegerValue,
                LastDiscoveryTime = Application["LastDiscoveryTime"].DateTimeValue,
                SiteID = Application["SiteID"].IntegerValue,
                SubnetID = Application["SubnetID"].IntegerValue,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Application"></param>
        /// <returns></returns>
        public static SMS_ADForest ADForestFromIresultObject(IResultObject Application)
        {
            Application = CompleteIresultOject(Application);
            return new SMS_ADForest()
            {
                Account = Application["Account"].StringValue,
                CreatedBy = Application["CreatedBy"].StringValue,
                CreatedOn = Application["CreatedOn"].DateTimeValue,
                Description = Application["Description"].StringValue,
                DiscoveredADSites = Application["DiscoveredADSites"].IntegerValue,
                DiscoveredDomains = Application["DiscoveredDomains"].IntegerValue,
                DiscoveredIPSubnets = Application["DiscoveredIPSubnets"].IntegerValue,
                DiscoveredTrusts = Application["DiscoveredTrusts"].IntegerValue,
                DiscoveryStatus = Application["DiscoveryStatus"].IntegerValue,
                EnableDiscovery = Application["EnableDiscovery"].BooleanValue,
                ForestFQDN = Application["ForestFQDN"].StringValue,
                ForestID = Application["ForestID"].IntegerValue,
                ModifiedBy = Application["ModifiedBy"].StringValue,
                ModifiedOn = Application["ModifiedOn"].DateTimeValue,
                PublishingPath = Application["PublishingPath"].StringValue,
                PublishingStatus = Application["PublishingStatus"].IntegerValue,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Site"></param>
        /// <returns></returns>
        public static SMS_Site SiteFromIresultObject(IResultObject Site)
        {

            Site.Get();
            Site = CompleteIresultOject(Site);

            return new SMS_Site()
            {

                BuildNumber = Site["BuildNumber"].IntegerValue,
                Features = Site["Features"].StringValue,
                InstallDir = Site["InstallDir"].StringValue,
                Mode = (SMS_Site.SiteMode)Site["Mode"].IntegerValue,
                ReportingSiteCode = Site["ReportingSiteCode"].StringValue,
                RequestedStatus = Site["RequestedStatus"].IntegerValue,
                ServerName = Site["ServerName"].StringValue,
                SiteCode = Site["SiteCode"].StringValue,
                SiteName = Site["SiteName"].StringValue,
                Status = (SMS_Site.SiteStatus)Site["Status"].IntegerValue,
                TimeZoneInfo = Site["TimeZoneInfo"].StringValue,
                Type = (SMS_Site.SiteType)Site["Type"].IntegerValue,
                Version = Site["Version"].StringValue,
            };
        }

        public static SMS_Collection CollectionFromIresultObject(IResultObject Collection)
        {

           // Collection.Get();
            Collection = CompleteIresultOject(Collection);

            return new SMS_Collection()
            {

                CollectionID = Collection["CollectionID"].StringValue,
                CollectionRules = CollectionRulesFromIresultObject((ManagementBaseObject[])Collection["CollectionRules"].ObjectArrayValue),
                CollectionType = Collection["CollectionType"].IntegerValue,
                CollectionVariablesCount = Collection["CollectionVariablesCount"].IntegerValue,
                Comment = Collection["Comment"].StringValue,
                CurrentStatus = Collection["CurrentStatus"].IntegerValue,
                HasProvisionedMember = Collection["HasProvisionedMember"].BooleanValue,
                IncludeExcludeCollectionsCount = Collection["IncludeExcludeCollectionsCount"].IntegerValue,
                IsBuiltIn = Collection["IsBuiltIn"].BooleanValue,
                IsReferenceCollection = Collection["IsReferenceCollection"].BooleanValue,
                ISVData = Collection["ISVData"].ByteArrayValue,
                ISVDataSize = Collection["ISVDataSize"].IntegerValue,
                LastChangeTime = Collection["LastChangeTime"].DateTimeValue,
                LastMemberChangeTime = Collection["LastMemberChangeTime"].DateTimeValue,
                LastRefreshTime = Collection["LastRefreshTime"].DateTimeValue,
                LimitToCollectionID = Collection["LimitToCollectionID"].StringValue,
                LimitToCollectionName = Collection["LimitToCollectionName"].StringValue,
                LocalMemberCount = Collection["LocalMemberCount"].IntegerValue,
                MemberClassName = Collection["MemberClassName"].StringValue,
                MemberCount = Collection["MemberCount"].IntegerValue,
                MonitoringFlags = Collection["MonitoringFlags"].IntegerValue,
                Name = Collection["Name"].StringValue,
                OwnedByThisSite = Collection["OwnedByThisSite"].BooleanValue,
                PowerConfigsCount = Collection["PowerConfigsCount"].IntegerValue,
                RefreshSchedule = ScheduleTokFromIresultObject((ManagementBaseObject[])Collection["RefreshSchedule"].ObjectArrayValue),
                RefreshType = Collection["RefreshType"].IntegerValue,
                ReplicateToSubSites = Collection["ReplicateToSubSites"].BooleanValue,
                ServiceWindowsCount = Collection["ServiceWindowsCount"].IntegerValue,

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="System"></param>
        /// <returns></returns>
        public static SMS_R_System SystemFromIresultObject(IResultObject System)
        {

            System.Get();
            System = CompleteIresultOject(System);

            return new SMS_R_System()
            {
                ADSiteName = System["ADSiteName"].StringValue,
                AgentName = System["AgentName"].StringArrayValue,
                AgentSite = System["AgentSite"].StringArrayValue,
                AMTFullVersion = System["AMTFullVersion"].StringValue,
                ClientVersion = System["ClientVersion"].StringValue,
                CPUType = System["CPUType"].StringValue,
                DistinguishedName = System["DistinguishedName"].StringValue,
                EASDeviceID = System["EASDeviceID"].StringValue,
                HardwareID = System["HardwareID"].StringValue,
                IPAddresses = System["IPAddresses"].StringArrayValue,
                IPSubnets = System["IPSubnets"].StringArrayValue,
                IPv6Addresses = System["IPv6Addresses"].StringArrayValue,
                IPv6Prefixes = System["IPv6Prefixes"].StringArrayValue,
                LastLogonUserDomain = System["LastLogonUserDomain"].StringValue,
                LastLogonUserName = System["LastLogonUserName"].StringValue,
                MACAddresses = System["MACAddresses"].StringArrayValue,
                Name = System["Name"].StringValue,
                NetbiosName = System["NetbiosName"].StringValue,
                OperatingSystemNameandVersion = System["OperatingSystemNameandVersion"].StringValue,
                PreviousSMSUUID = System["PreviousSMSUUID"].StringValue,
                ResourceDomainORWorkgroup = System["ResourceDomainORWorkgroup"].StringValue,
                ResourceNames = System["ResourceNames"].StringArrayValue,
                SecurityGroupName = System["SecurityGroupName"].StringArrayValue,
                SID = System["SID"].StringValue,
                SMBIOSGUID = System["SMBIOSGUID"].StringValue,
                SMSAssignedSites = System["SMSAssignedSites"].StringArrayValue,
                SMSInstalledSites = System["SMSInstalledSites"].StringArrayValue,
                SMSResidentSites = System["SMSResidentSites"].StringArrayValue,
                SMSUniqueIdentifier = System["SMSUniqueIdentifier"].StringValue,
                SNMPCommunityName = System["SNMPCommunityName"].StringValue,
                SystemContainerName = System["SystemContainerName"].StringArrayValue,
                SystemGroupName = System["SystemGroupName"].StringArrayValue,
                SystemOUName = System["SystemOUName"].StringArrayValue,
                SystemRoles = System["SystemRoles"].StringArrayValue,
                VirtualMachineHostName = System["VirtualMachineHostName"].StringValue,
                Active = System["Active"].IntegerValue,
                AgentTime = System["AgentTime"].DateTimeArrayValue,
                AlwaysInternet = System["AlwaysInternet"].IntegerValue,
                Client = System["Client"].IntegerValue,
                ClientType = System["ClientType"].IntegerValue,
                CreationDate = System["CreationDate"].DateTimeValue,
                Decommissioned = System["Decommissioned"].IntegerValue,
                SMSUUIDChangeDate = System["SMSUUIDChangeDate"].DateTimeValue,
                LastLogonTimestamp = System["LastLogonTimestamp"].DateTimeValue,
                InternetEnabled = System["InternetEnabled"].IntegerValue,
                IsAssignedToUser = System["IsAssignedToUser"].BooleanValue,
                IsVirtualMachine = System["IsVirtualMachine"].BooleanValue,
                Obsolete = System["Obsolete"].IntegerValue,
                PrimaryGroupID = System["PrimaryGroupID"].IntegerValue,
                ResourceID = System["ResourceID"].IntegerValue,
                ResourceType = System["ResourceType"].IntegerValue,
                UserAccountControl = System["UserAccountControl"].IntegerValue,
                IsClientAMT30Compatible = System["IsClientAMT30Compatible"].IntegerValue,
                IsMachineChangesPersisted = System["IsMachineChangesPersisted"].BooleanValue,
                ObjectGUID = System["ObjectGUID"].ByteArrayValue,
                SuppressAutoProvision = System["SuppressAutoProvision"].IntegerValue,
                Unknown = System["Unknown"].IntegerValue,
                AMTStatus = System["AMTStatus"].IntegerValue,
                WipeStatus = System["WipeStatus"].IntegerValue,
            };


        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Application"></param>
        /// <returns></returns>
        public static SMS_Application ApplicationFromIresultObject(IResultObject Application)
        {
            Application.Get();
            Application = CompleteIresultOject(Application);

            return new SMS_Application()
            {
                ApplicabilityCondition = Application["ApplicabilityCondition"].StringValue,
                CategoryInstance_UniqueIDs = Application["CategoryInstance_UniqueIDs"].StringArrayValue,
                CI_ID = Application["CI_ID"].IntegerValue,
                CI_UniqueID = Application["CI_UniqueID"].StringValue,
                CIType_ID = Application["CIType_ID"].IntegerValue,
                CIVersion = Application["CIVersion"].IntegerValue,
                CreatedBy = Application["CreatedBy"].StringValue,
                DateCreated = Application["DateCreated"].DateTimeValue,
                DateLastModified = Application["DateLastModified"].DateTimeValue,
                EffectiveDate = Application["EffectiveDate"].DateTimeValue,
                EULAAccepted = Application["EULAAccepted"].IntegerValue,
                EULAExists = Application["EULAExists"].BooleanValue,
                EULASignoffDate = Application["EULASignoffDate"].DateTimeValue,
                EULASignoffUser = Application["EULASignoffUser"].StringValue,
                ExecutionContext = Application["ExecutionContext"].IntegerValue,
                Featured = Application["Featured"].IntegerValue,
                HasContent = Application["HasContent"].BooleanValue,
                IsBundle = Application["IsBundle"].BooleanValue,
                IsDeployable = Application["IsDeployable"].BooleanValue,
                IsDeployed = Application["IsDeployed"].BooleanValue,
                IsDigest = Application["IsDigest"].BooleanValue,
                IsEnabled = Application["IsEnabled"].BooleanValue,
                IsExpired = Application["IsExpired"].BooleanValue,
                IsHidden = Application["IsHidden"].BooleanValue,
                IsLatest = Application["IsLatest"].BooleanValue,
                IsQuarantined = Application["IsQuarantined"].BooleanValue,
                IsSuperseded = Application["IsSuperseded"].BooleanValue,
                IsSuperseding = Application["IsSuperseding"].BooleanValue,
                IsUserDefined = Application["IsUserDefined"].BooleanValue,
                LastModifiedBy = Application["LastModifiedBy"].StringValue,
                LocalizedCategoryInstanceNames = Application["LocalizedCategoryInstanceNames"].StringArrayValue,
                LocalizedDescription = Application["LocalizedDescription"].StringValue,
                LocalizedDisplayName = Application["LocalizedDisplayName"].StringValue,
                LocalizedInformativeURL = Application["LocalizedInformativeURL"].StringValue,
                LocalizedPropertyLocaleID = Application["LocalizedPropertyLocaleID"].IntegerValue,
                LogonRequirement = Application["LogonRequirement"].IntegerValue,
                Manufacturer = Application["Manufacturer"].StringValue,
                ModelName = Application["ModelName"].StringValue,
                ModelID = Application["ModelID"].IntegerValue,
                NumberOfDependentDTs = Application["NumberOfDependentDTs"].IntegerValue,
                NumberOfDependentTS = Application["NumberOfDependentTS"].IntegerValue,
                NumberOfDeployments = Application["NumberOfDeployments"].IntegerValue,
                NumberOfDeploymentTypes = Application["NumberOfDeploymentTypes"].IntegerValue,
                NumberOfDevicesWithApp = Application["NumberOfDevicesWithApp"].IntegerValue,
                NumberOfDevicesWithFailure = Application["NumberOfDevicesWithFailure"].IntegerValue,
                NumberOfUsersWithApp = Application["NumberOfUsersWithApp"].IntegerValue,
                NumberOfUsersWithFailure = Application["NumberOfUsersWithFailure"].IntegerValue,
                NumberOfUsersWithRequest = Application["NumberOfUsersWithRequest"].IntegerValue,
                NumberOfVirtualEnvironments = Application["NumberOfVirtualEnvironments"].IntegerValue,
                PackageID = Application["PackageID"].StringValue,
                PermittedUses = Application["PermittedUses"].IntegerValue,
                PlatformCategoryInstance_UniqueIDs = Application["PlatformCategoryInstance_UniqueIDs"].StringArrayValue,
                PlatformType = Application["PlatformType"].IntegerValue,
                SDMPackageLocalizedData = SSDMPackageFromIresultObject((ManagementBaseObject[])Application["SDMPackageLocalizedData"].ObjectArrayValue),
                SDMPackageVersion = Application["SDMPackageVersion"].IntegerValue,
                SDMPackageXML = Application["SDMPackageXML"].StringValue,
                SecuredScopeNames = Application["SecuredScopeNames"].StringArrayValue,
                SedoObjectVersion = Application["SedoObjectVersion"].StringValue,
                SoftwareVersion = Application["SoftwareVersion"].StringValue,
                SourceCIVersion = Application["SourceCIVersion"].IntegerValue,
                SourceModelName = Application["SourceModelName"].StringValue,
                SourceSite = Application["SourceSite"].StringValue,
                SummarizationTime = Application["SummarizationTime"].DateTimeValue
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Application"></param>
        /// <returns></returns>
        public static SMS_TaskSequencePackage TaskSequencePackageFromIresultObject(IResultObject Application)
        {
            Application.Get();
            Application = CompleteIresultOject(Application);

            return new SMS_TaskSequencePackage()
            {
                ActionInProgress = Application["ActionInProgress"].IntegerValue,
                AlternateContentProviders = Application["AlternateContentProviders"].StringValue,
                BootImageID = Application["BootImageID"].StringValue,
                Category = Application["Category"].StringValue,
                CustomProgressMsg = Application["CustomProgressMsg"].StringValue,
                DependentProgram = Application["DependentProgram"].StringValue,
                Description = Application["Description"].StringValue,
                Duration = Application["Duration"].IntegerValue,
                ExtendedData = Application["ExtendedData"].ByteArrayValue,
                ExtendedDataSize = Application["ExtendedDataSize"].IntegerValue,
                ForcedDisconnectDelay = Application["ForcedDisconnectDelay"].IntegerValue,
                ForcedDisconnectEnabled = Application["ForcedDisconnectEnabled"].BooleanValue,
                ForcedDisconnectNumRetries = Application["ForcedDisconnectNumRetries"].IntegerValue,
                Icon = Application["Icon"].ByteArrayValue,
                IconSize = Application["IconSize"].IntegerValue,
                IgnoreAddressSchedule = Application["IgnoreAddressSchedule"].BooleanValue,
                ISVData = Application["ISVData"].ByteArrayValue,
                ISVDataSize = Application["ISVDataSize"].IntegerValue,
                Language = Application["Language"].StringValue,
                LastRefreshTime = Application["LastRefreshTime"].DateTimeValue,
                LocalizedCategoryInstanceNames = Application["LocalizedCategoryInstanceNames"].StringArrayValue,
                Manufacturer = Application["Manufacturer"].StringValue,
                MIFFilename = Application["MIFFilename"].StringValue,
                MIFName = Application["MIFName"].StringValue,
                MIFPublisher = Application["MIFPublisher"].StringValue,
                MIFVersion = Application["MIFVersion"].StringValue,
                Name = Application["Name"].StringValue,
                NumOfPrograms = Application["NumOfPrograms"].IntegerValue,
                PackageID = Application["PackageID"].StringValue,
                PackageSize = Application["PackageSize"].IntegerValue,
                PackageType = Application["PackageType"].IntegerValue,
                PkgFlags = Application["PkgFlags"].IntegerValue,
                PkgSourceFlag = Application["PkgSourceFlag"].IntegerValue,
                PkgSourcePath = Application["PkgSourcePath"].StringValue,
                PreferredAddressType = Application["PreferredAddressType"].StringValue,
                Priority = Application["Priority"].IntegerValue,
                ProgramFlags = Application["ProgramFlags"].IntegerValue,
                References = TSRefFromIresultObject((ManagementBaseObject[])Application["References"].ObjectArrayValue),
                RefreshPkgSourceFlag = Application["RefreshPkgSourceFlag"].BooleanValue,
                RefreshSchedule = ScheduleTokFromIresultObject((ManagementBaseObject[])Application["RefreshSchedule"].ObjectArrayValue),
                SecuredScopeNames = Application["SecuredScopeNames"].StringArrayValue,
                SedoObjectVersion = Application["SedoObjectVersion"].StringValue,
                ReferencesCount = Application["ReferencesCount"].IntegerValue,
                Reserved = Application["Reserved"].StringValue,
                Sequence = Application["Sequence"].StringValue,
                ShareName = Application["ShareName"].StringValue,
                ShareType = Application["ShareType"].IntegerValue,
                SourceDate = Application["SourceDate"].DateTimeValue,
                SourceSite = Application["SourceSite"].StringValue,
                SourceVersion = Application["SourceVersion"].IntegerValue,
                StoredPkgPath = Application["StoredPkgPath"].StringValue,
                StoredPkgVersion = Application["StoredPkgVersion"].IntegerValue,
                SupportedOperatingSystems = OSDetailsPackageFromIresultObject((ManagementBaseObject[])Application["SupportedOperatingSystems"].ObjectArrayValue),
                TaskSequenceFlags = Application["TaskSequenceFlags"].IntegerValue,
                Type = Application["Type"].IntegerValue,
                Version = Application["Version"].StringValue
            };

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Application"></param>
        /// <returns></returns>
        public static SMS_ScheduleToken[] ScheduleTokFromIresultObject(ManagementBaseObject[] Application)
        {
            if (Application == null)
            {
                return new SMS_ScheduleToken[0];
            }


            SMS_ScheduleToken[] SMS_ScheduleTokenList = new SMS_ScheduleToken[Application.Count()];
            int i = 0;
            foreach (ManagementBaseObject Element in Application)
            {
                // IResultObject AppElement = CompleteIresultOject(Element);

                SMS_ScheduleTokenList[i++] = new SMS_ScheduleToken()
                {
                    DayDuration = Convert.ToInt32(Element["DayDuration"]),
                    HourDuration = Convert.ToInt32(Element["HourDuration"]),
                    IsGMT = Convert.ToBoolean(Element["IsGMT"]),
                    MinuteDuration = Convert.ToInt32(Element["MinuteDuration"]),
                    StartTime = Convert.ToDateTime(Element["StartTime"]),
                };
            }
            return SMS_ScheduleTokenList;
        }

        public static SMS_CollectionRule[] CollectionRulesFromIresultObject(ManagementBaseObject[] Collection)
        {
            if (Collection == null)
            {
                return new SMS_CollectionRule[0];
            }


            SMS_CollectionRule[] SMS_CollectionRuleList = new SMS_CollectionRule[Collection.Count()];
            int i = 0;
            foreach (ManagementBaseObject Element in Collection)
            {

                SMS_CollectionRuleList[i++] = new SMS_CollectionRule()
                {
                    RuleName = Element["RuleName"].ToString(),
                };
            }
            return SMS_CollectionRuleList;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Application"></param>
        /// <returns></returns>
        public static SMS_SDMPackageLocalizedData[] SSDMPackageFromIresultObject(ManagementBaseObject[] Application)
        {
            //  Application = CompleteIresultOject(Application);

            if (Application == null)
            {
                return new SMS_SDMPackageLocalizedData[0];
            }

            SMS_SDMPackageLocalizedData[] SMS_SDMPackageLocalizedDataList = new SMS_SDMPackageLocalizedData[Application.Count()];
            int i = 0;
            foreach (ManagementBaseObject Element in Application)
            {
                SMS_SDMPackageLocalizedDataList[i++] = new SMS_SDMPackageLocalizedData()
                {
                    LocaleID = Convert.ToInt32(Element["LocaleID"]),
                    LocalizedData = Element["LocalizedData"].ToString(),
                };
            }
            return SMS_SDMPackageLocalizedDataList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Application"></param>
        /// <returns></returns>
        public static SMS_APermission[] APermissionFromIresultObject(ManagementBaseObject[] Application)
        {
            //  Application = CompleteIresultOject(Application);

            if (Application == null)
            {
                return new SMS_APermission[0];
            }

            SMS_APermission[] SMS_APermissionList = new SMS_APermission[Application.Count()];
            int i = 0;
            foreach (ManagementBaseObject Element in Application)
            {
                SMS_APermissionList[i++] = new SMS_APermission()
                {
                    CategoryID = Element["CategoryID"].ToString(),
                    CategoryName = Element["CategoryName"].ToString(),
                    CategoryTypeID = Convert.ToInt32(Element["CategoryTypeID"]),
                    RoleID = Element["RoleID"].ToString(),
                    RoleName = Element["RoleName"].ToString(),
                };
            }
            return SMS_APermissionList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Application"></param>
        /// <returns></returns>
        public static SMS_TaskSequence_Reference[] TSRefFromIresultObject(ManagementBaseObject[] Application)
        {

            if (Application == null)
            {
                return new SMS_TaskSequence_Reference[0];
            }

            SMS_TaskSequence_Reference[] SMS_TaskSequence_ReferenceList = new SMS_TaskSequence_Reference[Application.Count()];

            int i = 0;
            foreach (ManagementBaseObject Element in Application)
            {
                //  IResultObject AppElement = CompleteIresultOject(Element);

                SMS_TaskSequence_ReferenceList[i++] = new SMS_TaskSequence_Reference()
                {
                    Package = Element["Package"].ToString(),
                    Program = Element["Program"].ToString(),
                    Type = Convert.ToInt32(Element["Type"]),
                };
            }

            return SMS_TaskSequence_ReferenceList;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Application"></param>
        /// <returns></returns>
        public static SMS_OS_Details[] OSDetailsPackageFromIresultObject(ManagementBaseObject[] Application)
        {

            if (Application == null)
            {
                return new SMS_OS_Details[0];
            }

            SMS_OS_Details[] SMS_OS_DetailsList = new SMS_OS_Details[Application.Count()];
            int i = 0;
            foreach (ManagementBaseObject Element in Application)
            {
                //  IResultObject AppElement = CompleteIresultOject(Element);

                SMS_OS_DetailsList[i++] = new SMS_OS_Details()
                {
                    Name = Element["Name"].ToString(),
                    Platform = Element["Platform"].ToString(),
                    MinVersion = Element["MinVersion"].ToString(),
                    MaxVersion = Element["MaxVersion"].ToString(),
                };
            }

            return SMS_OS_DetailsList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Pkgresults"></param>
        /// <returns></returns>
        public static SMS_Package PackageFromIresultObject(IResultObject Pkgresults)
        {
            Pkgresults = CompleteIresultOject(Pkgresults);

            return new SMS_Package()
            {
                PackageID = Pkgresults["PackageID"].StringValue,
                LastRefreshTime = Pkgresults["LastRefreshTime"].DateTimeValue,
                Name = Pkgresults["Name"].StringValue,
                MIFPublisher = Pkgresults["MIFPublisher"].StringValue,
                MIFVersion = Pkgresults["MIFVersion"].StringValue,
                NumOfPrograms = Pkgresults["NumOfPrograms"].IntegerValue,
                PackageSize = Pkgresults["PackageSize"].IntegerValue,
                PackageType = Pkgresults["PackageType"].IntegerValue,
                PkgFlags = Pkgresults["PkgFlags"].IntegerValue,
                PkgSourceFlag = Pkgresults["PkgSourceFlag"].IntegerValue,
                PkgSourcePath = Pkgresults["PkgSourcePath"].StringValue,
                PreferredAddressType = Pkgresults["PreferredAddressType"].StringValue,
                Priority = Pkgresults["Priority"].IntegerValue,
                RefreshPkgSourceFlag = Pkgresults["RefreshPkgSourceFlag"].BooleanValue,
                SecuredScopeNames = Pkgresults["SecuredScopeNames"].StringArrayValue,
                ShareName = Pkgresults["ShareName"].StringValue,
                ShareType = Pkgresults["ShareType"].IntegerValue,
                SourceDate = Pkgresults["SourceDate"].DateTimeValue,
                SourceSite = Pkgresults["SourceSite"].StringValue,
                SourceVersion = Pkgresults["SourceVersion"].IntegerValue,
                StoredPkgPath = Pkgresults["StoredPkgPath"].StringValue,
                StoredPkgVersion = Pkgresults["StoredPkgVersion"].IntegerValue
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="System"></param>
        /// <returns></returns>
        public static IResultObject CompleteIresultOject(IResultObject System)
        {


            foreach (string Properties in System.PropertyNames)
            {
                if (System[Properties].ObjectValue == null)
                {
                    switch (System[Properties].DataType)
                    {
                        case ManagementClassPropertyDescription.TypeOfData.Boolean:
                            System[Properties].ObjectValue = false;
                            break;

                        case ManagementClassPropertyDescription.TypeOfData.String:
                            if (System[Properties].IsArray)
                            {

                                switch (Properties)
                                {
                                    case "SDMPackageLocalizedData":
                                        //  SMS_SDMPackageLocalizedData[] CompletionData = new SMS_SDMPackageLocalizedData[1];
                                        //  CompletionData[0] = new SMS_SDMPackageLocalizedData();
                                        //  System[Properties].ObjectArrayValue = CompletionData;
                                        break;

                                    default:
                                        System[Properties].ObjectValue = new string[0];
                                        break;
                                }

                            }
                            else { System[Properties].ObjectValue = System[Properties].ObjectValue.IfNullThenNull(); }
                            break;

                        case ManagementClassPropertyDescription.TypeOfData.Integer:
                            if (System[Properties].IsArray)
                            { System[Properties].ObjectValue = new int[0]; }
                            else { System[Properties].ObjectValue = System[Properties].ObjectValue.IfNullThenZero(); }
                            break;

                        case ManagementClassPropertyDescription.TypeOfData.InvariantInteger:
                            if (System[Properties].IsArray)
                            { System[Properties].ObjectValue = new int[0]; }
                            else { System[Properties].ObjectValue = System[Properties].ObjectValue.IfNullThenZero(); }
                            break;

                        case ManagementClassPropertyDescription.TypeOfData.InvariantLong:
                            if (System[Properties].IsArray)
                            { System[Properties].ObjectValue = new int[0]; }
                            else { System[Properties].ObjectValue = System[Properties].ObjectValue.IfNullThenZero(); }
                            break;

                        case ManagementClassPropertyDescription.TypeOfData.DateTime:
                            System[Properties].ObjectValue = System[Properties].ObjectValue.IfNullThenDateTime();
                            break;


                    }

                }
            }

            return System;

        }



    }
}