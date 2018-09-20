//*******************************************************************************//
//  Developpement :  Raphael DELPLANQUE                                          //
//*******************************************************************************//

using System;
using System.Threading;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web.Services;
using ConfigMgr.Configuration.Webservice.utils;
using ConfigMgr.Configuration.Webservice.utils.SMS;
using ConfigMgr.Configuration.Webservice.utils.LDAP;
using Microsoft.Win32;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;

namespace ConfigMgr.Configuration.Webservice
{
    //****************************************************************************//
    //  Cette classe definit les methodes relatives au Webservice ConfigMgr       //
    //  permettant de disposer des informations d'installation des serveurs de    //
    //  l infrastructure SCCM.                                                    //
    //                                                                            //
    //  Ce webservice met a disposition les methodes suivantes :                  
    // AddClientToCollection 
    // Add a Client in a collection 
    // 
    // AddComputerAssociationForMigration 
    // Add a computer association between a computer and older computer for a single user 
    // 
    // AddDPGroups 
    // Add a distribution point to a Distribution Group 
    // 
    // AddPackageOnDistributionPoint 
    // Add a package on a Distribution Point 
    // 
    // AddSecondaryToCollection 
    // Add a distribution Group to a Collection 
    // 
    // AddSenderAddress 
    // Add a sender Address in order to communicate with a new site recently created 
    // 
    // AddSiteBoundaries 
    // Add Site Boundary 
    // 
    // AddUnknownClientToCollection 
    // Add an unknown Client from a collection 
    // 
    // AssignApplicationToCollection 
    // Deploy an application on a collection of devices or Users 
    // 
    // CopyPackageOnDP 
    // Copy all packages on the Dp from a Reference Distribution Point 
    // 
    // CreateApplication 
    // Add an unknown Client from a collection 
    // 
    // GetADSiteFromGateway 
    // Get ADSite by Gateway 
    // 
    // GetADSiteFromIP 
    // Get ADSite by IP Address 
    // 
    // GetAllBoundaries 
    // Get all boundaries available in SCCM 
    // 
    // GetAllClient 
    // Retrieve all clients informations available in SCCM 
    // 
    // GetAllDPGroups 
    // Get all DP Groups available in SCCM 
    // 
    // GetAllUserAdmin 
    // Retrieve all clients informations available in SCCM 
    // 
    // GetApplicationsListInfos 
    // Get all applications assign to a Site 
    // 
    // GetClientByName 
    // Retrieve Clients associate to the UserName (Without domain) 
    // 
    // GetClientBySubnet 
    // Retrieve Clients associate to a subnet (format = > 192.168.1.0) 
    // 
    // GetClientByUUID 
    // Retrieve client informations available in SCCM by BIOS UUID identifier 
    // 
    // GetClientByUserName 
    // Retrieve Clients associate to the UserName (Without domain) 
    // 
    // GetClientDetailsLDAP 
    // Get Computer informations from the active directory 
    // 
    // GetClientExist 
    // Client Exist ? 1=Yes | 0=No 
    // 
    // GetClientOSByResourceID 
    // Retrieve client Operating System informations available in SCCM by ResourceID 
    // 
    // GetCollectionID 
    // Get all Collection ID associate to a client 
    // 
    // GetCriticalSiteComponent 
    // Get All Critical status for a component, example : SMS_MP 
    // 
    // GetDirectCollectionID 
    // Get all direct collection assignement to a Client 
    // 
    // GetLocationSite 
    // Get client Location by IP Address 
    // 
    // GetPackagesListInfos 
    // Get all Packages | Boot Images | Windows Updates assign to a Site 
    // 
    // GetParentSiteInfos 
    // Get a parent of a SCCM server 
    // 
    // GetServerExist 
    // SCCM Server Exist => 1=Yes | 0=No 
    // 
    // GetServerInfos 
    // Get the SCCM Server informations 
    // 
    // GetSiteCodeInfos 
    // Get the Site code of a SCCM Server 
    // 
    // GetTaskSequencesListInfos 
    // Get all task sequences assign to a Site 
    // 
    // GetUnknowClientByUUID 
    // Check for 'Unknown' Client record by UUID (SMBIOS GUID) 
    // 
    // IsInAllSystemsCollection 
    // Check if client exist in the collection AllSystems 
    // 
    // IsPackageExist 
    // Check if a package exist in SCCM 
    // 
    // RefreshAllPackages 
    // Refresh all packages assign to a distribution point 
    // 
    // RefreshPackage 
    // Refresh a package assign to a distribution point 
    // 
    // RemoveClientByName 
    // Remove client in SCCM by Name 
    // 
    // RemoveClientByUUID 
    // Remove client in SCCM by BIOS UUID identifier 
    // 
    // RemoveClientFromCollection 
    // Remove a Client from a collection 
    // 
    // RemoveClientLDAP 
    // Remove a computer from the active directory 
    // 
    // RemoveDPGroups 
    // Remove a distribution point from a Distribution Group 
    // 
    // RemovePackageFromDistributionPoint 
    // Remove a Package from a distribution point 
    // 
    // RemoveSenderAddress 
    // Remove a Sender Address 
    // 
    // RemoveServerFromSCCM 
    // Remove a server from SCCL 
    // 
    // RemoveSiteBoundaries 
    // Remove all site boundaries associte to a Server site 
    // 
    // RemoveUnknowClientByUUID 
    // Check for 'Unknown' Client record by UUID (SMBIOS GUID) 
    // 
    // RemoveUnknownClientFromCollection 
    // Remove an unknown Client from a collection 
    // 
    // SyncKeySecondary 
    // Send the Secondary Key to The Primary server and conversely in order to synchronize both sites 
    // 
    // UpdateSiteBoundaries 
    // Update site boudaries 
    //****************************************************************************//

