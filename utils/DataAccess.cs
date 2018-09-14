//*******************************************************************************//
//  Developpement :  Raphael DELPLANQUE                                          //
//*******************************************************************************//

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Win32;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using ConfigMgr.Configuration.Webservice.utils.SMS;

namespace ConfigMgr.Configuration.Webservice.utils
{
    //****************************************************************************//
    //  Cette classe fournit la methode d acces a la BDD SMS_Provisionning        //
    //  permettant d obtenir les informations nescessaires a l'installation des   // 
    // serveurs de l infra SCCM.                                                  //
    //  PARAM:     Aucun.                                                         //
    //  Cette classe retourne :                                                   //
    //             un objet de type DataAccess auquel on peut appliquer les       //
    //             methodes suivantes :                                           //
    //                  DataAccess() -> Constructeur                              //
    //                  GetSetupInfos()                                           //
    //                  GetInfosBoundaries()                                      //
    //                  SetFlagACloner()                                          //
    //****************************************************************************//
    /// <summary>
    /// Cette classe fournit la methode d acces a la BDD SMS_Provisionning
    /// permettant d obtenir les informations nescessaires a l'installation des
    /// serveurs de l infra SCCM.
    /// 
    /// </summary>
    /// <summary> 
    /// 
    /// </summary> 
    public class DataAccess
    {
        //************************************************************************//
        // Initialisation des variables de classe                                 //
        //************************************************************************//
        private string _className = "DataAccess";
        /// <summary> 
        /// 
        /// </summary> 
        public Logger _log = new Logger();
        // Recupere la valeur de la chaine de connexion a la base de donnees      //
        // SMS_Provisioning (Voir Web.config - tag connectionSting)               //             

        private string _connectionStringSMS = string.Format(ConfigurationManager.ConnectionStrings["SMS_XXX"].ConnectionString,
                Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\SMS\\SQL Server", "Server", "Nothing"),
                Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\SMS\\SQL Server", "Database Name", "Nothing"));



        //************************************************************************//
        //  Constructeur de DataAccess                                            //
        //************************************************************************//
        /// <summary>
        /// Constructeur de DataAccess
        /// </summary>
        /// <summary> 
        /// 
        /// </summary> 
        public DataAccess()
        {
            try
            {

                // System.Security.Principal.WindowsPrincipal.Current

            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }


        } // Fin Constructeur DataAccess                                          //

        void dispose()
        {
            SqlConnection.ClearAllPools();
        }



        /// <summary> 
        /// 
        /// </summary> 
        public ListComponent SMS_GetComponentCritical(string PrimaryName, string PrimarySiteCode, string ComponentName)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            DataSet Result = new DataSet();
            String MaQuery = null;
            SqlConnection connexion = new SqlConnection(_connectionStringSMS);



            ListComponent ListSiteCritique = new ListComponent();
            _log.Write(_className, methodName, PrimaryName, "Starting the function " + methodName);
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                // Configuration de l appel a la procedure stockee                //

                MaQuery = "SELECT distinct " +
                          "Case v_ComponentSummarizer.Status " +
                          "When 0 Then 'OK' " +
                          "When 1 Then 'Warning' " +
                          "When 2 Then 'Critical' " +
                          "Else ' ' " +
                          "End As 'Status', " +
                          "SiteCode 'Site Code', " +
                          "MachineName 'Site', " +
                          "ComponentName 'Component', " +
                          "Case v_componentSummarizer.State " +
                          "When 0 Then 'Stopped' " +
                          "When 1 Then 'Started' " +
                          "When 2 Then 'Paused' " +
                          "When 3 Then 'Installing' " +
                          "When 4 Then 'Re-Installing' " +
                          "When 5 Then 'De-Installing' " +
                          "Else ' ' " +
                          "END AS 'Thread State', " +
                          "Case v_componentSummarizer.Type " +
                          "When 0 Then 'Autostarting' " +
                          "When 1 Then 'Scheduled' " +
                          "When 2 Then 'Manual' " +
                          "ELSE ' ' " +
                          "END AS 'Startup Type', " +
                          "CASE AvailabilityState " +
                          "When 0 Then 'Online' " +
                          "When 3 Then 'Offline' " +
                          "ELSE ' ' " +
                          "END AS 'Availability State', " +
                          "LastStarted 'Last Started', " +
                          "LastContacted 'Last Status Message' " +
                          "from v_ComponentSummarizer " +
                          "Where ComponentName = '" + ComponentName + "' And Status = 2 ";

                _log.Write(_className, methodName, PrimaryName, "Recuperation des informations du primaire " + PrimaryName);
                _log.Write(_className, methodName, PrimaryName, "Execution de la requete de recuperation des erreurs pour le composant " + ComponentName);
                SqlCommand command = new SqlCommand(MaQuery, connexion);
                // Execution d e la Procedure stockee                            //
                adapter.SelectCommand = command;
                command.Connection.Open();
                adapter.Fill(Result);

                if (command.Connection.State != ConnectionState.Closed)
                {
                    command.Connection.Close();
                    command.Dispose();
                }

                _log.Write(_className, methodName, PrimaryName, "Nombre de site critique recuperes : " + Result.Tables[0].Rows.Count.ToString());

                foreach (DataRow Data in Result.Tables[0].Rows)
                {
                    Component Composant = new Component();
                    Composant.Server = Data["Site"].ToString();
                    Composant.SiteCode = Data["Site Code"].ToString();
                    Composant.ComponentName = Data["Component"].ToString();
                    Composant.Status = Data["Status"].ToString();
                    Composant.State = Data["Thread State"].ToString();
                    Composant.Startup = Data["Startup Type"].ToString();
                    Composant.LastStatus = Data["Last Status Message"].ToString();
                    Composant.LastStartup = Data["Last Started"].ToString();
                    ListSiteCritique.Components.Add(Composant);
                }

                return ListSiteCritique;

            }
            catch (Exception ex)
            {
                string msgText = "WARNING : Echec de recuperation des donnees lors de l'acces a " + connexion.ConnectionString;
                _log.Write(_className, methodName, "Exception", msgText);
                _log.Write(_className, methodName, "Exception", ex.ToString());
                throw new Exception("WARNING : " + ex.Message);

            }
            finally
            {
                _log.Write(_className, methodName, PrimaryName, "Fermeture des AppPools SQL");
                // dans tous les cas on ferme la connexion                        //
                connexion.Close();
                connexion.Dispose();
                SqlConnection.ClearAllPools();
                _log.Write(_className, methodName, PrimaryName, "End of the function");
            }

        }

