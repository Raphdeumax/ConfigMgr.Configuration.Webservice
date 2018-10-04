# ConfigMgr.Configuration.Webservice
The ConfigMgr Configuration WebService has been designed to Provides access to the main functions used in System Center Configuration Manager (SCCM Service Web).  It contains methods for performing operations in Configuration Manager and Active Directory.

# Configuration Manager (methods available) :

- AddClientToCollection :
Add a Client in a collection 

- AddComputerAssociationForMigration :
Add a computer association between a computer and older computer for a single user 

- AddDPGroups :
Add a distribution point to a Distribution Group 

AddPackageOnDistributionPoint :
Add a package on a Distribution Point 

AddSecondaryToCollection :
Add a distribution Group to a Collection 

AddSenderAddress :
Add a sender Address in order to communicate with a new site recently created 

AddSiteBoundaries :
Add Site Boundary 

AddUnknownClientToCollection :
Add an unknown Client from a collection 

AssignApplicationToCollection :
Deploy an application on a collection of devices or Users 

CopyPackageOnDP :
Copy all packages on the Dp from a Reference Distribution Point 

CreateApplication :
Add an unknown Client from a collection 

GetADSiteFromGateway :
Get ADSite by Gateway 

GetADSiteFromIP :
Get ADSite by IP Address 

GetAllBoundaries :
Get all boundaries available in SCCM 

GetAllClient :
Retrieve all clients informations available in SCCM 

GetAllDPGroups :
Get all DP Groups available in SCCM 

GetAllUserAdmin :
Retrieve all clients informations available in SCCM 

GetApplicationsListInfos :
Get all applications assign to a Site 

GetClientByName :
Retrieve Clients associate to the UserName (Without domain) 

GetClientBySubnet :
Retrieve Clients associate to a subnet (format = > 192.168.1.0) 

GetClientByUUID :
Retrieve client informations available in SCCM by BIOS UUID identifier 

GetClientByUserName :
Retrieve Clients associate to the UserName (Without domain) 

GetClientDetailsLDAP :
Get Computer informations from the active directory 

GetClientExist :
Client Exist ? 1=Yes | 0=No 

GetClientOSByResourceID :
Retrieve client Operating System informations available in SCCM by ResourceID 

GetCollectionID :
Get all Collection ID associate to a client 

GetCriticalSiteComponent :
Get All Critical status for a component, example : SMS_MP 

GetDirectCollectionID :
Get all direct collection assignement to a Client 

GetLocationSite :
Get client Location by IP Address 

GetPackagesListInfos :
Get all Packages | Boot Images | Windows Updates assign to a Site 

GetParentSiteInfos :
Get a parent of a SCCM server 

GetServerExist :
SCCM Server Exist => 1=Yes | 0=No 

GetServerInfos :
Get the SCCM Server informations 

GetSiteCodeInfos :
Get the Site code of a SCCM Server 

GetTaskSequencesListInfos :
Get all task sequences assign to a Site 

GetUnknowClientByUUID :
Check for 'Unknown' Client record by UUID (SMBIOS GUID) 

IsInAllSystemsCollection :
Check if client exist in the collection AllSystems 

IsPackageExist :
Check if a package exist in SCCM 

RefreshAllPackages :
Refresh all packages assign to a distribution point 

RefreshPackage :
Refresh a package assign to a distribution point 

RemoveClientByName :
Remove client in SCCM by Name 

RemoveClientByUUID :
Remove client in SCCM by BIOS UUID identifier 

RemoveClientFromCollection :
Remove a Client from a collection 

RemoveClientLDAP :
Remove a computer from the active directory 

RemoveDPGroups :
Remove a distribution point from a Distribution Group 

RemovePackageFromDistributionPoint :
Remove a Package from a distribution point 

RemoveSenderAddress :
Remove a Sender Address 

RemoveServerFromSCCM :
Remove a server from SCCL 

RemoveSiteBoundaries :
Remove all site boundaries associte to a Server site 

RemoveUnknowClientByUUID :
Check for 'Unknown' Client record by UUID (SMBIOS GUID) 

RemoveUnknownClientFromCollection :
Remove an unknown Client from a collection 

SyncKeySecondary :
Send the Secondary Key to The Primary server and conversely in order to synchronize both sites 

UpdateSiteBoundaries :
Update site boundaries

# Supported Configurations
This web service has been built to support the following versions of System Center Configuration Manager:

Configuration Manager 2012 SP1
Configuration Manager 2012 SP2
Configuration Manager 2012 R2
Configuration Manager 2012 R2 SP1
Configuration Manager Current Branch (all currently supported versions released by Microsoft)
Make sure that .NET Framework 4.5.1 is available on the member server you intend to host this web service on.

# Installation instructions

To successfully run this web service, you'll need to have IIS installed on a member server with ASP.NET enabled. Easiest way to get going is to install the ConfigMgrWebService on the same server as where your Management Point role is hosted. You'll also need to have a service account for the application pool in IIS. It's recommended that you add the service account in ConfigMgr with Full Administrator privileges.

# 1 - Create folder structure
Download the project and compile the solution in Visual Studio (you can download the free version called Visual Studio Community Edition)
Create a folder in C:\inetpub called ConfigMgr.Configuration.Webservice. Inside that folder, create a folder called bin.
Copy the compiled ConfigMgr.Configuration.Webservice.dll to C:\inetpub\ConfigMgr.Configuration.Webservice\bin.
Rename Web.Release.config to Web.config and copy it to C:\inetpub\ConfigMgr.Configuration.Webservice.
Copy ConfigMgrWebService.asmx to C:\inetpub\ConfigMgr.Configuration.Webservice.
Locate all .dll files in lib folder and copy them to C:\inetpub\ConfigMgr.Configuration.Webservice\bin.

# 2 - Add an Application Pool in IIS
Open IIS management console, right click on Application Pools and select Add Application Pool.
Enter ConfigMgr.Configuration.Webservice as name, select the .NET CLR version .NET CLR Version v4.0.30319 and click OK.
Select the new ConfigMgr.Configuration.Webservice application pool and select Advanced Settings.
In the Process Model section, specify the service account that will have access to ConfigMgr in the Identity field (account with full administrators right in the SCCM console) and click OK.

# 3 - Add an Application to Default Web Site
Open IIS management console, expand Sites, right click on Default Web Site and select Add Application.
As for Alias, enter ConfigMgr.Configuration.Webservice.
Select ConfigMgr.Configuration.Webservice as application pool.
Set the physical path to C:\inetpub\ConfigMgr.Configuration.Webservice and click OK.
# 4 - Set Application Settings
Open IIS management console, expand Sites and Default Web Site.
Select ConfigMgr.Configuration.Webservice application and go to Authentication and click "Edit" on them, Select Application Pool identity and click OK.

![alt text](https://github.com/Raphdeumax/ConfigMgr.Configuration.Webservice/blob/master/capture.png)