    /// <summary>
    /// 
    /// </summary>
    /// 
    [WebService(Namespace = "ConfigMgr.ai3.infraweb.fr")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class ConfigMgr : WebService
    {
        //************************************************************************//
        // Initialisation des variables de classe                                 //
        //************************************************************************//
        private string _className = "ConfigMgr";

        /// <summary>
        /// 
        /// </summary>
        public Logger _log = new Logger();

        /// <summary>
        /// 
        /// </summary>
        public DataAccess _da = new DataAccess();

        /// <summary>
        /// 
        /// </summary>
        public LDAPAccess _ldap = new LDAPAccess();

        //************************************************************************//
        //  Constructeur de SccmAccess                                            //
        //************************************************************************//
        /// <summary>
        /// Constructeur de SccmAccess
        /// </summary>
        public SccmAccess _sa = new SccmAccess();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="IPStation"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get client Location by IP Address")]
        public List<SMS_Boundary> GetLocationSite(string IPStation)
        {
            List<SMS_Boundary> Boundaries = new List<SMS_Boundary>();
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, IPStation, "Starting the call " + methodName);
                if (!string.IsNullOrEmpty(IPStation))
                {
                    Boundaries = _sa.GetSiteCodeByIPBound(IPStation, ConnectionManager);
                    //if (Boundaries.Count > 1)
                    //{
                    //    throw new Exception("Erreur Overlapping : L'Ip " + IPStation + " est associee à plusieurs sites");
                    //}

                }

                return Boundaries;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "Impossible de recuperer les informations de localisation");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                //  return "1;" + ex.Message.ToString();
                return new List<SMS_Boundary>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, IPStation, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="IPStation"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get ADSite by IP Address")]
        public List<SMS_ADSite> GetADSiteFromIP(string IPStation)
        {
            List<SMS_ADSite> SMS_ADSites = new List<SMS_ADSite>();
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, IPStation, "Starting the call " + methodName);
                if (!string.IsNullOrEmpty(IPStation))
                {
                    SMS_ADSites = _sa.GetADSiteByIPBound(IPStation, ConnectionManager);

                }

                return SMS_ADSites;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "Impossible de recuperer les informations de localisation");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                //  return "1;" + ex.Message.ToString();
                return new List<SMS_ADSite>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, IPStation, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Domain"></param>
        /// <param name="HostName"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get Computer informations from the active directory")]
        public List<LDAPAccess.LDAP_Computer> GetClientDetailsLDAP(string Domain, string HostName, string UserName, string Password)
        {
            
            List<LDAPAccess.LDAP_Computer> LDAP_Computers = new List<LDAPAccess.LDAP_Computer>();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, HostName, "Starting the call " + methodName);
                if (!string.IsNullOrEmpty(HostName))
                {
                    LDAP_Computers = _ldap.GetComputerDetailsLDAP(Domain, HostName, UserName, Password); 

                };