        /// <summary> 
        /// 
        /// </summary> 
        public String SMS_GetDirectCollectionID(string serverName)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            serverName = serverName.ToUpper();
            DataSet Result = new DataSet();
            String MaQuery = null;
            SqlConnection connexion = new SqlConnection(_connectionStringSMS);
            String IDReturn = null;
            //int i_Row;
            _log.Write(_className, methodName, serverName, "Starting the function " + methodName);
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                // Configuration de l appel a la procedure stockee                //
                //TODO : voir s'il faut restreindre le filtre sur le B.Name pour avoir Cxxx-DEP-* au lieu d'exclure les collections selon les criteres Contains("C000-ORG")
                //v_FullCollectionMembership : champ isobsolete : s'applique au couple SiteID (=CollectionID) et MachineId(ResourceID) de la table CollectionMembers. 
                MaQuery = "select A.CollectionId, B.Name from dbo.v_R_System R join dbo.v_FullCollectionMembership A on R.ResourceID = A.ResourceID join dbo.v_Collection B on B.CollectionID = A.CollectionID Where R.Name0  = '" + serverName + "' And A.IsDirect = 1 And A.IsObsolete = 0";
                SqlCommand command = new SqlCommand(MaQuery, connexion);

                // Execution d e la Procedure stockee                            //
                adapter.SelectCommand = command;
                command.Connection.Open();
                adapter.Fill(Result);

                if (command.Connection.State != ConnectionState.Closed)
                {
                    command.Connection.Close();
                    command.Dispose();
                }

                _log.Write(_className, methodName, serverName, Result.Tables[0].Rows.Count.ToString());

                foreach (DataRow Collection in Result.Tables[0].Rows)
                {
                    if (string.Concat(Collection["CollectionId"]).Contains("SMS") | string.Concat(Collection["Name"]).Contains("All ") | string.Concat(Collection["Name"]).Contains("C000-ORG"))
                    {
                        _log.Write(_className, methodName, serverName, "Collection : " + Collection["CollectionId"] + " Nom : " + Collection["Name"] + " ne sera pas ajoute a la liste " + serverName);
                    }
                    else
                    {
                        _log.Write(_className, methodName, serverName, "Collection : " + Collection["CollectionId"] + " Nom : " + Collection["Name"] + " ajout a la liste de " + serverName);
                        if (string.IsNullOrEmpty(IDReturn))
                        {
                            IDReturn = string.Concat(Collection["CollectionId"]);
                        }
                        else
                        {
                            IDReturn += ";" + string.Concat(Collection["CollectionId"]);
                        }
                    }
                }
                return IDReturn;

                //  return Result; //
            }
            catch (Exception ex)
            {
                string msgText = "WARNING : Echec lors du passage de la fonction " + methodName;
                _log.Write(_className, methodName, "SMS_003", msgText);
                _log.Write(_className, methodName, "Exception", ex.ToString());

                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "Fermeture des AppPools SQL");
                // dans tous les cas on ferme la connexion                        //
                connexion.Close();
                connexion.Dispose();
                SqlConnection.ClearAllPools();
                _log.Write(_className, methodName, serverName, "End of the function");
            }

        }

        /// <summary> 
        /// 
        /// </summary> 
        public String SMS_GetCollectionID(string serverName)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            serverName = serverName.ToUpper();
            DataSet Result = new DataSet();
            String MaQuery = null;
            SqlConnection connexion = new SqlConnection(_connectionStringSMS);
            String IDReturn = null;
            //int i_Row;
            _log.Write(_className, "GetCollectionID", serverName, "Starting the function GetCollectionID");
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                // Configuration de l appel a la procedure stockee                //
                //Les ID à recuperer sont ceux pour lesquels le champ IsObsolete vaut 0
                MaQuery = "select A.CollectionId, B.Name from dbo.v_R_System R join dbo.v_FullCollectionMembership A on R.ResourceID = A.ResourceID join dbo.v_Collection B on B.CollectionID = A.CollectionID Where R.Name0  = '" + serverName + "' And A.IsObsolete=0";
                SqlCommand command = new SqlCommand(MaQuery, connexion);


                // Execution d e la Procedure stockee                            //
                adapter.SelectCommand = command;
                command.Connection.Open();
                adapter.Fill(Result);

                if (command.Connection.State != ConnectionState.Closed)
                {
                    command.Connection.Close();
                    command.Dispose();
                }

                _log.Write(_className, "GetCollectionID", serverName, Result.Tables[0].Rows.Count.ToString());

                foreach (DataRow Collection in Result.Tables[0].Rows)
                {
                    if (string.Concat(Collection["CollectionId"]).Contains("SMS") | string.Concat(Collection["Name"]).Contains("All ") | string.Concat(Collection["Name"]).Contains("C000-ORG"))
                    {
                        _log.Write(_className, "GetCollectionID", serverName, "Collection : " + Collection["CollectionId"] + " Nom : " + Collection["Name"] + " ne sera pas ajoute a la liste " + serverName);
                    }
                    else
                    {
                        _log.Write(_className, "GetCollectionID", serverName, "Collection : " + Collection["CollectionId"] + " Nom : " + Collection["Name"] + " ajout a la liste de " + serverName);
                        if (string.IsNullOrEmpty(IDReturn))
                        {
                            IDReturn = string.Concat(Collection["CollectionId"]);
                        }
                        else
                        {
                            IDReturn += ";" + string.Concat(Collection["CollectionId"]);
                        }
                    }
                }
                return IDReturn;

                //  return Result; //
            }
            catch (Exception ex)
            {
                string msgText = "WARNING : Echec lors du passage de la fonction " + methodName;
                _log.Write(_className, "GetCollectionID", "SMS_003", msgText);
                _log.Write(_className, "GetCollectionID", "Exception", ex.ToString());

                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "Fermeture des AppPools SQL");
                // dans tous les cas on ferme la connexion                        //
                connexion.Close();
                connexion.Dispose();
                SqlConnection.ClearAllPools();
                _log.Write(_className, methodName, serverName, "End of the function");
            }

        }

        /// <summary> 
        /// 
        /// </summary> 
        public List<SMS_Package> SMS_GetImagesInfos(String Sitecode, string Type)
        {
            DataSet Result = new DataSet();
            String MaQuery = null;
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            SqlConnection connexion = new SqlConnection(_connectionStringSMS);
            List<SMS_Package> PackageList = new List<SMS_Package>();
            SMS_Package Package;
            //int i_Row;
            _log.Write(_className, methodName, Sitecode, "Starting the function " + methodName);
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                // Configuration de l appel a la procedure stockee                //


                if (Type == "2")
                {
                    MaQuery = "SELECT vSMS_BootImagePackage.*, v_PackageStatusDistPointsSumm.State," +
                        "v_PackageStatusDistPointsSumm.SourceNALPath AS Location " +
                        "FROM vSMS_BootImagePackage INNER JOIN " +
                        "v_PackageStatusDistPointsSumm ON v_PackageStatusDistPointsSumm.PackageID = vSMS_BootImagePackage.PkgID " +
                        "WHERE (v_PackageStatusDistPointsSumm.SiteCode =  '" + Sitecode + "')";
                }
                else
                {
                    MaQuery = "SELECT DISTINCT vSMS_BootImagePackage.*, PkgStatus.Location, PkgStatus.State, PkgStatus.Status, PkgStatus.Type " +
                              "FROM vSMS_BootImagePackage " +
                              "INNER JOIN PkgStatus ON PkgStatus.ID= vSMS_BootImagePackage.PkgID " +
                              "WHERE PkgStatus.SiteCode = '" + Sitecode + "' And PkgStatus.Type =" + Type;
                }
                SqlCommand command = new SqlCommand(MaQuery, connexion);


                // Execution d e la Procedure stockee                            //
                adapter.SelectCommand = command;
                command.Connection.Open();
                adapter.Fill(Result);

                if (command.Connection.State != ConnectionState.Closed)
                {
                    command.Connection.Close();
                    command.Dispose();
                }

                _log.Write(_className, methodName, Sitecode, "Recuperation des instances : " + Result.Tables[0].Rows.Count.ToString());

                foreach (DataRow Row in Result.Tables[0].Rows)
                {
                    Package = new SMS_Package();
                    Package.Name = Row["Name"].ToString();
                    Package.PackageID = Row["PkgID"].ToString();
                    Package.Language = Row["Language"].ToString();
                    Package.Manufacturer = Row["Manufacturer"].ToString();
                    Package.Description = Row["Description"].ToString();
                    Package.SourceSite = Row["SourceSite"].ToString();
                    Package.StoredPkgPath = Row["StoredPkgPath"].ToString();
                    Package.LastRefreshTime = (DateTime)Row["LastRefresh"];
                    Package.ShareName = Row["ShareName"].ToString();
                    Package.PreferredAddressType = Row["PreferredAddress"].ToString();
                    Package.StoredPkgVersion = (int)Row["StoredPkgVersion"];
                    Package.ShareType = (int)Row["ShareType"];
                    Package.Priority = (int)Row["Priority"];
                    Package.PkgFlags = (int)Row["PkgFlags"];
                    Package.MIFFilename = Row["MIFFilename"].ToString();
                    Package.MIFPublisher = Row["MIFPublisher"].ToString();
                    Package.MIFName = Row["MIFName"].ToString();
                    Package.MIFVersion = Row["MIFVersion"].ToString();
                    Package.SourceVersion = (int)Row["SourceVersion"];
                    Package.SourceDate = (DateTime)Row["SourceDate"];
                    Package.PackageType = (int)Row["PackageType"];
                    Package.AlternateContentProviders = Row["AlternateContentProviders"].ToString();
                    Package.TransformReadiness = (int)Row["TransformReadiness"];
                    Package.TransformAnalysisDate = (DateTime)Row["TransformAnalysisDate"];
                    Package.SedoObjectVersion = Row["SedoObjectVersion"].ToString();
                    Package.NumOfPrograms = (int)Row["NumOfPrograms"];
                    Package.Status = Convert.ToInt32(Row["state"]);

                    if (Package.StoredPkgPath.Contains("MSWNET:[\"SMS_SITE=" + Sitecode + "\"]"))
                    {
                        Package.StoredPkgPath = Package.StoredPkgPath.Replace("MSWNET:[\"SMS_SITE=" + Sitecode + "\"]", string.Empty);
                    }
                    PackageList.Add(Package);
                }

                return PackageList;
                //  return Result; //
            }
            catch (Exception ex)
            {
                string msgText = "WARNING : Echec lors du passage de la fonction " + methodName;
                _log.Write(_className, "GetPackagesInfos", "SMS_003", msgText);
                _log.Write(_className, "GetPackagesInfos", "Exception", ex.ToString());

                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, Sitecode, "Fermetture des AppPools SQL");
                // dans tous les cas on ferme la connexion                        //
                connexion.Close();
                connexion.Dispose();
                SqlConnection.ClearAllPools();
                _log.Write(_className, methodName, Sitecode, "End of the function");
            }

        }



        /// <summary> 
        /// 
        /// </summary> 
        public List<SMS_Package> SMS_GetSoftwareUpdatesInfos(String Sitecode, string Type)
        {
            DataSet Result = new DataSet();
            String MaQuery = null;
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            SqlConnection connexion = new SqlConnection(_connectionStringSMS);
            List<SMS_Package> PackageList = new List<SMS_Package>();
            SMS_Package Update;
            //int i_Row;
            _log.Write(_className, methodName, Sitecode, "Starting the function " + methodName);
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                // Configuration de l appel a la procedure stockee                //

                if (Type == "2")
                {
                    MaQuery = "SELECT vSoftwareUpdatesPackage.*, v_PackageStatusDistPointsSumm.State " +
                              "FROM vSoftwareUpdatesPackage  INNER JOIN " +
                              "v_PackageStatusDistPointsSumm ON v_PackageStatusDistPointsSumm.PackageID = vSoftwareUpdatesPackage.PkgID " +
                              "WHERE (v_PackageStatusDistPointsSumm.SiteCode =  '" + Sitecode + "')";
                }
                else
                {
                    MaQuery = "SELECT vSoftwareUpdatesPackage.*, PkgStatus.Location, PkgStatus.State, PkgStatus.Status, PkgStatus.Type " +
                              "FROM vSoftwareUpdatesPackage " +
                              "INNER JOIN PkgStatus ON PkgStatus.ID = vSoftwareUpdatesPackage.PkgID " +
                              "WHERE PkgStatus.SiteCode ='" + Sitecode + "' And PkgStatus.Type =" + Type;
                }
                SqlCommand command = new SqlCommand(MaQuery, connexion);


                // Execution d e la Procedure stockee                            //
                adapter.SelectCommand = command;
                command.Connection.Open();
                adapter.Fill(Result);

                if (command.Connection.State != ConnectionState.Closed)
                {
                    command.Connection.Close();
                    command.Dispose();
                }

                _log.Write(_className, methodName, Sitecode, "Recuperation des instances : " + Result.Tables[0].Rows.Count.ToString());

                foreach (DataRow Row in Result.Tables[0].Rows)
                {

                    Update = new SMS_Package();
                    Update.Name = Row["Name"].ToString();
                    Update.PackageID = Row["PkgID"].ToString();
                    Update.Language = Row["Language"].ToString();
                    Update.Manufacturer = Row["Manufacturer"].ToString();
                    Update.Description = Row["Description"].ToString();
                    Update.SourceSite = Row["SourceSite"].ToString();
                    Update.StoredPkgPath = Row["StoredPkgPath"].ToString();
                    Update.LastRefreshTime = (DateTime)Row["LastRefresh"];
                    Update.ShareName = Row["ShareName"].ToString();
                    Update.PreferredAddressType = Row["PreferredAddress"].ToString();
                    Update.StoredPkgVersion = (int)Row["StoredPkgVersion"];
                    Update.ShareType = (int)Row["ShareType"];
                    Update.Priority = (int)Row["Priority"];
                    Update.PkgFlags = (int)Row["PkgFlags"];
                    Update.MIFFilename = Row["MIFFilename"].ToString();
                    Update.MIFPublisher = Row["MIFPublisher"].ToString();
                    Update.MIFName = Row["MIFName"].ToString();
                    Update.MIFVersion = Row["MIFVersion"].ToString();
                    Update.SourceVersion = (int)Row["SourceVersion"];
                    Update.SourceDate = (DateTime)Row["SourceDate"];
                    Update.PackageType = (int)Row["PackageType"];
                    Update.AlternateContentProviders = Row["AlternateContentProviders"].ToString();
                    Update.TransformReadiness = (int)Row["TransformReadiness"];
                    Update.TransformAnalysisDate = (DateTime)Row["TransformAnalysisDate"];
                    Update.SedoObjectVersion = Row["SedoObjectVersion"].ToString();
                    Update.NumOfPrograms = (int)Row["NumOfPrograms"];
                    Update.Status = Convert.ToInt32(Row["state"]);

                    if (Update.StoredPkgPath.Contains("MSWNET:[\"SMS_SITE=" + Sitecode + "\"]"))
                    {
                        Update.StoredPkgPath = Update.StoredPkgPath.Replace("MSWNET:[\"SMS_SITE=" + Sitecode + "\"]", string.Empty);
                    }
                    PackageList.Add(Update);
                }

                return PackageList;
                //  return Result; //
            }
            catch (Exception ex)
            {
                string msgText = "WARNING : Echec lors du passage de la fonction " + methodName;
                _log.Write(_className, "GetPackagesInfos", "SMS_003", msgText);
                _log.Write(_className, "GetPackagesInfos", "Exception", ex.ToString());

                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, Sitecode, "Fermeture des AppPools SQL");
                // dans tous les cas on ferme la connexion                        //
                connexion.Close();
                connexion.Dispose();
                SqlConnection.ClearAllPools();
                _log.Write(_className, methodName, Sitecode, "End of the function");
            }

        }


        /// <summary> 
        /// 
        /// </summary> 
        public List<SMS_Package> SMS_GetPackagesInfos(String Sitecode, string Type)
        {
            DataSet Result = new DataSet();
            String MaQuery = null;
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            SqlConnection connexion = new SqlConnection(_connectionStringSMS);
            List<SMS_Package> PackageList = new List<SMS_Package>();
            SMS_Package Package;
            //int i_Row;
            _log.Write(_className, methodName, Sitecode, "Starting the function " + methodName);
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                // Configuration de l appel a la procedure stockee                //

                if (Type == "2")
                {
                    MaQuery = "SELECT vPackage.*, v_PackageStatusDistPointsSumm.State," +
                        "v_PackageStatusDistPointsSumm.SourceNALPath AS Location " +
                        "FROM vPackage INNER JOIN " +
                        "v_PackageStatusDistPointsSumm ON v_PackageStatusDistPointsSumm.PackageID = vPackage.PkgID " +
                        "WHERE (v_PackageStatusDistPointsSumm.SiteCode =  '" + Sitecode + "')";
                }
                else
                {
                    MaQuery = "SELECT DISTINCT vPackage.*, PkgStatus.Location, PkgStatus.State, PkgStatus.Status, PkgStatus.Type " +
                              "FROM vPackage " +
                              "INNER JOIN PkgStatus ON PkgStatus.ID= vPackage.PkgID " +
                              "WHERE PkgStatus.SiteCode = '" + Sitecode + "' And PkgStatus.Type =" + Type;
                }
                SqlCommand command = new SqlCommand(MaQuery, connexion);


                // Execution d e la Procedure stockee                            //
                adapter.SelectCommand = command;
                command.Connection.Open();
                adapter.Fill(Result);

                if (command.Connection.State != ConnectionState.Closed)
                {
                    command.Connection.Close();
                    command.Dispose();
                }

                _log.Write(_className, methodName, Sitecode, "Recuperation des instances : " + Result.Tables[0].Rows.Count.ToString());

                foreach (DataRow Row in Result.Tables[0].Rows)
                {
                    Package = new SMS_Package();
                    Package.Name = Row["Name"].ToString();
                    Package.PackageID = Row["PkgID"].ToString();
                    Package.Language = Row["Language"].ToString();
                    Package.Manufacturer = Row["Manufacturer"].ToString();
                    Package.Description = Row["Description"].ToString();
                    Package.SourceSite = Row["SourceSite"].ToString();
                    Package.StoredPkgPath = Row["StoredPkgPath"].ToString();
                    Package.LastRefreshTime = (DateTime)Row["LastRefresh"];
                    Package.ShareName = Row["ShareName"].ToString();
                    Package.PreferredAddressType = Row["PreferredAddress"].ToString();
                    Package.StoredPkgVersion = (int)Row["StoredPkgVersion"];
                    Package.ShareType = (int)Row["ShareType"];
                    Package.Priority = (int)Row["Priority"];
                    Package.PkgFlags = (int)Row["PkgFlags"];
                    Package.MIFFilename = Row["MIFFilename"].ToString();
                    Package.MIFPublisher = Row["MIFPublisher"].ToString();
                    Package.MIFName = Row["MIFName"].ToString();
                    Package.MIFVersion = Row["MIFVersion"].ToString();
                    Package.SourceVersion = (int)Row["SourceVersion"];
                    Package.SourceDate = (DateTime)Row["SourceDate"];
                    Package.PackageType = (int)Row["PackageType"];
                    Package.AlternateContentProviders = Row["AlternateContentProviders"].ToString();
                    Package.TransformReadiness = (int)Row["TransformReadiness"];
                    Package.TransformAnalysisDate = (DateTime)Row["TransformAnalysisDate"];
                    Package.SedoObjectVersion = Row["SedoObjectVersion"].ToString();
                    Package.NumOfPrograms = (int)Row["NumOfPrograms"];
                    Package.Status = Convert.ToInt32(Row["state"]);

                    if (Package.StoredPkgPath.Contains("MSWNET:[\"SMS_SITE=" + Sitecode + "\"]"))
                    {
                        Package.StoredPkgPath = Package.StoredPkgPath.Replace("MSWNET:[\"SMS_SITE=" + Sitecode + "\"]", string.Empty);
                    }
                    PackageList.Add(Package);
                }

                return PackageList;
                //  return Result; //
            }
            catch (Exception ex)
            {
                string msgText = "WARNING : Echec lors du passage de la fonction " + methodName;
                _log.Write(_className, "GetPackagesInfos", "SMS_003", msgText);
                _log.Write(_className, "GetPackagesInfos", "Exception", ex.ToString());

                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, Sitecode, "Fermeture des AppPools SQL");
                // dans tous les cas on ferme la connexion                        //
                connexion.Close();
                connexion.Dispose();
                SqlConnection.ClearAllPools();
                _log.Write(_className, methodName, Sitecode, "End of the function");
            }

        }

        /// <summary> 
        /// 
        /// </summary> 
        public string SMS_GetClientStatus(string serverName, WqlConnectionManager connection)
        {
            serverName = serverName.ToUpper();
            DataSet Result = new DataSet();
            String MaQuery = null;
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            SqlConnection connexion = new SqlConnection(_connectionStringSMS);
            String IDReturn = null;
            //int i_Row;
            _log.Write(_className, methodName, serverName, "Starting the function " + methodName);
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                // Configuration de l appel a la procedure stockee                //

                MaQuery = "SELECT DISTINCT NetBiosName, AssignedSiteCode, HealthStateName, CASE Health.healthState WHEN 1 THEN 'OK' WHEN 2 THEN 'KO' ELSE 'WARNING' END AS healthState, CASE ErrorCode WHEN 0 THEN 'OK' ELSE 'KO' END AS ErrorCode, LastHealthReportDate FROM v_ClientHealthState AS Health WHERE  (NetBiosName='" + serverName + "')";
                SqlCommand command = new SqlCommand(MaQuery, connexion);

                // Execution d e la Procedure stockee                            //
                adapter.SelectCommand = command;
                command.Connection.Open();
                adapter.Fill(Result);

                if (command.Connection.State != ConnectionState.Closed)
                {
                    command.Connection.Close();
                    command.Dispose();
                }

                _log.Write(_className, "GetClientStatus", serverName, "Recuperation des instances : " + Result.Tables[0].Rows.Count.ToString());

                foreach (DataRow Row in Result.Tables[0].Rows)
                {
                    if (string.Concat(Row["healthState"]) == "OK" | string.Concat(Row["ErrorCode"]) == "OK")
                    {

                        _log.Write(_className, "GetClientStatus", serverName, "Etat : " + Row["healthState"] + " Code statut retour : " + Row["ErrorCode"]);
                        IDReturn = "OK";
                    }
                    else
                    {
                        _log.Write(_className, "GetClientStatus", serverName, "Etat : " + Row["healthState"] + " Code statut retour : " + Row["ErrorCode"]);
                        IDReturn = "KO";
                    }
                }

                return IDReturn;
                //  return Result; //
            }
            catch (Exception ex)
            {
                string msgText = "WARNING : Echec lors du passage de la fonction " + methodName;
                _log.Write(_className, "GetClientStatus", "SMS_003", msgText);
                _log.Write(_className, "GetClientStatus", "Exception", ex.ToString());

                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "Fermeture des AppPools SQL");
                // dans tous les cas on ferme la connexion                        //
                connexion.Close();
                connexion.Dispose();
                SqlConnection.ClearAllPools();
                _log.Write(_className, methodName, serverName, "End of the function");
            }

        }

        /// <summary> 
        /// 
        /// </summary> 
        public bool SMS_ExistPackageStatus(String Sitecode, string PackageID)
        {

            DataSet Result = new DataSet();
            Boolean FindResult = false;
            String MaQuery = null;
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection connexion = new SqlConnection(_connectionStringSMS);

            _log.Write(_className, methodName, Sitecode, "Starting the function " + methodName);
            try
            {
                _log.Write(_className, methodName, Sitecode, "Recherche du statut de package " + PackageID + " rattache au code site " + Sitecode);
                MaQuery = "SELECT * FROM PkgStatus " +
                          "WHERE (ID = '" + PackageID + "') AND (SiteCode = '" + Sitecode + "') AND (Type = '1')";
                SqlCommand command = new SqlCommand(MaQuery, connexion);
                adapter.SelectCommand = command;
                command.Connection.Open();
                adapter.Fill(Result);

                if (Result.Tables[0].Rows.Count > 0)
                {
                    FindResult = true;
                }

                _log.Write(_className, methodName, Sitecode, "Recherche du statut de packag " + PackageID + " effectuee avec succes");
                if (command.Connection.State != ConnectionState.Closed)
                {
                    command.Connection.Close();
                    command.Dispose();
                }

                return FindResult;

            }
            catch (Exception ex)
            {
                string msgText = "WARNING : Echec lors du passage de la fonction " + methodName;
                _log.Write(_className, "GetPackagesInfos", "SMS_003", msgText);
                _log.Write(_className, "GetPackagesInfos", "Exception", ex.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, Sitecode, "Fermeture des AppPools SQL");
                connexion.Close();
                connexion.Dispose();
                SqlConnection.ClearAllPools();
                _log.Write(_className, methodName, Sitecode, "End of the function");
            }
        }

        /// <summary> 
        /// 
        /// </summary> 
        public bool SMS_RefreshPackageStatus(String Sitecode, string PackageID)
        {
            if (SMS_ExistPackageStatus(Sitecode, PackageID))
            {

                DataSet Result = new DataSet();
                String MaQuery = null;
                System.Diagnostics.StackFrame sf;
                sf = new System.Diagnostics.StackFrame();
                string methodName = sf.GetMethod().Name;
                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlConnection connexion = new SqlConnection(_connectionStringSMS);

                _log.Write(_className, methodName, Sitecode, "Starting the function " + methodName);
                try
                {
                    _log.Write(_className, methodName, Sitecode, "Mise a jour du statut de package " + PackageID + " rattache au code site " + Sitecode);
                    MaQuery = "UPDATE PkgStatus " +
                              "SET Status = '2', SourceVersion = '0' " +
                              "WHERE (ID = '" + PackageID + "') AND (SiteCode = '" + Sitecode + "') AND (Type = '1')";
                    SqlCommand command = new SqlCommand(MaQuery, connexion);
                    adapter.SelectCommand = command;
                    command.Connection.Open();
                    adapter.Fill(Result);

                    _log.Write(_className, methodName, Sitecode, "Mise a jour du statut de package " + PackageID + " effectuee avec succes");
                    if (command.Connection.State != ConnectionState.Closed)
                    {
                        command.Connection.Close();
                        command.Dispose();
                    }

                    return true;

                }
                catch (Exception ex)
                {
                    string msgText = "WARNING : Echec lors du passage de la fonction " + methodName;
                    _log.Write(_className, "GetPackagesInfos", "SMS_003", msgText);
                    _log.Write(_className, "GetPackagesInfos", "Exception", ex.ToString());
                    throw new Exception("WARNING : " + ex.Message);
                }
                finally
                {
                    _log.Write(_className, methodName, Sitecode, "Fermeture des AppPools SQL");
                    connexion.Close();
                    connexion.Dispose();
                    SqlConnection.ClearAllPools();
                    _log.Write(_className, methodName, Sitecode, "End of the function");
                }

            }
            else
            {
                return true;
            }


        }

        /// <summary> 
        /// 
        /// </summary> 
        public int SMS_CheckSiteExist(string SiteCode)
        {
            DataSet Result = new DataSet();
            String MaQuery = null;
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            SqlConnection connexion = new SqlConnection(_connectionStringSMS);
            int NbRecords = 0;
            _log.Write(_className, methodName, SiteCode, "Starting the function " + methodName);
            try
            {

                MaQuery = "SELECT * FROM SiteBoundaryADSite WHERE SiteCode = '" + SiteCode + "' " +
                          "SELECT * FROM SiteBoundaryIPSubnet WHERE SiteCode = '" + SiteCode + "' " +
                          "SELECT * FROM SiteControl WHERE SiteCode = '" + SiteCode + "' " +
                          "SELECT * FROM SiteControlNotification WHERE SiteCode = '" + SiteCode + "' " +
                          "SELECT * FROM Sites WHERE SiteCode = '" + SiteCode + "' " +
                          "SELECT * FROM Sites_DATA WHERE SiteCode = '" + SiteCode + "' " +
                          "SELECT * FROM SiteWork WHERE SiteCode = '" + SiteCode + "' " +
                          "SELECT * FROM PkgServers WHERE sitecode = '" + SiteCode + "' " +
                          "SELECT * FROM PkgStatus WHERE sitecode = '" + SiteCode + "'";

                SqlCommand command = new SqlCommand(MaQuery, connexion);
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                command.Connection.Open();
                adapter.Fill(Result);

                if (command.Connection.State != ConnectionState.Closed)
                {
                    command.Connection.Close();
                    command.Dispose();
                }

                _log.Write(_className, methodName, SiteCode, "Recuperation des enregistrements dans toutes les tables");
                foreach (DataTable Table in Result.Tables)
                {
                    NbRecords += Table.Rows.Count;
                }



                if (NbRecords > 0)
                {
                    _log.Write(_className, methodName, SiteCode, "Nombres d'enregistrements trouves " + NbRecords);
                    return 1;
                }
                else
                {
                    _log.Write(_className, methodName, SiteCode, "Aucun enregistrement trouve");
                    return 0;
                }

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "Echec de recuperation de l'existence Site");
                _log.Write(_className, methodName, "Exception", ex.ToString());
                return 2;
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                // dans tous les cas on ferme la connexion                        //
                _log.Write(_className, methodName, SiteCode, "End of the function " + methodName);
                connexion.Close();
                connexion.Dispose();
                SqlConnection.ClearAllPools();
            }

        }

        /// <summary> 
        /// 
        /// </summary> 
        public string SMS_GetSiteInfosBySiteCode(string SiteCode)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet Result = new DataSet();
            string MaQuery = null;
            string ResultSite = null;

            _log.Write(_className, methodName, SiteCode, "Starting the function " + methodName);
            try
            {
                using (SqlConnection connexion = new SqlConnection(_connectionStringSMS))
                {
                    MaQuery = "SELECT Sites.SiteServer, Sites.SiteName, Sites.SiteCode, SitesReport.SiteServer AS ReportSiteServer, SitesReport.SiteName AS ReportSiteName, " +
                        " Sites.ReportToSite AS ReportSiteCode" +
                        " FROM Sites INNER JOIN" +
                        " Sites AS SitesReport ON Sites.ReportToSite = SitesReport.SiteCode" +
                        " WHERE Sites.SiteCode = '" + SiteCode + "'";

                    _log.Write(_className, methodName, SiteCode, "Execution de la requete de recuperation");
                    SqlCommand command = new SqlCommand(MaQuery, connexion);
                    _log.Write(_className, methodName, SiteCode, "Connexion a la DataBase SMS");
                    adapter.SelectCommand = command;

                    if (command.Connection.State != ConnectionState.Open)
                    {
                        command.Connection.Open();
                    }
                    adapter.Fill(Result);
                    if (command.Connection.State != ConnectionState.Closed)
                    {
                        command.Connection.Close();
                        command.Dispose();
                    }
                    if (connexion.State != ConnectionState.Closed)
                    {
                        connexion.Close();
                        connexion.Dispose();
                    }

                }

                foreach (DataRow row in Result.Tables[0].Rows)
                {
                    ResultSite = row["SiteServer"] + "|" + row["SiteName"] + "|" + row["SiteCode"] + "|" + row["ReportSiteServer"] + "|" + row["ReportSiteName"] + "|" + row["ReportSiteCode"];
                }

                return ResultSite;

            }
            catch (Exception ex)
            {
                string msgText = "WARNING : Echec lors du passage de la fonction " + methodName;
                _log.Write(_className, methodName, "Exception", msgText);
                _log.Write(_className, methodName, "Exception", ex.ToString());
                throw new Exception(ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, SiteCode, "Fermeture des AppPools SQL");
                SqlConnection.ClearAllPools();
                _log.Write(_className, methodName, SiteCode, "End of the function");
            }

        }


    } // Fin classe DataAccess                                                    //

} // Fin namespace ConfigMgr.Configuration.Webservice.utils                             //  