                return LDAP_Computers;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "Impossible de recuperer les informations LDAP");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                //  return "1;" + ex.Message.ToString();
                throw new Exception(ex.Message);
                    }
            finally
            {
                
                _log.Write(_className, methodName, HostName, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Domain"></param>
        /// <param name="HostName"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [WebMethod(Description = "Remove a computer from the active directory")]
        public bool RemoveClientLDAP(string Domain, string HostName, string UserName, string Password)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            bool Return = false;

            try
            {
                _log.Write(_className, methodName, HostName, "Starting the call " + methodName);
                if (!string.IsNullOrEmpty(HostName))
                {
                    Return = _ldap.RemoveComputerFromLDAP(Domain, HostName, UserName, Password);

                };

                return Return;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "Impossible de supprimer l'objet de puis l AD");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                //  return "1;" + ex.Message.ToString();
                throw new Exception(ex.Message);
            }
            finally
            {

                _log.Write(_className, methodName, HostName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Gateway"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get ADSite by Gateway")]
        public List<SMS_ADSite> GetADSiteFromGateway(string Gateway)
        {
            List<SMS_ADSite> SMS_ADSites = new List<SMS_ADSite>();
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, Gateway, "Starting the call " + methodName);
                if (!string.IsNullOrEmpty(Gateway))
                {
                    SMS_ADSites = _sa.GetADSiteByGateway(Gateway, ConnectionManager);

                }

                return SMS_ADSites;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "Impossible de recuperer les informations de localisation");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                //  return "1;" + ex.Message.ToString();
                return new List<SMS_ADSite>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, Gateway, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "Get all DP Groups available in SCCM")]
        public SMS_DistributionPointList GetAllDPGroups()
        {
            string returnCode = "0";
            System.Diagnostics.StackFrame sf;
            SMS_DistributionPointList ListDPGroups = new SMS_DistributionPointList();
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                ListDPGroups = _sa.GetAllDPGroups(ConnectionManager);

                return ListDPGroups;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information.");
                return new SMS_DistributionPointList();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "Fin de l'appel " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "Get all boundaries available in SCCM")]
        public List<SMS_Boundary> GetAllBoundaries()
        {
            List<SMS_Boundary> SMSBoundaries = new List<SMS_Boundary>();
            string returnCode = "0";
            System.Diagnostics.StackFrame sf;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);

                SMSBoundaries = _sa.GetSiteBoundaries(string.Empty, ConnectionManager);

                return SMSBoundaries;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information.");
                // return returnCode + ";" + ex.Message;
                return new List<SMS_Boundary>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "Fin de l'appel " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SMSBIOSUUID"></param>
        /// <returns></returns>
        [WebMethod(Description = "Check for 'Unknown' Client record by UUID (SMBIOS GUID)")]
        public List<SMS_R_System> GetUnknowClientByUUID(string SMSBIOSUUID)
        {
            string returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            List<SMS_R_System> ListSystem = new List<SMS_R_System>();


            try
            {

                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                ListSystem = _sa.GetClientByUUID(ConnectionManager, SMSBIOSUUID);

                return ListSystem;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_R_System>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="CollectionID"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add a Client in a collection")]
        public int AddClientToCollection(string Name, string CollectionID)
        {
            int returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                returnCode = _sa.AddClientToCollection(ConnectionManager, Name.ToUpper(), CollectionID);

                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = 1;
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="CollectionID"></param>
        /// <returns></returns>
        [WebMethod(Description = "Remove a Client from a collection")]
        public int RemoveClientFromCollection(string Name, string CollectionID)
        {
            int returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;


            try
            {

                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                returnCode = _sa.RemoveClientFromCollection(ConnectionManager, Name.ToUpper(), CollectionID);

                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = 1;
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UUID"></param>
        /// <param name="CollectionID"></param>
        /// <returns></returns>
        [WebMethod(Description = "Remove an unknown Client from a collection")]
        public int RemoveUnknownClientFromCollection(string UUID, string CollectionID)
        {
            int returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;


            try
            {

                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                returnCode = _sa.RemoveUnknownComputerFromCollection(ConnectionManager, UUID, CollectionID);

                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = 1;
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }


        [WebMethod(Description = "Add a package on a Distribution Point")]
        public int AddPackageOnDistributionPoint(string serverName, string PackageID)
        {
            int returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string siteCode = null;


            try
            {
                siteCode=  _sa.GetSiteCodeByDistributionPointName(ConnectionManager, serverName);
                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                returnCode = _sa.AssignPackageToDistributionPoint(ConnectionManager, PackageID, siteCode, serverName);

                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = 1;
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UUID"></param>
        /// <param name="CollectionID"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add an unknown Client from a collection")]
        public int AddUnknownClientToCollection(string UUID, string CollectionID)
        {
            int returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;


            try
            {

                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                returnCode = _sa.AddUnknownComputerToCollection(ConnectionManager, UUID, CollectionID);

                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = 1;
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UUID"></param>
        /// <param name="CollectionID"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add an unknown Client from a collection")]
        public int CreateApplication(string Publisher,string SoftwareVersion,string Title)
        {
            int returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;


            try
            {

                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                returnCode = _sa.CreateApplication(ConnectionManager, Publisher, SoftwareVersion, Title);

                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = 1;
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw ex;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SMSBIOSUUID"></param>
        /// <returns></returns>
        [WebMethod(Description = "Check for 'Unknown' Client record by UUID (SMBIOS GUID)")]
        public int RemoveUnknowClientByUUID(string SMSBIOSUUID)
        {
            int returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                returnCode = _sa.RemoveUnknownClientByUUID(ConnectionManager, SMSBIOSUUID);

                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = 1;
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get all Packages | Boot Images | Windows Updates assign to a Site")]
        public ReturnsPackages GetPackagesListInfos(string serverName, string Type)
        {
            string SiteCode = null;
            List<SMS_Package> PackageList = new List<SMS_Package>();
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                if (_sa.ExistServerName(serverName, ConnectionManager))
                {
                    SiteCode = _sa.GetSiteCodeByServerName(serverName, ConnectionManager);
                    if (string.IsNullOrEmpty(SiteCode))
                    {
                        return new ReturnsPackages() { ListPackage = new List<SMS_Package>(), ReturnCode = 1 };
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Type))
                        {
                            Type = "2";
                        }


                        PackageList.AddRange(_da.SMS_GetPackagesInfos(SiteCode, Type));
                        PackageList.AddRange(_da.SMS_GetImagesInfos(SiteCode, Type));
                        PackageList.AddRange(_da.SMS_GetSoftwareUpdatesInfos(SiteCode, Type));

                    }
                }
                else
                {
                    return null;
                }

                return new ReturnsPackages() { ListPackage = PackageList, ReturnCode = 0 };
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", 1 + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new ReturnsPackages() { ListPackage = new List<SMS_Package>(), ReturnCode = 1 };
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get all applications assign to a Site")]
        public List<SMS_Application> GetApplicationsListInfos(string serverName)
        {
            string SiteCode = null;
            List<SMS_Application> PackageList = new List<SMS_Application>();
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                if (_sa.ExistServerName(serverName, ConnectionManager))
                {
                    SiteCode = _sa.GetSiteCodeByServerName(serverName, ConnectionManager);
                    if (string.IsNullOrEmpty(SiteCode))
                    {
                        return new List<SMS_Application>();
                    }
                    else
                    {
                        PackageList.AddRange(_sa.GetApplicationsBySite(ConnectionManager,SiteCode));
                    }
                }
                else
                {
                    return null;
                }

                return PackageList;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", 1 + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_Application>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }
        }


        [WebMethod(Description = "Deploy an application on a collection of devices or Users")]
        public int AssignApplicationToCollection(string AssignmentName, string ApplicationName, string CollectionID)
        {
            int Return = 0;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, ApplicationName, "Starting the call " + methodName);
                _sa.ApplicationAssignment(ConnectionManager, AssignmentName, ApplicationName, CollectionID);

                return 0;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", 1 + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return 1;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, ApplicationName, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get all task sequences assign to a Site")]
        public List<SMS_TaskSequencePackage> GetTaskSequencesListInfos(string serverName)
        {
            string SiteCode = null;
            List<SMS_TaskSequencePackage> PackageList = new List<SMS_TaskSequencePackage>();
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                if (_sa.ExistServerName(serverName, ConnectionManager))
                {
                    SiteCode = _sa.GetSiteCodeByServerName(serverName, ConnectionManager);
                    if (string.IsNullOrEmpty(SiteCode))
                    {
                        return new List<SMS_TaskSequencePackage>();
                    }
                    else
                    {
                        PackageList.AddRange(_sa.GetTaskSequencesBySite(SiteCode, ConnectionManager));
                    }
                }
                else
                {
                    return null;
                }

                return PackageList;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", 1 + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_TaskSequencePackage>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get the SCCM Server informations")]
        public string GetServerInfos(string serverName)
        {
            string ReturnInfos = null;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                if (_sa.ExistServerName(serverName, ConnectionManager))
                {
                    ReturnInfos = _sa.GetSiteInfosByServerName(serverName, ConnectionManager);
                    if (string.IsNullOrEmpty(ReturnInfos))
                    {
                        ReturnInfos = "1";
                    }
                }
                else
                {
                    ReturnInfos = "1";
                }

                return ReturnInfos;
            }
            catch (Exception ex)
            {
                ReturnInfos = "1";
                _log.Write(_className, methodName, "Exception", ReturnInfos + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return ReturnInfos;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SiteCode"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get the Site code of a SCCM Server")]
        public string GetSiteCodeInfos(string SiteCode)
        {
            string ReturnInfos = null;
            // Connection Myconnection = new Connection();
            // WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, SiteCode, "Starting the call " + methodName);
                ReturnInfos = _da.SMS_GetSiteInfosBySiteCode(SiteCode);

                return ReturnInfos;
            }
            catch (Exception ex)
            {
                ReturnInfos = "1";
                _log.Write(_className, methodName, "Exception", ReturnInfos + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return ReturnInfos;
            }
            finally
            {
                // Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, SiteCode, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [WebMethod(Description = "SCCM Server Exist => 1=Yes | 0=No")]
        public string GetServerExist(string serverName)
        {
            string returnCode = "0";
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string siteCode = null;
            try
            {
                string srvName = serverName.Trim();
                _log.Write(_className, methodName, srvName, "Starting the call " + methodName);
                siteCode = _sa.GetSiteCodeByServerName(srvName, ConnectionManager);

                if (!string.IsNullOrEmpty(siteCode))
                {
                    _log.Write(_className, methodName, srvName, "Recherche de l'existence du site associe au serveur" + srvName + " dans la BDD");
                    if (_da.SMS_CheckSiteExist(siteCode) == 1)
                    {
                        returnCode = "1";
                        _log.Write(_className, methodName, srvName, "Le Serveur " + srvName + " Existe dans la BDD SCCM");
                    }
                }

                _log.Write(_className, methodName, srvName, "Recherche de l'existence du site dans le WMI SCCM");
                if (_sa.ExistServerName(srvName, ConnectionManager))
                {
                    returnCode = "1";
                    _log.Write(_className, methodName, srvName, "Le Serveur " + srvName + " existe dans le WMI SCCM");
                }

                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Le Site existe dans l'infra SCCM");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get a parent of a SCCM server")]
        public string GetParentSiteInfos(string serverName)
        {
            string returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                if (_sa.ExistServerName(serverName, ConnectionManager))
                {

                    returnCode = _sa.ParentInfos(serverName, ConnectionManager);
                }
                else
                {
                    returnCode = null;
                }
                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Client Exist ? 1=Yes | 0=No")]
        public string GetClientExist(string serverName)
        {
            string returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                _log.Write(_className, methodName, serverName, "Une demande d informations va etre traitee.");
                if (_sa.ExistClientName(serverName, ConnectionManager))
                {
                    returnCode = "1";
                }
                else
                {
                    returnCode = "0";
                }
                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieve Clients associate to the UserName (Without domain)")]
        public List<SMS_R_System> GetClientByUserName(string UserName)
        {
            List<SMS_R_System> ListClient = new List<SMS_R_System>();
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, UserName, "Starting the call " + methodName);
                _log.Write(_className, methodName, UserName, "Une demande d informations va etre traitee.");
                ListClient =_sa.GetClientByUserName(ConnectionManager,UserName);

                return ListClient;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", 1 + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_R_System>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, UserName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieve Clients associate to the UserName (Without domain)")]
        public List<SMS_R_System> GetClientByName(string Name)
        {
            List<SMS_R_System> ListClient = new List<SMS_R_System>();
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, Name, "Starting the call " + methodName);
                _log.Write(_className, methodName, Name, "Une demande d informations va etre traitee.");
                ListClient = _sa.GetClientByName( ConnectionManager, Name);

                return ListClient;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", 1 + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_R_System>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, Name, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Subnet"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieve Clients associate to a subnet (format = > 192.168.1.0)")]
        public List<SMS_R_System> GetClientBySubnet(string Subnet)
        {
            List<SMS_R_System> ListClient = new List<SMS_R_System>();
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, Subnet, "Starting the call " + methodName);
                _log.Write(_className, methodName, Subnet, "Une demande d informations va etre traitee.");
                ListClient = _sa.GetClientBySubnet(Subnet, ConnectionManager);

                return ListClient;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", 1 + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_R_System>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, Subnet, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of SMS_R_System</returns>
        [WebMethod(Description = "Retrieve all clients informations available in SCCM")]
        public List<SMS_R_System> GetAllClient()
        {
            string returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            List<SMS_R_System> ListSystem = new List<SMS_R_System>();


            try
            {

                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                ListSystem =_sa.GetAllClient(ConnectionManager);

                return ListSystem;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_R_System>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "Retrieve all clients informations available in SCCM")]
        public List<SMS_Admin> GetAllUserAdmin()
        {
            string returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            List<SMS_Admin> ListSystem = new List<SMS_Admin>();


            try
            {

                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                ListSystem = _sa.GetSmsAdmin(ConnectionManager);

                return ListSystem;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_Admin>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>List of SMS_R_System</returns>
        [WebMethod(Description = "Retrieve client informations available in SCCM by BIOS UUID identifier")]
        public List<SMS_R_System> GetClientByUUID(string SMSBIOSUUID)
        {
            string returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            List<SMS_R_System> ListSystem = new List<SMS_R_System>();


            try
            {

                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                ListSystem = _sa.GetClientByUUID(ConnectionManager, SMSBIOSUUID);

                return ListSystem;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_R_System>();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResourceID"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieve client Operating System informations available in SCCM by ResourceID")]
        public SMS_G_System_OS GetClientOSByResourceID(string ResourceID)
        {
            string returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            SMS_G_System_OS OSSystem = new SMS_G_System_OS();

            try
            {

                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                OSSystem = _sa.GetClientOSByResourceID(ConnectionManager, ResourceID);

                return OSSystem;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new SMS_G_System_OS();
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Component"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get All Critical status for a component, example : SMS_MP")]
        public ListComponent GetCriticalSiteComponent(string Component)
        {
            ListComponent ListCriticalSiteFinal = new ListComponent();
            List<SMS_Site> ListPrimary = new List<SMS_Site>();
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, Component, "Starting the call " + methodName);
                ListPrimary = _sa.GetAllPrimarySite(ConnectionManager);
                foreach (SMS_Site Primary in ListPrimary)
                {
                   
                        ListComponent ListSiteCritical = new ListComponent();
                        ListSiteCritical = _da.SMS_GetComponentCritical(Primary.ServerName,Primary.SiteCode, Component);
                        if (ListSiteCritical == null)
                        {
                            _log.Write(_className, methodName, Component, "Impossible de recuperer les composants en erreur depuis le site " + Primary.SiteName);
                            _log.Write(_className, methodName, Component, "Verifie la connexion SQL");
                        }
                        else
                        {
                            if (ListSiteCritical.Components.Count > 0)
                            {
                                foreach (utils.Component Composant in ListSiteCritical.Components)
                                {
                                    ListCriticalSiteFinal.Components.Add(Composant);
                                }

                            }
                        }
                    
                }

                return ListCriticalSiteFinal;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return null;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, Component, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ServerName"></param>
        /// <param name="PackageID"></param>
        /// <returns></returns>
        [WebMethod(Description = "Refresh a package assign to a distribution point")]
        public string RefreshPackage(string ServerName, string PackageID)
        {
            string returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string SiteCode = string.Empty;

            try
            {

                _log.Write(_className, methodName, ServerName, "Starting the call " + methodName);

                _log.Write(_className, methodName, SiteCode, "Recuperation du code site associe au serveur " + ServerName);
                SiteCode = _sa.GetSiteCodeByServerName(ServerName, ConnectionManager);
                _log.Write(_className, methodName, SiteCode, "Code site associe au serveur " + ServerName + " : " + SiteCode);

                if (!string.IsNullOrEmpty(SiteCode) & _sa.ExistPackageID(PackageID, ConnectionManager))
                {
                    _log.Write(_className, methodName, SiteCode, "Verification de l'existence d'une instance de package " + PackageID + " associe au " + ServerName);
                    if (!_sa.ExistPackageOnDp(SiteCode, PackageID, ConnectionManager))
                    {
                        _log.Write(_className, methodName, SiteCode, "Ajout d'une nouvelle instance de package " + PackageID + " associe au " + ServerName);
                        if (_sa.AddPackage(SiteCode, PackageID, ConnectionManager))
                        {
                            _log.Write(_className, methodName, SiteCode, "Ajout d'une nouvelle instance de package " + PackageID + " associe au " + ServerName + " OK");
                        }
                    }
                    else
                    {
                        _log.Write(_className, methodName, SiteCode, "Une instance existe deja pour le package " + PackageID + " sur le serveur " + ServerName);
                    }

                    _log.Write(_className, methodName, SiteCode, "Rafraichissement du statut de package " + PackageID + " associe au " + ServerName);
                    _da.SMS_RefreshPackageStatus(SiteCode, PackageID);
                    _log.Write(_className, methodName, SiteCode, "Rafraichissement du statut de package " + PackageID + " associe au " + ServerName + " OK");

                    _log.Write(_className, methodName, SiteCode, "Rafraichissement du package " + PackageID + " sur le Serveur " + ServerName);
                    if (_sa.RefreshPackage(SiteCode, PackageID, ConnectionManager))
                    {
                        _log.Write(_className, methodName, ServerName, "Rafraichissement du package " + PackageID + " sur le serveur " + ServerName + " OK");
                        returnCode = "OK";
                    }
                    else
                    {
                        _log.Write(_className, methodName, ServerName, "Rafraichissement du package " + PackageID + " sur le serveur " + ServerName + " KO");
                        returnCode = "KO";
                    }
                }
                else
                {
                    _log.Write(_className, methodName, ServerName, "Rafraichissement du package " + PackageID + " sur le serveur " + ServerName + " KO");
                    returnCode = "KO";
                }


                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, ServerName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ServerName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Refresh all packages assign to a distribution point")]
        public string RefreshAllPackages(string ServerName)
        {
            string returnCode;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, ServerName, "Starting the call " + methodName);

                if (_sa.RefreshAllPackage(ServerName, ConnectionManager))
                {
                    returnCode = "OK";
                }
                else
                {
                    returnCode = "KO";
                }


                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, ServerName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ServerName"></param>
        /// <param name="PackageID"></param>
        /// <returns></returns>
        [WebMethod(Description = "Remove a Package from a distribution point")]
        public int RemovePackageFromDistributionPoint(string ServerName, string PackageID)
        {
            int returnCode =1;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string SiteCode;
            try
            {

                _log.Write(_className, methodName, ServerName, "Starting the call " + methodName);

                SiteCode = _sa.GetSiteCodeByDistributionPointName(ConnectionManager, ServerName);

                if (!string.IsNullOrEmpty(SiteCode))
                {
                    if (_sa.RemovePackage(SiteCode, ServerName,  PackageID, ConnectionManager))
                    {
                        returnCode =0;
                    }
                }
                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = 1;
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, ServerName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ServerName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Remove client in SCCM by Name")]
        public int RemoveClientByName(string ServerName)
        {
            int returnCode = 1;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, ServerName, "Starting the call " + methodName);
               returnCode = _sa.RemoveClientByName(ServerName, ConnectionManager);
                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, ServerName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SMSBIOSUUID"></param>
        /// <returns></returns>
        [WebMethod(Description = "Remove client in SCCM by BIOS UUID identifier")]
        public int RemoveClientByUUID(string SMSBIOSUUID)
        {
            int returnCode=1;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                _log.Write(_className, methodName, methodName, "Une demande d informations va etre traitee.");
                returnCode = _sa.RemoveClientByUUID(ConnectionManager, SMSBIOSUUID);
                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Error retrieving information");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get all Collection ID associate to a client")]
        public string GetCollectionID(string serverName)
        {
            String ReturnID;
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);

                ReturnID = _da.SMS_GetCollectionID(serverName);
                if (string.IsNullOrEmpty(ReturnID))
                {
                    return "1";
                }
                else
                {
                    return ReturnID;
                }
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "1;" + "Erreur lors de la recuperation des Collections IDs");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return "1;" + "Erreur lors de la recuperation des Collections IDs";
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get all direct collection assignement to a Client")]
        public string GetDirectCollectionID(string serverName)
        {
            string returnCode = "1";
            String ReturnID;
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);

                ReturnID = _da.SMS_GetDirectCollectionID(serverName);
                if (string.IsNullOrEmpty(ReturnID))
                {
                    return "1";
                }
                else
                {
                    return ReturnID;
                }
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Erreur lors de la recuperation des Collections IDs");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return "1";
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// Ajoute le serveur secondaire ServerName à la collection d'id ID
        /// </summary>
        /// <param name="ServerName">Nom du serveur secondaire</param>
        /// <param name="ID">Id de la collection à laquelle ajouter le serveur</param>
        /// <returns></returns>
        [WebMethod(Description = "Add a distribution Group to a Collection")]
        public string AddSecondaryToCollection(string ServerName, string ID)
        {
            string returnCode = "1";
            int RessourceID = 0;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, ServerName, "Starting the call " + methodName);

                if (_sa.ExistClientName(ServerName, ConnectionManager))
                {
                    RessourceID = _sa.GetRessourceID(ServerName, ConnectionManager);

                    if (_sa.AddSecondaryToCollection(ServerName, ID, RessourceID, ConnectionManager) == true)
                    {
                        returnCode = "0";
                    }
                    else
                    {
                        returnCode = "1";
                    }

                }
                else
                {
                    returnCode = "1";
                }
                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Erreur lors l'ajout de " + ServerName + " à la collection ID " + ID);
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return "1";
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, ServerName, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ServerName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Check if client exist in the collection AllSystems")]
        public string IsInAllSystemsCollection(string ServerName)
        {
            string returnCode = "1";
            int RessourceID = 0;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, ServerName, "Starting the call " + methodName);
                if (_sa.ExistClientName(ServerName, ConnectionManager))
                {
                    RessourceID = _sa.GetRessourceID(ServerName, ConnectionManager);
                    if (_sa.IsCollectionMembers("SMS00001", RessourceID, ConnectionManager) == true)
                    {
                        returnCode = "0";
                    }
                    else
                    {
                        returnCode = "1";
                    }

                }
                else
                {
                    returnCode = "1";
                }
                return returnCode;
            }
            catch (Exception ex)
            {
                returnCode = "1";
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Erreur lors la verification de " + ServerName + ".");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return "1";
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, ServerName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Packages"></param>
        /// <returns></returns>
        [WebMethod(Description = "Check if a package exist in SCCM")]
        public string IsPackageExist(List<String> Packages)
        {
            string returnCode = "1";
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string RetFinal = null;
            Boolean RetCheckPackage;
            try
            {
                _log.Write(_className, methodName, methodName, "Starting the call " + methodName);
                foreach (string Package in Packages)
                {
                    RetCheckPackage = _sa.ExistPackageID(Package, ConnectionManager);
                    if (string.IsNullOrEmpty(RetFinal))
                    {
                        RetFinal = Package + "," + RetCheckPackage;
                    }
                    else
                    {
                        RetFinal += "|" + Package + "," + RetCheckPackage;
                    }
                }

                return "0;" + RetFinal;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode + ";" + ex.Message;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, methodName, "End of  the call " + methodName);
            }

        }

        /// <summary>
/// 
/// </summary>
/// <param name="DPName"></param>
/// <param name="DPMoRemoveName"></param>
/// <returns></returns>
        [WebMethod(Description = "Copy all packages on the Dp from a Reference Distribution Point")]
        public string CopyPackageOnDP(string DPName, string DPModelName)
        {
            string returnCode = "1";
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, DPName, "Starting the call " + methodName);
                if (_sa.PushPackageOnDp(DPName, DPModelName, ConnectionManager) == true)
                {
                    returnCode = "0";
                }
                else
                {
                    returnCode = "1";
                }
                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode + ";" + ex.Message;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, DPName, "End of  the call " + methodName);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Remove all site boundaries associte to a Server site")]
        public int RemoveSiteBoundaries(string serverName)
        {
            string sitecode;
            int returnCode = 1;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                if (!_sa.ExistServerName(serverName, ConnectionManager))
                {
                    return returnCode;
                }

                sitecode = _sa.GetSiteCodeByServerName(serverName, ConnectionManager);
                if (_sa.RemoveSiteBoundaries(sitecode, ConnectionManager))
                {
                    returnCode = 0;
                }
                else
                {
                    returnCode = 1;
                }
                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Erreur lors la suppression des boundaries");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Remove a Sender Address")]
        public int RemoveSenderAddress(string serverName)
        {
            string sitecode;
            int returnCode = 1;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string PriSiteCode;
            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                if (!_sa.ExistServerName(serverName, ConnectionManager))
                {
                    return returnCode;
                }

                sitecode = _sa.GetSiteCodeByServerName(serverName, ConnectionManager);
                PriSiteCode = _sa.ParentSiteCode(ConnectionManager ,serverName);
                if (_sa.RemoveSenderAddress(PriSiteCode, sitecode, ConnectionManager))
                {
                    returnCode = 0;
                }
                else
                {
                    returnCode = 1;
                }
                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Erreur lors du passage de la fonction RemoveSenderAddress");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="ParentSiteCode"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add a sender Address in order to communicate with a new site recently created")]
        public int AddSenderAddress(string serverName, string ParentSiteCode)
        {
            string sitecode;
            int returnCode = 1;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string FinalServerName = null;

            try
            {

                if (serverName.Contains("."))
                {
                    FinalServerName = serverName.Split('.')[0].ToString();
                }
                else
                {
                    FinalServerName = serverName;
                }

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                if (!_sa.ExistServerName(FinalServerName, ConnectionManager))
                {
                    return returnCode;
                }

                sitecode = _sa.GetSiteCodeByServerName(FinalServerName, ConnectionManager);
                if (_sa.AddSenderAddress(ParentSiteCode, sitecode, serverName, ConnectionManager))
                {
                    returnCode = 0;
                }
                else
                {
                    returnCode = 1;
                }
                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Erreur lors du passage de la fonction AddSenderAddress");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }

        }


        [WebMethod(Description = "Add a distribution point to a Distribution Group")]
        public string AddDPGroups(string serverName, string DPGroupName)
        {
            string sitecode;
            string returnCode = null;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                if (!_sa.ExistServerName(serverName, ConnectionManager))
                {
                    return returnCode;
                }

                sitecode = _sa.GetSiteCodeByServerName(serverName, ConnectionManager);

                if (_sa.AddSiteSystemToDpGroup(DPGroupName, sitecode, serverName, ConnectionManager))
                {
                    returnCode = "OK";
                }
                else
                {
                    returnCode = "KO";
                }


                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Erreur lors l'ajout du groupe de DP");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }

        }

         /// <summary>
         /// 
         /// </summary>
         /// <param name="ComputerName"></param>
         /// <param name="OldComputerName"></param>
         /// <param name="userName"></param>
         /// <returns></returns>
        [WebMethod(Description = "Add a computer association between a computer and older computer for a single user")]
        public string AddComputerAssociationForMigration(string ComputerName, string OldComputerName, string userName,MigrationBehavior MigrationType)
        {
            string returnCode = "1";
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, ComputerName, "Starting the call " + methodName);
                returnCode = _sa.AddComputerAssociationForUser(ComputerName, OldComputerName, userName, MigrationType, ConnectionManager);   
                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "error during the computer association");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, ComputerName, "End of  the call " + methodName);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="DPGroupName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Remove a distribution point from a Distribution Group")]
        public string RemoveDPGroups(string serverName, string DPGroupName)
        {
            string sitecode;
            string returnCode = null;
            string DPGroups = null;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                if (!_sa.ExistServerName(serverName, ConnectionManager))
                {
                    return returnCode;
                }

                sitecode = _sa.GetSiteCodeByServerName(serverName, ConnectionManager);

                _log.Write(_className, methodName, serverName, "Recuperation des NalPath associe au groupe " + DPGroupName);
                if (_sa.ExistDpGroup(DPGroupName, ConnectionManager))
                {
                    DPGroups = _sa.GetNaPathListByDpGroup(DPGroupName, ConnectionManager);
                }

                _log.Write(_className, methodName, serverName, "Nettoyage et suppression du Nalpath depuis la liste du groupe " + DPGroupName);

                string NalPathServer = "[\"Display=\\\\" + serverName + "\\\"]MSWNET:[\"SMS_SITE=" + sitecode + "\"]\\\\" + serverName + "\\";

                List<string> NalPathList = new List<string>();
                string[] NalPathListString = null;

                foreach (string Path in DPGroups.Split(','))
                {
                    if (Path == NalPathServer | Path == "")
                    {
                        _log.Write(_className, methodName, serverName, "Le NalPath " + Path + " ne peut etre ajoute");
                    }
                    else
                    {
                        NalPathList.Add(Path);
                    }
                }

                string TempNalPath = string.Join(",", NalPathList.ToArray());
                NalPathListString = TempNalPath.Split(',');

                if (!string.IsNullOrEmpty(NalPathListString[0]))
                {
                    if (_sa.UpdateNalPathDPGroup(NalPathListString, DPGroupName, ConnectionManager))
                    {
                        returnCode = "OK";
                    }
                    else
                    {
                        returnCode = "KO";
                    }
                }



                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Erreur lors la suppression du Groupe de DP");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="BoundaryServer"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add Site Boundary")]
        public int AddSiteBoundaries(string serverName, SMS_Boundary BoundaryServer)
        {
            string sitecode;
            int returnCode = 1;
            List<SMS_Boundary> boundaries;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            Boolean BoundaryExist = false;
            try
            {
                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                if (!_sa.ExistServerName(serverName, ConnectionManager))
                {
                    return returnCode;
                }

                sitecode = _sa.GetSiteCodeByServerName(serverName, ConnectionManager);

                if (_sa.ExistSiteBoundary(sitecode, ConnectionManager))
                {
                    boundaries = _sa.GetSiteBoundaries(sitecode, ConnectionManager);

                }
                else
                {
                    boundaries = new List<SMS_Boundary>();
                }

                if (!BoundaryExist)
                {
                    boundaries.Add(BoundaryServer);
                }

                string[] boundaryNames = new string[boundaries.Count];
                string[] detailBoundaries = new string[boundaries.Count];
                int cpt = 0;

                foreach (SMS_Boundary boundary in boundaries)
                {
                    boundaryNames[cpt] = boundary.DisplayName;
                    detailBoundaries[cpt] = boundary.Value;
                    _log.Write(_className, methodName, serverName, "Boundary " + boundaryNames[cpt] + " " + detailBoundaries[cpt]);
                    cpt = cpt + 1;
                }

                if (_sa.AddSiteBoundaries(sitecode, serverName, boundaryNames, detailBoundaries, ConnectionManager))
                {
                    //  ConnectionManager.CommitSiteControlFileUpdates(sitecode);
                    returnCode = 0;
                }
                else
                {
                    returnCode = 1;
                }
                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Erreur lors de l'ajout de Boundary");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="BoundaryName"></param>
        /// <param name="IPStart"></param>
        /// <param name="IPEnd"></param>
        /// <param name="ProtectedMode"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update site boudaries")]
        public int UpdateSiteBoundaries(string serverName, string BoundaryName, string IPStart, string IPEnd, Boolean ProtectedMode)
        {
            string sitecode;
            int returnCode = 1;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the call " + methodName);
                if (!_sa.ExistServerName(serverName, ConnectionManager))
                {
                    return returnCode;
                }

                sitecode = _sa.GetSiteCodeByServerName(serverName, ConnectionManager);
                if (_sa.UpdateSiteBoundaries(sitecode, serverName, BoundaryName, (IPStart + "-" + IPEnd), ProtectedMode, ConnectionManager))
                {
                    returnCode = 0;
                }
                else
                {
                    returnCode = 1;
                }

                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Erreur lors du passage de l'appel " + methodName);
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }
        }


        /// <summary>
        /// Supprime un serveur de SCCM uniquement
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        [WebMethod(Description = "Remove a server from SCCL")]
        public string RemoveServerFromSCCM(string serverName)
        {
            string parentSiteCode;
            string siteCode;
            string returnCode = null;
            Connection Myconnection = new Connection();
            WqlConnectionManager ConnectionManager = Myconnection.Connect();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                string srvName = serverName.Trim();
                //TODO : si le serveur est un primaire ou le central, la webmethod retourne OK. Elle devrait retourner KO avec un msg informatif  
                // ou bien permettre la suppression d'un primaire (en excluant le central)
                _log.Write(_className, methodName, srvName, "Starting the call " + methodName);
                if (_sa.isSecondaryServer(srvName, ConnectionManager))
                {
                    siteCode = _sa.GetSiteCodeByServerName(srvName, ConnectionManager);
                    parentSiteCode = _sa.ParentSiteCode(ConnectionManager, serverName);

                    if (_sa.RemoveSenderAddress(parentSiteCode, siteCode, ConnectionManager))
                    {
                        returnCode += "OK";
                    }
                    else
                    {
                        returnCode += "KO";
                    }

                    if (_sa.RemoveSiteBoundaries(siteCode, ConnectionManager))
                    {
                        returnCode += "|OK";
                    }
                    else
                    {
                        returnCode += "|KO";
                    }
                    if (_sa.RemoveSecondarySiteJobs(siteCode))
                    {
                        returnCode += "|OK";
                    }
                    else
                    {
                        returnCode += "|KO";
                    }
                    if (_sa.RemoveSecondarySiteSystem(siteCode, parentSiteCode))
                    {
                        returnCode += "|OK";
                    }
                    else
                    {
                        returnCode += "|KO";
                    }
                    if (_sa.CleanPkcFile(siteCode))
                    {
                        returnCode += "|OK";
                    }
                    else
                    {
                        returnCode += "|KO";
                    }

                    if (_sa.CleanActiveDirectoryEntries(siteCode))
                    {
                        returnCode += "|OK";
                    }
                    else
                    {
                        returnCode += "|KO";
                    }

                    if (_sa.RemoveRequestSchedule(siteCode))
                    {
                        returnCode += "|OK";
                    }
                    else
                    {
                        returnCode += "|KO";
                    }
                    _log.Write(_className, methodName, srvName, "Enregistrement des informations pour le site " + siteCode);
                  //  ConnectionManager.CommitSiteControlFileUpdates(siteCode);
                }
                else
                {
                    returnCode = "OK|OK|OK|OK|OK|OK|OK";
                }
                _log.Write(_className, methodName, srvName, "End of the function");


                return returnCode;

            }

            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", returnCode + ";" + "Erreur a la suppression du Site ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return returnCode;
            }
            finally
            {
                Myconnection.Disconnect(ConnectionManager);
                _log.Write(_className, methodName, serverName, "End of  the call " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [WebMethod(Description = "Send the Secondary Key to The Primary server and conversely in order to synchronize both sites")]
        public byte[] SyncKeySecondary(string FileName, byte[] buffer)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string _serverCode;
            string _Installationpath;
            string inboxDir;

            try
            {
                _serverCode = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\SMS\\Identification", "Site Code", "Nothing").ToString();
                _Installationpath = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\SMS\\Identification", "Installation Directory", "Nothing").ToString();
                inboxDir = _Installationpath + "\\inboxes\\hman.box\\";
                _log.Write(_className, methodName, FileName, "Starting the call " + methodName);

                _log.Write(_className, methodName, FileName, "Verification de la presence d'une ancienne Cle " + Path.GetFileNameWithoutExtension(FileName) + ".Pkc");
                if (File.Exists(inboxDir + "\\PubKey\\" + Path.GetFileNameWithoutExtension(FileName) + ".Pkc"))
                {
                    File.Delete(inboxDir + "\\PubKey\\" + Path.GetFileNameWithoutExtension(FileName) + ".Pkc");
                    _log.Write(_className, methodName, FileName, "Ancienne Cle " + Path.GetFileNameWithoutExtension(FileName) + ".Pkc Supprimee");
                }

                _log.Write(_className, methodName, FileName, "Reception de la Cle " + FileName);
                FileStream file = new FileStream(inboxDir + FileName, FileMode.Create);
                file.Write(buffer, 0, buffer.Length);
                file.Close();
                file.Dispose();

                while (File.Exists(inboxDir + FileName))
                {
                    _log.Write(_className, methodName, FileName, "Cle " + FileName + " en cours de traitement...");
                    Thread.Sleep(500);
                }

                _log.Write(_className, methodName, FileName, "Cle traite " + FileName + " par le site " + _serverCode);


                _log.Write(_className, methodName, FileName, "Transmission de la Cle " + _serverCode + ".CT5");
                return File.ReadAllBytes(inboxDir.Split('\\')[0] + _serverCode + ".CT5");

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, FileName, "Erreur a la Synchronisation du Site");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return null;
            }
            finally
            {
                _log.Write(_className, methodName, FileName, "End of  the call " + methodName);
            }
        }


    } // Fin classe ConfigMgr                                                   //  
} // Fin namespace ConfigMgr.Configuration.Webservice                                   //

