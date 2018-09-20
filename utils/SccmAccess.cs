//*******************************************************************************//
//  Developpement :  Raphael DELPLANQUE                                          //
//*******************************************************************************//

using ConfigMgr.Configuration.Webservice.utils.SMS;
using Microsoft.ConfigurationManagement.AdminConsole.AppManFoundation;
using Microsoft.ConfigurationManagement.ApplicationManagement;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;

namespace ConfigMgr.Configuration.Webservice.utils
{



    /// <summary>
    /// 
    /// </summary>
    public class SccmAccess
    {
        //************************************************************************//
        // Initialisation des variables de classe                                 //
        //************************************************************************//
        private string _className = "SccmAccess";
        /// <summary> 
        /// 
        /// </summary> 
        public Logger _log = new Logger();
        // private DataAccess _da = new DataAccess(); //
        // Recupere les parametres de connexion au provider wmi de SCCM          //             
        private String _serverName;
        private String _serverCode;

        //************************************************************************//
        //  Constructeur de SccmAccess                                            //
        //************************************************************************//
        /// <summary>
        /// Constructeur de SccmAccess
        /// </summary>
        /// <summary> 
        /// 
        /// </summary> 
        public SccmAccess()
        {

            try
            {
                _serverName = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\SMS\\Identification", "Server", "Nothing").ToString();
                _serverCode = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\SMS\\Identification", "Site Code", "Nothing").ToString();
            }
            catch
            {
                throw new Exception("Verifier l identification du serveur");
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean ExistServerName(String serverName, WqlConnectionManager connection)
        {
            serverName = serverName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            Boolean returnCode = false;
            IResultObject sccmInstance = null;

            try
            {
                _log.Write(_className, methodName, serverName, "Starting the function " + methodName);

                smsQuery = "SELECT * FROM SMS_Site Where ServerName Like '%" + serverName + "%'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    _log.Write(_className, methodName, serverName, "Le Site Exist.");
                    returnCode = true;
                }

                smsQuery = "SELECT * FROM SMS_DistributionPointInfo Where ServerName Like '%" + serverName + "%'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    _log.Write(_className, methodName, serverName, "The Distribution Point Exist.");
                    returnCode = true;
                }


                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                _log.Write(_className, methodName, serverName, "Le serveur n Existe pas.");

                return returnCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : Le serveur n Existe pas.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="Adresse"></param>
        /// <param name="DebutRange"></param>
        /// <param name="FinRange"></param>
        /// <returns></returns>
        public Boolean IsInRange(string Adresse, string DebutRange, string FinRange)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            IPAddress rangeStart = IPAddress.Parse(DebutRange);
            IPAddress rangeEnd = IPAddress.Parse(FinRange);
            IPAddress check = IPAddress.Parse(Adresse);

            try
            {

                byte[] rbs = rangeStart.GetAddressBytes();
                byte[] rbe = rangeEnd.GetAddressBytes();
                byte[] cb = check.GetAddressBytes();

                Array.Reverse(rbs);
                Array.Reverse(rbe);
                Array.Reverse(cb);

                UInt32 rs = BitConverter.ToUInt32(rbs, 0);
                UInt32 re = BitConverter.ToUInt32(rbe, 0);
                UInt32 chk = BitConverter.ToUInt32(cb, 0);

                if (chk >= rs && chk <= re)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }
            finally
            {

            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="Connection"></param>
        /// <returns></returns>
        public List<SMS_Boundary> GetSiteCodeByIPBound(String IP, WqlConnectionManager Connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string[] PlageIp;
            string[] CheckIP;
            string IPDepart;
            string IPfin;
            List<SMS_Boundary> Boundaries = new List<SMS_Boundary>();

            try
            {
                _log.Write(_className, methodName, IP, "Starting the function " + methodName);

                string smsQuery = "select * from SMS_Boundary";
                IResultObject sccmInstance = Connection.QueryProcessor.ExecuteQuery(smsQuery);

                _log.Write(_className, methodName, IP, "Recuperation des boundaries associe a l'IP");
                foreach (IResultObject Elements in sccmInstance)
                {
                    switch (Elements["BoundaryType"].IntegerValue)
                    {
                        case (int)SMS_Boundary.Type.IPSUBNET:
                            CheckIP = Elements["Value"].StringValue.Split(Convert.ToChar("."));
                            IPDepart = Elements["Value"].StringValue;
                            IPfin = CheckIP[0] + "." + CheckIP[1] + "." + CheckIP[2] + "." + "255";
                            if (IsInRange(IP, IPDepart, IPfin))
                            {
                                Boundaries.Add(IresultWorker.BoundaryFromIresultObject(Elements));
                                _log.Write(_className, methodName, IP, "Debut Range : " + IPDepart + " , Fin Range " + IPfin);
                            }
                            break;

                        case (int)SMS_Boundary.Type.IPRANGE:
                            PlageIp = Elements["Value"].StringValue.Split(Convert.ToChar("-"));
                            IPDepart = PlageIp[0];
                            IPfin = PlageIp[1];
                            if (IsInRange(IP, IPDepart, IPfin))
                            {
                                Boundaries.Add(IresultWorker.BoundaryFromIresultObject(Elements));
                                _log.Write(_className, methodName, IP, "Debut Range : " + IPDepart + " , Fin Range " + IPfin);
                            }
                            break;
                        case (int)SMS_Boundary.Type.ADSITE:
                            string SubNet = GetRangesADSite(Elements["Value"].StringValue, Connection);
                            if (!string.IsNullOrEmpty(SubNet))
                            {
                                PlageIp = SubNet.Split(Convert.ToChar("-"));
                                IPDepart = PlageIp[0];
                                IPfin = PlageIp[1];
                                if (IsInRange(IP, IPDepart, IPfin))
                                {
                                    Boundaries.Add(IresultWorker.BoundaryFromIresultObject(Elements));
                                    _log.Write(_className, methodName, IP, "Debut Range : " + IPDepart + " , Fin Range " + IPfin);
                                }
                            }
                            break;
                    }
                }

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return Boundaries;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, IP, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="DefaultGateway"></param>
        /// <param name="Connection"></param>
        /// <returns></returns>
        public List<SMS_ADSite> GetADSiteByGateway(String DefaultGateway, WqlConnectionManager Connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string[] PlageIp;
            string IPDepart;
            string IPfin;
            List<SMS_ADSite> SMS_ADSites = new List<SMS_ADSite>();

            try
            {
                _log.Write(_className, methodName, DefaultGateway, "Starting the function " + methodName);

                string smsQuery = "SELECT * FROM SMS_ADSite As ADSite " +
                               "Join SMS_ADSubnet As ADSub On ADSite.SiteID = AdSub.SiteID " +
                               "Join SMS_ADForest As ADForest On ADSite.ForestID=ADForest.ForestID " +
                               "Join SMS_ADDomain As ADDomain On ADDomain.ForestID=ADForest.ForestID Where ADSub.ADSubnetName Like '" + DefaultGateway + "%'";
                IResultObject sccmInstance = Connection.QueryProcessor.ExecuteQuery(smsQuery);

                _log.Write(_className, methodName, DefaultGateway, "Recuperation des boundaries associe a l'IP");
                foreach (IResultObject Element in sccmInstance)
                {
                    IResultObject ADDomain = Element.GenericsArray[0];
                    IResultObject ADForest = Element.GenericsArray[1];
                    IResultObject ADSite = Element.GenericsArray[2];
                    IResultObject ADSub = Element.GenericsArray[3];

                    string SubNet = CalculRangeIP(ADSub["ADSubnetName"].StringValue);
                    PlageIp = SubNet.Split(Convert.ToChar("-"));
                    IPDepart = PlageIp[0];
                    IPfin = PlageIp[1];

                    //if (IsInRange(DefaultGateway, IPDepart, IPfin))
                    //{
                    _log.Write(_className, methodName, DefaultGateway, "Debut Range : " + IPDepart + " , Fin Range " + IPfin);
                    SMS_ADSite Adsite = IresultWorker.ADSiteFromIresultObject(ADSite);

                    List<SMS_ADSubnet> Subnets = new List<SMS_ADSubnet>();
                    Subnets.Add(IresultWorker.ADSubnetFromIresultObject(ADSub));

                    List<SMS_ADDomain> Domains = new List<SMS_ADDomain>();
                    Domains.Add(IresultWorker.ADDomainFromIresultObject(ADDomain));

                    SMS_ADForest Forests;
                    Forests = IresultWorker.ADForestFromIresultObject(ADForest);

                    Adsite.Subnets = Subnets;
                    Adsite.Forests = Forests;
                    Adsite.Domains = Domains;
                    SMS_ADSites.Add(Adsite);
                    // }

                }

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return SMS_ADSites;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, DefaultGateway, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="Connection"></param>
        /// <returns></returns>
        public List<SMS_ADSite> GetADSiteByIPBound(String IP, WqlConnectionManager Connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string[] PlageIp;
            string IPDepart;
            string IPfin;
            List<SMS_ADSite> SMS_ADSites = new List<SMS_ADSite>();

            try
            {
                _log.Write(_className, methodName, IP, "Starting the function " + methodName);

                string smsQuery = "SELECT * FROM SMS_ADSite As ADSite " +
                               "Join SMS_ADSubnet As ADSub On ADSite.SiteID = AdSub.SiteID " +
                               "Join SMS_ADForest As ADForest On ADSite.ForestID=ADForest.ForestID " +
                               "Join SMS_ADDomain As ADDomain On ADDomain.ForestID=ADForest.ForestID";
                IResultObject sccmInstance = Connection.QueryProcessor.ExecuteQuery(smsQuery);

                _log.Write(_className, methodName, IP, "Recuperation des boundaries associe a l'IP");
                foreach (IResultObject Element in sccmInstance)
                {
                    IResultObject ADDomain = Element.GenericsArray[0];
                    IResultObject ADForest = Element.GenericsArray[1];
                    IResultObject ADSite = Element.GenericsArray[2];
                    IResultObject ADSub = Element.GenericsArray[3];

                    string SubNet = CalculRangeIP(ADSub["ADSubnetName"].StringValue);
                    PlageIp = SubNet.Split(Convert.ToChar("-"));
                    IPDepart = PlageIp[0];
                    IPfin = PlageIp[1];

                    if (IsInRange(IP, IPDepart, IPfin))
                    {
                        _log.Write(_className, methodName, IP, "Debut Range : " + IPDepart + " , Fin Range " + IPfin);
                        SMS_ADSite Adsite = IresultWorker.ADSiteFromIresultObject(ADSite);

                        List<SMS_ADSubnet> Subnets = new List<SMS_ADSubnet>();
                        Subnets.Add(IresultWorker.ADSubnetFromIresultObject(ADSub));

                        List<SMS_ADDomain> Domains = new List<SMS_ADDomain>();
                        Domains.Add(IresultWorker.ADDomainFromIresultObject(ADDomain));

                        SMS_ADForest Forests;
                        Forests = IresultWorker.ADForestFromIresultObject(ADForest);

                        Adsite.Subnets = Subnets;
                        Adsite.Forests = Forests;
                        Adsite.Domains = Domains;
                        SMS_ADSites.Add(Adsite);
                    }

                }

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return SMS_ADSites;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, IP, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ADSiteName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string GetRangesADSite(String ADSiteName, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            string Return = null;
            try
            {
                smsQuery = "SELECT AdSub.ADSubnetName FROM SMS_ADSite As ADSite " +
                           "Join SMS_ADSubnet As AdSub On ADSite.SiteID = AdSub.SiteID " +
                           "Where ADSite.ADSiteName = '" + ADSiteName + "'";

                IResultObject sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    Return = instance["ADSubnetName"].StringValue;
                }

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                if (!string.IsNullOrEmpty(Return))
                {
                    Return = CalculRangeIP(Return);
                }

                return Return;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : Erreur lors de la recuperation des Sites AD");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return null;
            }

        } // FIn methode ExistSiteSystemInDpGroup                                //

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Subnet"></param>
        /// <returns></returns>
        public string CalculRangeIP(string Subnet)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                IPAddress ip = IPAddress.Parse(Subnet.Split('/')[0]);
                int bits = Convert.ToInt32(Subnet.Split('/')[1]);

                uint mask = ~(uint.MaxValue >> bits);

                // Convert the IP address to bytes.
                byte[] ipBytes = ip.GetAddressBytes();

                // BitConverter gives bytes in opposite order to GetAddressBytes().
                byte[] maskBytes = BitConverter.GetBytes(mask).Reverse().ToArray();

                byte[] startIPBytes = new byte[ipBytes.Length];
                byte[] endIPBytes = new byte[ipBytes.Length];

                // Calculate the bytes of the start and end IP addresses.
                for (int i = 0; i < ipBytes.Length; i++)
                {
                    startIPBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
                    endIPBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
                }

                // Convert the bytes to IP addresses.
                IPAddress startIP = new IPAddress(startIPBytes);
                IPAddress endIP = new IPAddress(endIPBytes);

                return startIP.ToString() + "-" + endIP.ToString();

            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : Erreur lors de la transformation du subnet en range IP");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RecordID"></param>
        /// <returns></returns>
        public string GetMessageByID(string RecordID)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            String Reponse = null;
            try
            {
                _log.Write(_className, methodName, RecordID, "Starting the function " + methodName);

                XmlDocument xmlSettings = new XmlDocument();
                XmlUrlResolver Resolver = new XmlUrlResolver();
                Resolver.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                xmlSettings.XmlResolver = Resolver;
                xmlSettings.Load("http://" + _serverName + "/SMSReporting_" + _serverCode + "/DescriptionStatusXml.asp?RecordID=" + RecordID);
                Reponse = xmlSettings.SelectSingleNode("configuration/string").InnerText;

                return Reponse;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, RecordID, "End of the function " + methodName);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="siteCode"></param>
        /// <param name="destSiteCode"></param>
        /// <param name="destServerName"></param>
        /// <returns></returns>
        public Boolean AddSenderAddress(WqlConnectionManager connection,
                                               String siteCode,
                                               String destSiteCode,
                                               String destServerName)
        {
            // Recuperation du nom de la methode courante                         //
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            Boolean returnCode = false;
            IResultObject senderAddress = null;
            String AddressType = "MS_LAN";
            String ItemName = destSiteCode + "|" + AddressType;
            String ItemType = "Address";
            int Order = 1;
            Boolean UnlimitedRateForAll = true;

            try
            {
                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);
                if (!ExistSiteCode(siteCode, connection))
                {
                    _log.Write(_className, methodName, siteCode, "Site Code " + siteCode + " non trouve");
                    _log.Write(_className, methodName, siteCode, "Impossible d'ajouter le Sender Adress");
                    throw new Exception("WARNING : Impossible d'ajouter le Sender Adress");
                }

                _log.Write(_className, methodName, siteCode, "Ajout du Sender Adress");
                senderAddress = connection.CreateInstance("SMS_SCI_Address");
                senderAddress["AddressType"].StringValue = AddressType;
                _log.Write(_className, methodName, siteCode, "AddressType : " + senderAddress["AddressType"].StringValue);
                senderAddress["DesSiteCode"].StringValue = destSiteCode;
                _log.Write(_className, methodName, siteCode, "DesSiteCode : " + senderAddress["DesSiteCode"].StringValue);
                senderAddress["ItemName"].StringValue = ItemName;
                _log.Write(_className, methodName, siteCode, "ItemName : " + senderAddress["ItemName"].StringValue);
                senderAddress["ItemType"].StringValue = ItemType;
                _log.Write(_className, methodName, siteCode, "ItemType : " + senderAddress["ItemType"].StringValue);
                senderAddress["Order"].IntegerValue = Order;
                _log.Write(_className, methodName, siteCode, "Order : " + senderAddress["Order"].IntegerValue);
                senderAddress["SiteCode"].StringValue = siteCode;
                _log.Write(_className, methodName, siteCode, senderAddress["SiteCode"].StringValue);

                senderAddress["UnlimitedRateForAll"].BooleanValue = UnlimitedRateForAll;
                senderAddress = WriteScfEmbeddedProperty(senderAddress, "Connection Point", 0, destServerName, "SMS_SITE");
                senderAddress = WriteScfEmbeddedProperty(senderAddress, "LAN Login", 0, "", "");
                senderAddress.Put();
                returnCode = ExistSenderAddress(siteCode, destSiteCode, connection);

                if (returnCode)
                {
                    _log.Write(_className, methodName, siteCode, "Creation SenderAddress => OK");

                }
                else
                {
                    _log.Write(_className, methodName, siteCode, "Creation SenderAddress => KO.");
                }
                return returnCode;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
        }  // Fin methode AddSenderAddress                                       //


        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="siteCode"></param>
        /// <param name="serverName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean AddSiteSystemToDpGroup(String groupName, String siteCode, String serverName, WqlConnectionManager connection)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            Boolean returnCode = false;
            string nalPath = "[\"Display=\\\\" + serverName + "\\\"]MSWNET:[\"SMS_SITE=" + siteCode + "\"]\\\\" + serverName + "\\";
            try
            {
                if (!ExistSiteCode(siteCode, connection))
                {
                    _log.Write(_className, methodName, siteCode, "Site Code " + siteCode + " non trouve. A voir.");
                    _log.Write(_className, methodName, siteCode, "Ajout du site au Goupe Distribution Point \"" + groupName + "\" => KO");
                    throw new Exception("WARNING : Ajout du site au Goupe Distribution Point \"" + groupName + "\" => KO");
                }

                if (ExistSiteSystemInDpGroup(groupName, nalPath, connection))
                {
                    _log.Write(_className, methodName, siteCode, "Site Code " + siteCode + " deja present dans le groupe DP \"" + groupName + "\".");
                    _log.Write(_className, methodName, siteCode, "Ajout du site au Goupe Distribution Point \"" + groupName + "\" => OK");
                    returnCode = true;
                    return returnCode;

                }
                string napathList;
                if (!ExistDpGroup(groupName, connection))
                {
                    napathList = "";
                }
                else
                {
                    napathList = GetNaPathListByDpGroup(groupName, connection);
                    if (napathList == null)
                    {
                        _log.Write(_className, methodName, siteCode, "Pb a la recuperation des donnees associe au groupe \"" + groupName + "\"");
                        _log.Write(_className, methodName, siteCode, "Ajout du site au Goupe Distribution Point \"" + groupName + "\" => KO");
                        throw new Exception("WARNING : Ajout du site au Goupe Distribution Point \"" + groupName + "\" => KO");
                    }
                }

                string[] nalPathTab = new string[napathList.Split(',').Length + 1];
                int cpt = 0;
                foreach (string path in napathList.Split(','))
                {
                    nalPathTab[cpt] = path;
                    cpt = cpt + 1;
                }

                nalPathTab[cpt] = nalPath;

                if (UpdateNalPathDPGroup(nalPathTab, groupName, connection))
                {
                    _log.Write(_className, methodName, siteCode, "Mise a jour du Goupe Distribution Point \"" + groupName + "\" => OK");
                }
                else
                {
                    _log.Write(_className, methodName, siteCode, "Mise a jour du Goupe Distribution Point \"" + groupName + "\" => KO");
                    returnCode = false;
                }

                if (ExistSiteSystemInDpGroup(groupName, nalPath, connection))
                {
                    _log.Write(_className, methodName, siteCode, "Ajout du site au Goupe Distribution Point \"" + groupName + "\" => OK");
                    returnCode = true;
                }
                else
                {
                    _log.Write(_className, methodName, siteCode, "Ajout du site au Goupe Distribution Point \"" + groupName + "\" => KO");
                    returnCode = false;
                }

                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "Impossible d ajouter le site " + siteCode + " au groupe \"" + groupName + "\".");
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }

        } // Fin methode AddSiteSystemToDpGroup                                  //




        /// <summary>
        /// 
        /// </summary>
        /// <param name="NalPathTab"></param>
        /// <param name="GroupName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean UpdateNalPathDPGroup(string[] NalPathTab, String GroupName, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            IResultObject instance = null;

            try
            {
                instance = connection.CreateInstance("SMS_DistributionPointGroup");
                instance["sGroupName"].StringValue = GroupName;
                instance["arrNALPath"].StringArrayValue = NalPathTab;
                instance.Put();

                if (!(instance == null))
                {
                    instance.Dispose();
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }
        } // Fin methode UpdateNalPathDPGroup                                  //

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public String GetNaPathListByDpGroup(String groupName, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string nalPath = null;

            try
            {
                IResultObject DpGroup = connection.GetInstance(@"SMS_DistributionPointGroup.sGroupName='" + groupName + "'");
                foreach (IResultObject instance in DpGroup)
                {
                    nalPath = string.Join(",", instance["arrNALPath"].StringArrayValue);
                }


                if (!(DpGroup == null))
                {
                    DpGroup.Dispose();
                }

                return nalPath;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
        } // Fin methode GetNaPathListByDpGroup                                  //

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="nALPath"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean ExistSiteSystemInDpGroup(String groupName, String nALPath, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string nALPathList;
            string smsQuery;
            Boolean returnCode = false;
            try
            {
                smsQuery = "SELECT * FROM SMS_DistributionPointGroup WHERE sGroupName='" + groupName + "'";
                IResultObject sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    nALPathList = string.Join(",", instance["arrNALPath"].StringArrayValue);

                    if (nALPathList.Contains(nALPath))
                    {
                        returnCode = true;
                    }
                }

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return returnCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : Le site n existe pas dans le groupe de DP");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }

        } // FIn methode ExistSiteSystemInDpGroup                                //


        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean ExistDpGroup(String groupName, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            Boolean returnCode = false;
            try
            {
                smsQuery = "SELECT * FROM SMS_DistributionPointGroup";
                IResultObject sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    string dpGroupName = instance["sGroupName"].StringValue;
                    if (dpGroupName.Contains(groupName))
                    {
                        returnCode = true;
                    }
                }

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return returnCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : Le Groupe de DP n existe pas.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }

        } // Fin methode ExistDpGroup                                            // 


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <summary> 
        /// 
        /// </summary> 
        public Boolean ExistClientName(String serverName, WqlConnectionManager connection)
        {
            serverName = serverName.ToUpper();

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            Boolean returnCode = false;
            IResultObject sccmInstance = null;


            try
            {
                _log.Write(_className, methodName, serverName, "Starting the function " + methodName);
                _log.Write(_className, methodName, serverName, "Retrieving the Client : " + serverName);

                smsQuery = "SELECT * FROM SMS_R_System Where Name = '" + serverName + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                int nbrecord = 0;
                foreach (IResultObject instance in sccmInstance)
                {

                    if (instance["Name"].StringValue == serverName)
                    {
                        nbrecord += 1;
                    }
                }

                if (nbrecord > 0)
                {
                    returnCode = true;
                }

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return returnCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <summary> 
        /// 
        /// </summary> 
        public List<SMS_R_System> GetAllClient(WqlConnectionManager connection)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            IResultObject sccmInstance = null;
            List<SMS_R_System> ListSystem = new List<SMS_R_System>();


            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Retrieving all Clients.");

                smsQuery = "SELECT * FROM SMS_R_System";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    ListSystem.Add(IresultWorker.SystemFromIresultObject(instance));

                }


                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return ListSystem;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_R_System>();
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }


        private IResultObject GetCollection(WqlConnectionManager connection, string CollectionID)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            IResultObject sccmInstance = null;
            IResultObject RetInstance = null;


            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Rerieving the collection " + CollectionID);

                smsQuery = "SELECT * FROM SMS_Collection WHERE CollectionID = '" + CollectionID + "' AND CollectionType = 2";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    RetInstance = instance;
                    _log.Write(_className, methodName, methodName, "The collection " + CollectionID + " Exist.");
                }


                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return RetInstance;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : The collection does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return null;
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }


        private SMS_Collection GetCollectionByID(WqlConnectionManager connection, string CollectionID)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            IResultObject sccmInstance = null;
            SMS_Collection RetInstance = null;


            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Rerieving the collection " + CollectionID);

                smsQuery = "SELECT * FROM SMS_Collection WHERE CollectionID = '" + CollectionID + "' AND CollectionType = 2";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    RetInstance = IresultWorker.CollectionFromIresultObject(instance);
                    _log.Write(_className, methodName, CollectionID, "The Collection " + CollectionID + " Exist.");
                }


                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return RetInstance;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : The Collection does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return null;
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="Name"></param>
        /// <param name="CollectionID"></param>
        /// <returns></returns>
        public int AddClientToCollection(WqlConnectionManager connection, string Name, string CollectionID)
        {
            int Return = 0;
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Adding Client " + Name + " to the collection " + CollectionID);

                List<SMS_R_System> ListClient = GetClientByName(connection, Name);
                IResultObject collection = GetCollection(connection, CollectionID);

                if (collection != null)
                {

                    foreach (SMS_R_System Client in ListClient)
                    {


                        IResultObject newRule = connection.CreateInstance("SMS_CollectionRuleDirect");
                        newRule["ResourceClassName"].StringValue = "SMS_R_System";
                        newRule["ResourceID"].StringValue = Client.ResourceID.ToString();
                        newRule["RuleName"].StringValue = Client.Name.ToString();

                        Dictionary<string, object> methodParams = new Dictionary<string, object>();
                        methodParams.Add("CollectionRule", newRule);

                        IResultObject result = collection.ExecuteMethod("AddMembershipRule", methodParams);
                        Return += result["ReturnValue"].IntegerValue;

                    }

                    if (Return == 0)
                    {
                        Dictionary<string, object> refreshParams = new Dictionary<string, object>();
                        collection.ExecuteMethod("RequestRefresh", refreshParams);
                    }

                    if (!(collection == null))
                    {
                        collection.Dispose();
                    }
                }



                return Return;
            }
            catch (SmsQueryException ex)
            {
                Return = 1;
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return Return;
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="UUID"></param>
        /// <param name="CollectionID"></param>
        /// <returns></returns>
        public int AddUnknownComputerToCollection(WqlConnectionManager connection, string UUID, string CollectionID)
        {
            int Return = 1;
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Adding Unknown Client " + UUID + " to the collection " + CollectionID);

                SMS_R_System UnknownClient = GetUnknownClientByUUID(connection, UUID);

                if (UnknownClient != null)
                {
                    IResultObject collection = GetCollection(connection, CollectionID);

                    if (collection != null)
                    {
                        IResultObject newRule = connection.CreateInstance("SMS_CollectionRuleDirect");
                        newRule["ResourceClassName"].StringValue = "SMS_R_System";
                        newRule["ResourceID"].StringValue = UnknownClient.ResourceID.ToString();
                        newRule["RuleName"].StringValue = UnknownClient.Name.ToString();

                        Dictionary<string, object> methodParams = new Dictionary<string, object>();
                        methodParams.Add("CollectionRule", newRule);

                        IResultObject result = collection.ExecuteMethod("AddMembershipRule", methodParams);

                        if (result["ReturnValue"].IntegerValue == 0)
                        {
                            Dictionary<string, object> refreshParams = new Dictionary<string, object>();
                            collection.ExecuteMethod("RequestRefresh", refreshParams);

                            Return = 0;
                        }
                    }


                    if (!(collection == null))
                    {
                        collection.Dispose();
                    }
                }



                return Return;
            }
            catch (SmsQueryException ex)
            {
                Return = 1;
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return Return;
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="UUID"></param>
        /// <param name="CollectionID"></param>
        /// <returns></returns>
        public int RemoveUnknownComputerFromCollection(WqlConnectionManager connection, string UUID, string CollectionID)
        {
            int Return = 1;
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Removing Unknown Client " + UUID + " from the collection " + CollectionID);

                //' Get the device resource instance to be removed from collection
                SMS_R_System UnknownClient = GetUnknownClientByUUID(connection, UUID);

                if (UnknownClient != null)
                {
                    //' Get the collection instance where the device resource will be removed from
                    IResultObject collection = GetCollection(connection, CollectionID);

                    if (collection != null)
                    {
                        //' Construct dictionary for removal parameters
                        Dictionary<string, object> removeParams = new Dictionary<string, object>();

                        //' Construct new direct rule instance and add as param
                        IResultObject removalRule = connection.CreateInstance("SMS_CollectionRuleDirect");
                        removalRule["ResourceID"].StringValue = UnknownClient.ResourceID.ToString();
                        removeParams.Add("collectionRule", removalRule);

                        //' Remove direct rule from collection
                        IResultObject execute = collection.ExecuteMethod("DeleteMembershipRule", removeParams);

                        if (execute["ReturnValue"].IntegerValue == 0)
                        {
                            Dictionary<string, object> refreshParams = new Dictionary<string, object>();
                            IResultObject exec = collection.ExecuteMethod("RequestRefresh", refreshParams);

                            Return = 0;
                        }
                    }


                    if (!(collection == null))
                    {
                        collection.Dispose();
                    }
                }


                return Return;
            }
            catch (SmsQueryException ex)
            {
                Return = 1;
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return Return;
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="Name"></param>
        /// <param name="CollectionID"></param>
        /// <returns></returns>
        public int RemoveClientFromCollection(WqlConnectionManager connection, string Name, string CollectionID)
        {
            int Return = 0;
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Removing Client " + Name + " from the collection " + CollectionID);


                List<SMS_R_System> ListClient = GetClientByName(connection, Name);
                IResultObject collection = GetCollection(connection, CollectionID);

                if (collection != null)
                {

                    foreach (SMS_R_System Client in ListClient)
                    {

                        Dictionary<string, object> removeParams = new Dictionary<string, object>();

                        IResultObject removalRule = connection.CreateInstance("SMS_CollectionRuleDirect");
                        removalRule["ResourceID"].StringValue = Client.ResourceID.ToString();
                        removeParams.Add("collectionRule", removalRule);
                        IResultObject execute = collection.ExecuteMethod("DeleteMembershipRule", removeParams);
                        Return += execute["ReturnValue"].IntegerValue;
                    }
                }

                if (Return == 0)
                {
                    Dictionary<string, object> refreshParams = new Dictionary<string, object>();
                    IResultObject exec = collection.ExecuteMethod("RequestRefresh", refreshParams);
                }


                if (!(collection == null))
                {
                    collection.Dispose();
                }

                return Return;
            }
            catch (SmsQueryException ex)
            {
                Return = 1;
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return Return;
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<SMS_R_System> GetClientByName(WqlConnectionManager connection, string Name)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            IResultObject sccmInstance = null;
            List<SMS_R_System> ListSystem = new List<SMS_R_System>();


            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Retrieving the client by Name " + Name);

                smsQuery = "SELECT * FROM SMS_R_System Where Name = '" + Name + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    ListSystem.Add(IresultWorker.SystemFromIresultObject(instance));
                    _log.Write(_className, methodName, Name, "The Client " + Name + " exist.");
                }

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return ListSystem;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_R_System>();
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Subnet"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<SMS_R_System> GetClientBySubnet(string Subnet, WqlConnectionManager connection)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            IResultObject sccmInstance = null;
            List<SMS_R_System> ListSystem = new List<SMS_R_System>();


            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Retrieving the client by Subnet " + Subnet);
                // SELECT * FROM SMS_R_System WHERE IPSubnets[0]='192.168.1.0'
                smsQuery = "SELECT * FROM SMS_R_System Where IPSubnets[0]='" + Subnet + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    ListSystem.Add(IresultWorker.SystemFromIresultObject(instance));
                    _log.Write(_className, methodName, Subnet, "The Client " + Subnet + " exist.");
                }

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return ListSystem;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_R_System>();
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<SMS_R_System> GetClientByUserName(WqlConnectionManager connection, string UserName)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            IResultObject sccmInstance = null;
            List<SMS_R_System> ListSystem = new List<SMS_R_System>();


            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Retrieving the client by UserName " + UserName);

                smsQuery = "SELECT * FROM SMS_R_System Where LastLogonUserName='" + UserName + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    ListSystem.Add(IresultWorker.SystemFromIresultObject(instance));
                    _log.Write(_className, methodName, UserName, "The Client " + UserName + " exist.");

                }

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return ListSystem;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_R_System>();
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }

        /// <summary> 
        /// 
        /// </summary> 
        public SMS_G_System_OS GetClientOSByResourceID(WqlConnectionManager connection, string RecourceID)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            IResultObject sccmInstance = null;
            SMS_G_System_OS OSSystem = new SMS_G_System_OS();


            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Retrieving the GetClientOS by RecourceID " + RecourceID);

                smsQuery = "SELECT * FROM SMS_G_System_OPERATING_SYSTEM " +
                            "Where ResourceID ='" + RecourceID + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    OSSystem = IresultWorker.SystemOSFromIresultObject(instance);
                    _log.Write(_className, methodName, RecourceID, "The SMS_G_System_OPERATING_SYSTEM for " + RecourceID + " exist.");
                }


                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return OSSystem;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new SMS_G_System_OS();
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="SMBIOSGUID"></param>
        /// <returns></returns>
        public SMS_R_System GetUnknownClientByUUID(WqlConnectionManager connection, string SMBIOSGUID)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            IResultObject sccmInstance = null;
            SMS_R_System ListSystem = new SMS_R_System();


            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Retrieving Unknown Client by SMBIOSGUID " + SMBIOSGUID);

                smsQuery = "SELECT * FROM SMS_R_System WHERE Name like 'Unknown' AND " +
                            "SMBIOSGUID='" + SMBIOSGUID + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    ListSystem = IresultWorker.SystemFromIresultObject(instance);
                    _log.Write(_className, methodName, SMBIOSGUID, "The Unknown Client " + SMBIOSGUID + " exist.");
                }


                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return ListSystem;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new SMS_R_System();
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="SMBIOSGUID"></param>
        /// <returns></returns>
        public int RemoveUnknownClientByUUID(WqlConnectionManager connection, string SMBIOSGUID)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            IResultObject sccmInstance = null;
            List<SMS_R_System> ListSystem = new List<SMS_R_System>();


            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, SMBIOSGUID, "Deleting the Unknown Client " + SMBIOSGUID);

                smsQuery = "SELECT * FROM SMS_R_System WHERE Name like 'Unknown' AND " +
                            "SMBIOSGUID='" + SMBIOSGUID + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    instance.Delete();
                    _log.Write(_className, methodName, SMBIOSGUID, "Unknown Client " + SMBIOSGUID + " Delete.");
                }


                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return 0;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return 1;
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="SMBIOSGUID"></param>
        /// <returns></returns>
        public List<SMS_R_System> GetClientByUUID(WqlConnectionManager connection, string SMBIOSGUID)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            IResultObject sccmInstance = null;
            List<SMS_R_System> ListSystem = new List<SMS_R_System>();


            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Retrieving Client by SMBIOSGUID " + SMBIOSGUID);

                smsQuery = " SELECT * FROM  SMS_R_System " +
                            "Where SMBIOSGUID='" + SMBIOSGUID + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    ListSystem.Add(IresultWorker.SystemFromIresultObject(instance));
                    _log.Write(_className, methodName, methodName, "Client " + SMBIOSGUID + " exist.");
                }


                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return ListSystem;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return new List<SMS_R_System>();
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="SMBIOSGUID"></param>
        /// <returns></returns>
        public int RemoveClientByUUID(WqlConnectionManager connection, string SMBIOSGUID)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            IResultObject sccmInstance = null;

            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, SMBIOSGUID, "Deleting the Client " + SMBIOSGUID);

                smsQuery = " SELECT * FROM  SMS_R_System " +
                            "Where SMBIOSGUID='" + SMBIOSGUID + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {
                    instance.Delete();
                    _log.Write(_className, methodName, SMBIOSGUID, "Client " + SMBIOSGUID + " Delete.");
                }


                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }
                else
                {
                    return 2;
                }

                return 0;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : The Client does not exist.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return 1;
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public int GetRessourceID(String serverName, WqlConnectionManager connection)
        {
            serverName = serverName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            IResultObject sccmInstance = null;

            _log.Write(_className, methodName, serverName, "Starting the function " + methodName);
            try
            {
                //clause obsolete=0 ou is null (exemple pour itgpit01/6914 vs 4698) pour ne pas obtenir un RessourceId obsolete
                smsQuery = "SELECT Name, ResourceId FROM SMS_R_System Where Name = '" + serverName + "' AND (Obsolete=0 or Obsolete is null)";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                foreach (IResultObject instance in sccmInstance)
                {

                    if (instance["Name"].StringValue == serverName)
                    {
                        _log.Write(_className, methodName, serverName, "L'ID Client est " + instance["ResourceID"].StringValue + ".");

                        return Convert.ToInt32(instance["ResourceID"].StringValue);
                    }
                }
                _log.Write(_className, methodName, serverName, "Impossible de recuperer l'ID Client.");


                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return 0;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of the function " + methodName);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ServerName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public int RemoveClientByName(String ServerName, WqlConnectionManager connection)
        {
            ServerName = ServerName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, ServerName, "Starting the function " + methodName);
                //On supprime même les affectations du nom du serveur à un id de ressource obsoletes
                string smsQuery = "SELECT ResourceID,Name FROM SMS_R_SYSTEM where Name = '" + ServerName + "'";
                IResultObject sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    _log.Write(_className, methodName, "RemoveClientSccm", "L'element " + instance["Name"].StringValue + " va etre supprime.");
                    instance.Delete();
                }

                _log.Write(_className, methodName, "RemoveClientSccm", "Suppression du client OK.");

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }
                else
                {
                    return 2;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, ServerName, "End of the function " + methodName);

            }
        }

        /// <summary>
        /// Ajoute le secondaire ServerName d'ID RessourceID à la collection IDCollection si ce n'est pas le cas
        /// </summary>
        /// <param name="ServerName">Nom du serveur</param>
        /// <param name="IDCollection">Id de la collection à laquelle ajouter l'id de ressource du serveur</param>
        /// <param name="RessourceID">ID de resource associe au serveur</param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean AddSecondaryToCollection(String ServerName, String IDCollection, Int32 RessourceID, WqlConnectionManager connection)
        {
            ServerName = ServerName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            IResultObject I_CollSettings = null;
            IResultObject NewStaticRule = null;
            Dictionary<String, Object> AddMembershipRuleParameters = new Dictionary<String, Object>();
            Dictionary<String, Object> RequestRefreshParameters = new Dictionary<String, Object>();


            try
            {
                _log.Write(_className, methodName, ServerName, "Starting the function " + methodName);
                if (IsResourceServerObsolete(RessourceID, connection))
                {
                    _log.Write(_className, methodName, ServerName, string.Format("Le serveur {0} d'ID {1} est obsolete. Son ajout dans les collections n'est pas possible.", ServerName, RessourceID));
                    return false;
                }
                else
                {
                    if (IsCollectionMembers(IDCollection, RessourceID, connection) == true)
                    {
                        _log.Write(_className, methodName, ServerName, ServerName + " est deja membre de la collection " + IDCollection);
                        return true;
                    }
                    else
                    {
                        I_CollSettings = connection.GetInstance("SMS_Collection.CollectionID='" + IDCollection + "'");
                        NewStaticRule = connection.CreateInstance("SMS_CollectionRuleDirect");

                        NewStaticRule["ResourceClassName"].StringValue = "SMS_R_System";
                        NewStaticRule["ResourceID"].IntegerValue = RessourceID;
                        NewStaticRule["RuleName"].StringValue = ServerName;

                        _log.Write(_className, methodName, ServerName, "Ajout du serveur : " + ServerName + " dans la collection " + IDCollection);

                        AddMembershipRuleParameters.Add("collectionRule", NewStaticRule);
                        I_CollSettings.ExecuteMethod("AddMembershipRule", AddMembershipRuleParameters);

                        RequestRefreshParameters.Add("IncludeSubCollections", false);
                        I_CollSettings.ExecuteMethod("RequestRefresh", RequestRefreshParameters);
                        _log.Write(_className, methodName, ServerName, "Serveur : " + ServerName + " correctement ajoute a la collection " + IDCollection);
                        return true;

                    }
                }




            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                if (!(I_CollSettings == null))
                {
                    I_CollSettings.Dispose();
                }
                if (!(NewStaticRule == null))
                {
                    NewStaticRule.Dispose();
                }
                _log.Write(_className, methodName, ServerName, "End of the function " + methodName);
            }
        }

        private bool IsResourceServerObsolete(int ResourceID, WqlConnectionManager connection)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            IResultObject ListOfResources1 = null;
            bool isObsolete = false;
            try
            {
                _log.Write(_className, methodName, ResourceID.ToString(), "Starting the function " + methodName);
                _log.Write(_className, methodName, ResourceID.ToString(), "Verification de la presence de " + ResourceID + " dans SMS_FullCollectionMembership");
                //est-ce que la ressource est obsolete
                string Query1 = string.Format("select count(*) from SMS_R_System where SMS_R_System.Obsolete = '1'  and ResourceId='{0}'", ResourceID);
                ListOfResources1 = connection.QueryProcessor.ExecuteQuery(Query1);

                foreach (IResultObject Resource1 in ListOfResources1)
                {
                    if (Resource1["Count"].IntegerValue > 0)
                    {
                        _log.Write(_className, methodName, ResourceID.ToString(), "La ressource " + ResourceID + " est obsolete");
                        isObsolete = true;
                    }
                }
                _log.Write(_className, methodName, ResourceID.ToString(), "La ressource " + ResourceID + " n'est pas obsolete");
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                if (!(ListOfResources1 == null))
                {
                    ListOfResources1.Dispose();
                }
            }
            return isObsolete;
        }

        /// <summary>
        /// Indique si le serveur d'Id resourceId appartient à la collection d'id CollectionId
        /// </summary>
        /// <param name="CollectionID">Id de la collection</param>
        /// <param name="ResourceID">id de la ressource (=du serveur)</param>
        /// <param name="connection">connection WMI</param>
        /// <returns>vrai si le serveur appartient à la collection, faux sinon</returns>
        public Boolean IsCollectionMembers(string CollectionID, int ResourceID, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            IResultObject ListOfResources1 = null;
            try
            {

                _log.Write(_className, methodName, ResourceID.ToString(), "Starting the function " + methodName);
                _log.Write(_className, methodName, ResourceID.ToString(), "Verification de la presence de " + ResourceID + " dans SMS_FullCollectionMembership");
                //il faut recuperer les ResourceId non obsoletes
                string Query1 = string.Format("SELECT count(ResourceID) FROM SMS_FullCollectionMembership WHERE CollectionID = '{0}' and ResourceID='{1}' and IsObsolete=0", CollectionID, ResourceID);
                ListOfResources1 = connection.QueryProcessor.ExecuteQuery(Query1);

                foreach (IResultObject Resource1 in ListOfResources1)
                {
                    if (Resource1["Count"].IntegerValue > 0)
                    {
                        _log.Write(_className, methodName, ResourceID.ToString(), "La ressource " + ResourceID + " est presente dans SMS_FullCollectionMembership");
                        return true;
                    }
                }
                _log.Write(_className, methodName, ResourceID.ToString(), "La ressource " + ResourceID + " n'est pas presente dans SMS_FullCollectionMembership");
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                if (!(ListOfResources1 == null))
                {
                    ListOfResources1.Dispose();
                }
            }


            IResultObject ListOfResources3 = null;
            try
            {
                _log.Write(_className, methodName, ResourceID.ToString(), "Verification de la presence de " + ResourceID + " dans SMS_CM_Res_Coll_" + CollectionID);
                //il faut recuperer les ResourceId non obsoletes 
                string Query3 = string.Format("SELECT count(ResourceID) FROM SMS_CM_Res_Coll_{0} where ResourceID={1} and IsObsolete=0", CollectionID, ResourceID);
                ListOfResources3 = connection.QueryProcessor.ExecuteQuery(Query3);
                foreach (IResultObject Resource3 in ListOfResources3)
                {
                    if (Resource3["Count"].IntegerValue > 0)
                    {
                        _log.Write(_className, methodName, ResourceID.ToString(), "La ressource " + ResourceID + " est presente dans SMS_CM_Res_Coll_" + CollectionID);
                        return true;
                    }
                }
                _log.Write(_className, methodName, ResourceID.ToString(), "La ressource " + ResourceID + " n'est pas presente dans SMS_CM_Res_Coll_" + CollectionID);
                return false;
            }

            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {

                if (!(ListOfResources3 == null))
                {
                    ListOfResources3.Dispose();
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean isSecondaryServer(String serverName, WqlConnectionManager connection)
        {
            serverName = serverName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            Boolean returnCode = false;
            IResultObject sccmInstance = null;

            try
            {
                _log.Write(_className, methodName, serverName, "Starting the function " + methodName);

                smsQuery = "SELECT * FROM SMS_Site where ServerName='" + serverName + "' and Type='1'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                int nbRecord = 0;

                _log.Write(_className, methodName, serverName, "Verification du role de secondaire sur " + serverName);

                foreach (IResultObject instance in sccmInstance)
                {
                    nbRecord = nbRecord + 1;
                }
                if (nbRecord == 1)
                {
                    returnCode = true;
                    _log.Write(_className, methodName, serverName, "Role de secondaire detecte pour " + serverName);
                }
                else
                {
                    _log.Write(_className, methodName, serverName, "Aucun role de secondaire detecte pour " + serverName);
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return returnCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean isPrimaryServer(String serverName, WqlConnectionManager connection)
        {
            serverName = serverName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            Boolean returnCode = false;
            IResultObject sccmInstance = null;

            try
            {
                _log.Write(_className, methodName, serverName, "Starting the function " + methodName);
                //TODO : exclure le central : il semble que si le champ ReportingSiteCode est vide, alors c'est le central
                smsQuery = "SELECT * FROM SMS_Site where ServerName='" + serverName + "' and Type='2'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                int nbRecord = 0;

                _log.Write(_className, methodName, serverName, "Verification du role de Primaire sur " + serverName);

                foreach (IResultObject instance in sccmInstance)
                {
                    nbRecord = nbRecord + 1;
                }
                if (nbRecord == 1)
                {
                    returnCode = true;
                    _log.Write(_className, methodName, serverName, "Role de Primaire detecte pour " + serverName);
                }
                else
                {
                    _log.Write(_className, methodName, serverName, "Aucun role de Primaire detecte pour " + serverName);
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return returnCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string GetSiteInfosByServerName(String serverName, WqlConnectionManager connection)
        {
            serverName = serverName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string SiteCode = null;
            string SiteName = null;
            string ParentSiteCode = null;
            string ParentSiteName = null;
            string ParentServerName = null;
            string smsQuery;
            IResultObject sccmInstance = null;

            try
            {
                _log.Write(_className, methodName, serverName, "Starting the function " + methodName);
                _log.Write(_className, methodName, serverName, "Recuperation des infos du parent concernant le serveur " + serverName);


                smsQuery = "SELECT * FROM SMS_Site where ServerName='" + serverName + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    serverName = instance["ServerName"].StringValue;
                    SiteName = instance["SiteName"].StringValue;
                    SiteCode = instance["SiteCode"].StringValue;
                    ParentSiteCode = instance["ReportingSiteCode"].StringValue;
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }


                smsQuery = "SELECT * FROM SMS_Site where SiteCode='" + ParentSiteCode + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    ParentSiteCode = instance["SiteCode"].StringValue;
                    ParentServerName = instance["ServerName"].StringValue;
                    ParentSiteName = instance["SiteName"].StringValue;
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }


                _log.Write(_className, methodName, serverName, "Parent associe a " + serverName + " : " + ParentSiteName);

                return serverName + ";" + SiteName + ";" + SiteCode + ";" + ParentServerName + ";" + ParentSiteName + ";" + ParentSiteCode;

            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ServerName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string GetSiteNameByServerName(String ServerName, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string SiteName = null;
            string smsQuery;
            IResultObject sccmInstance = null;

            try
            {
                _log.Write(_className, methodName, ServerName, "Starting the function " + methodName);
                _log.Write(_className, methodName, ServerName, "Recuperation du nom de site du serveur " + ServerName);


                smsQuery = "SELECT * FROM SMS_Site where ServerName Like '%" + ServerName + "%'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    SiteName = instance["SiteName"].StringValue;
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }


                _log.Write(_className, methodName, ServerName, "Nom de site recupere : " + SiteName);

                return SiteName;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, ServerName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<SMS_Site> GetAllPrimarySite(WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            List<SMS_Site> ListPrimaire = new List<SMS_Site>();
            IResultObject sccmInstance = null;

            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Recuperation de tous les primaires");

                smsQuery = "SELECT * FROM SMS_Site Where Type ='2'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject2 instance in sccmInstance)
                {
                    ListPrimaire.Add(IresultWorker.SiteFromIresultObject(instance));
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                _log.Write(_className, methodName, methodName, "Liste de primaire recupere OK.");
                return ListPrimaire;

            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SiteCode"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string GetSiteInfosBySiteCode(String SiteCode, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string ServerName = null;
            string SiteName = null;
            string ParentSiteCode = null;
            string ParentSiteName = null;
            string ParentServerName = null;
            string smsQuery;
            IResultObject sccmInstance = null;

            try
            {
                _log.Write(_className, methodName, SiteCode, "Starting the function " + methodName);
                _log.Write(_className, methodName, SiteCode, "Recuperation des infos du parent concernant le site " + SiteCode);


                smsQuery = "SELECT * FROM SMS_Site where SiteCode='" + SiteCode + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    ServerName = instance["ServerName"].StringValue;
                    SiteName = instance["SiteName"].StringValue;
                    SiteCode = instance["SiteCode"].StringValue;
                    ParentSiteCode = instance["ReportingSiteCode"].StringValue;
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }


                smsQuery = "SELECT * FROM SMS_Site where SiteCode='" + ParentSiteCode + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    ParentSiteCode = instance["SiteCode"].StringValue;
                    ParentServerName = instance["ServerName"].StringValue;
                    ParentSiteName = instance["SiteName"].StringValue;
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                _log.Write(_className, methodName, SiteCode, "Parent associe a " + SiteCode + " : " + ParentSiteName);

                return ServerName + "|" + SiteName + "|" + SiteCode + "|" + ParentServerName + "|" + ParentSiteName + "|" + ParentSiteCode;

            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, SiteCode, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string ParentInfos(String serverName, WqlConnectionManager connection)
        {
            serverName = serverName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string ParentSiteCode = null;
            string ParentServerName = null;
            string smsQuery;
            string ParentInfosSite = null;
            IResultObject sccmInstance = null;

            try
            {
                _log.Write(_className, methodName, serverName, "Starting the function " + methodName);

                smsQuery = "SELECT * FROM SMS_Site where ServerName='" + serverName + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);

                _log.Write(_className, methodName, serverName, "Recuperation des infos du parent concernant le serveur " + serverName);

                foreach (IResultObject instance in sccmInstance)
                {
                    ParentSiteCode = instance["ReportingSiteCode"].StringValue;
                }

                if (!string.IsNullOrEmpty(ParentSiteCode))
                {
                    smsQuery = "SELECT * FROM SMS_Site where SiteCode='" + ParentSiteCode + "'";
                    sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                    foreach (IResultObject instance in sccmInstance)
                    {
                        ParentSiteCode = instance["SiteCode"].StringValue;
                        ParentServerName = instance["ServerName"].StringValue;
                    }
                    ParentInfosSite = ParentServerName + "|" + ParentSiteCode;
                }
                _log.Write(_className, methodName, serverName, "Parent associe a " + serverName + " : " + ParentInfosSite);
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return ParentInfosSite;

            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sitecode"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<SMS_Package> GetPackagesInfosBySite(String Sitecode, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            int NbPackages = 0;
            List<SMS_Package> returnList = new List<SMS_Package>();
            SMS_Package Package;
            IResultObject MultiQuery = null;

            try
            {
                _log.Write(_className, methodName, Sitecode, "Starting the function " + methodName);
                _log.Write(_className, methodName, Sitecode, "Recuperation des Packages du code Site : " + Sitecode);
                smsQuery = "SELECT * FROM SMS_DistributionPoint As Dp " +
                           "Join SMS_Package As Pkg On Dp.PackageID = Pkg.PackageID " +
                           "Join SMS_PackageStatus As PkgStatus On Pkg.PackageID = PkgStatus.PackageID And PkgStatus.Type = 2 " +
                            "Where Dp.SiteCode = '" + Sitecode + "'";

                MultiQuery = connection.QueryProcessor.ExecuteQuery(smsQuery);
                if (MultiQuery == null)
                {
                    _log.Write(_className, methodName, Sitecode, "Aucun package disponible pour " + Sitecode);
                    throw new Exception("WARNING : Aucun package disponible pour " + Sitecode);
                }
                else
                {
                    foreach (IResultObject sccmInstance in MultiQuery)
                    {

                        IResultObject DPresults = sccmInstance.GenericsArray[0];
                        IResultObject Pkgresults = sccmInstance.GenericsArray[1];
                        IResultObject PkgStatusresults = sccmInstance.GenericsArray[2];

                        Package = IresultWorker.PackageFromIresultObject(Pkgresults);
                        Package.Status = PkgStatusresults["Status"].IntegerValue;
                        returnList.Add(Package);
                        NbPackages += 1;
                    }
                    if (!(MultiQuery == null))
                    {
                        MultiQuery.Dispose();
                    }
                }

                _log.Write(_className, methodName, Sitecode, "Recuperation du dernier des Packages OK");

                _log.Write(_className, methodName, Sitecode, "Nombres de packages Recuperer sur le site " + Sitecode + " : " + NbPackages);

                return returnList;
            }

            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, Sitecode, "End of the function " + methodName);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<SMS_Admin> GetSmsAdmin(WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            List<SMS_Admin> returnList = new List<SMS_Admin>();
            IResultObject Instance = null;

            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Recuperation des utilisateurs admin");
                smsQuery = "SELECT * FROM SMS_Admin";

                Instance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                if (Instance == null)
                {
                    _log.Write(_className, methodName, methodName, "Aucun utilisateurs disponible");
                    throw new Exception("WARNING : Aucune application disponible pour " + methodName);
                }
                else
                {
                    foreach (IResultObject sccmInstance in Instance)
                    {
                        returnList.Add(IresultWorker.AdminFromIresultObject(sccmInstance));
                    }
                    if (!(Instance == null))
                    {
                        Instance.Dispose();
                    }
                }

                _log.Write(_className, methodName, methodName, "Recuperation des Utilisateurs OK");

                return returnList;
            }

            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sitecode"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<SMS_Application> GetApplicationsBySite(WqlConnectionManager connection, String Sitecode)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            List<SMS_Application> returnList = new List<SMS_Application>();
            IResultObject Instance = null;

            try
            {
                _log.Write(_className, methodName, Sitecode, "Starting the function " + methodName);
                _log.Write(_className, methodName, Sitecode, "Recuperation des Packages du code Site : " + Sitecode);
                smsQuery = "SELECT * FROM SMS_Application Where SourceSite='" + Sitecode + "'";

                Instance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                if (Instance == null)
                {
                    _log.Write(_className, methodName, Sitecode, "Aucune application disponible pour " + Sitecode);
                    throw new Exception("WARNING : Aucune application disponible pour " + Sitecode);
                }
                else
                {
                    foreach (IResultObject sccmInstance in Instance)
                    {
                        returnList.Add(IresultWorker.ApplicationFromIresultObject(sccmInstance));
                    }
                    if (!(Instance == null))
                    {
                        Instance.Dispose();
                    }
                }

                _log.Write(_className, methodName, Sitecode, "Recuperation des applications OK");

                _log.Write(_className, methodName, Sitecode, "Nombres de applications Recuperer sur le site " + Sitecode + " : " + returnList.Count);

                return returnList;
            }

            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, Sitecode, "End of the function " + methodName);
            }
        }


        public SMS_Application GetApplicationsByName(WqlConnectionManager connection, String ApplicationName)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            SMS_Application Return = new SMS_Application();
            IResultObject Instance = null;

            try
            {
                _log.Write(_className, methodName, ApplicationName, "Starting the function " + methodName);
                _log.Write(_className, methodName, ApplicationName, "Recuperation des Packages du code Site : " + ApplicationName);
                smsQuery = "SELECT * FROM SMS_Application Where LocalizedDisplayName Like '%" + ApplicationName + "%'";

                Instance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                if (Instance == null)
                {
                    _log.Write(_className, methodName, ApplicationName, "Aucune application disponible pour " + ApplicationName);
                    throw new Exception("WARNING : Aucune application disponible pour " + ApplicationName);
                }
                else
                {
                    foreach (IResultObject sccmInstance in Instance)
                    {
                        Return = IresultWorker.ApplicationFromIresultObject(sccmInstance);
                    }
                    if (!(Instance == null))
                    {
                        Instance.Dispose();
                    }
                }

                _log.Write(_className, methodName, ApplicationName, "Recuperation des applications OK");
                _log.Write(_className, methodName, ApplicationName, "Nombres de applications Recuperer sur le site " + ApplicationName);

                return Return;
            }

            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, ApplicationName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sitecode"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<SMS_TaskSequencePackage> GetTaskSequencesBySite(String Sitecode, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            List<SMS_TaskSequencePackage> returnList = new List<SMS_TaskSequencePackage>();
            IResultObject Instance = null;

            try
            {
                _log.Write(_className, methodName, Sitecode, "Starting the function " + methodName);
                _log.Write(_className, methodName, Sitecode, "Recuperation des sequences de taches pour le code Site : " + Sitecode);
                smsQuery = "SELECT * FROM SMS_TaskSequencePackage Where SourceSite='" + Sitecode + "'";

                Instance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                if (Instance == null)
                {
                    _log.Write(_className, methodName, Sitecode, "Aucune application disponible pour " + Sitecode);
                    throw new Exception("WARNING : Aucune application disponible pour " + Sitecode);
                }
                else
                {
                    foreach (IResultObject sccmInstance in Instance)
                    {
                        returnList.Add(IresultWorker.TaskSequencePackageFromIresultObject(sccmInstance));
                    }
                    if (!(Instance == null))
                    {
                        Instance.Dispose();
                    }
                }

                _log.Write(_className, methodName, Sitecode, "Recuperation des sequences de taches OK");
                _log.Write(_className, methodName, Sitecode, "Nombres de sequences Recuperees sur le site " + Sitecode + " : " + returnList.Count);

                return returnList;
            }

            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, Sitecode, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PackageID"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean ExistPackageID(String PackageID, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            Boolean ret = false;
            IResultObject sccmInstance = null;
            try
            {
                smsQuery = "SELECT Name,StoredPkgVersion from SMS_Package Where PackageID='" + PackageID + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    ret = true;
                }

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return ret;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SiteCode"></param>
        /// <param name="PackageID"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean ExistPackageOnDp(String SiteCode, String PackageID, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            Boolean ret = false;
            IResultObject sccmInstance = null;
            try
            {
                smsQuery = "SELECT * from SMS_DistributionPoint WHERE SiteCode='" + SiteCode + "' And PackageID='" + PackageID + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    ret = true;
                }

                return ret;
            }
            catch (Exception)
            {
                return ret;
            }
            finally
            {
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        private void RequestApplicationCreation(WqlConnectionManager connection)
        {
            WqlResultObject smsIdentification = connection.GetClassObject("sms_identification") as WqlResultObject;
            IResultObject SiteObject = smsIdentification.ExecuteMethod("GetSiteID", null);

            string id = SiteObject["SiteID"].StringValue.Replace("{", "").Replace("}", "");
            NamedObject.DefaultScope = "ScopeId_" + id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="Publisher"></param>
        /// <param name="SoftwareVersion"></param>
        /// <param name="Title"></param>
        /// <returns></returns>
        public int CreateApplication(WqlConnectionManager connection, string Publisher, string SoftwareVersion, string Title)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            int Return = 0;

            try
            {

                RequestApplicationCreation(connection);

                Microsoft.ConfigurationManagement.ApplicationManagement.Application applicationRequest =
                    new Microsoft.ConfigurationManagement.ApplicationManagement.Application();
                applicationRequest.Publisher = Publisher;
                applicationRequest.SoftwareVersion = SoftwareVersion;
                applicationRequest.Title = Title;
                applicationRequest.Version = 1;
                applicationRequest.DisplayInfo.DefaultLanguage = CultureInfo.CurrentCulture.Name;
                applicationRequest.AutoInstall = true;
                applicationRequest.Description = string.Format("Application Created by {0}", "ConfigMgr WebSevice");
                applicationRequest.ReleaseDate = DateTime.Now.ToString();

                AppDisplayInfo displayInformation = new AppDisplayInfo();
                displayInformation.Title = Title;
                displayInformation.Language = CultureInfo.CurrentCulture.Name;
                displayInformation.Publisher = Publisher;
                applicationRequest.DisplayInfo.Add(displayInformation);

                ApplicationFactory applicationFactory = new ApplicationFactory();
                AppManWrapper applicationWrapper = AppManWrapper.Create(connection, applicationFactory) as AppManWrapper;

                applicationWrapper.InnerAppManObject = applicationRequest;
                applicationFactory.PrepareResultObject(applicationWrapper);
                applicationWrapper.InnerResultObject.Put();

                return Return;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="newPackageName"></param>
        /// <param name="newPackageDescription"></param>
        /// <param name="newPackageSourceFlag"></param>
        /// <param name="newPackageSourcePath"></param>
        public void CreatePackage(WqlConnectionManager connection, string newPackageName, string newPackageDescription, int newPackageSourceFlag, string newPackageSourcePath)
        {
            try
            {
                // Create new package object.
                IResultObject newPackage = connection.CreateInstance("SMS_Package");

                // Populate new package properties.
                newPackage["Name"].StringValue = newPackageName;
                newPackage["Description"].StringValue = newPackageDescription;
                newPackage["PkgSourceFlag"].IntegerValue = newPackageSourceFlag;
                newPackage["PkgSourcePath"].StringValue = newPackageSourcePath;

                // Save new package and new package properties.
                newPackage.Put();

                // Output new package name.
                Console.WriteLine("Created package: " + newPackageName);
            }

            catch (SmsException ex)
            {
                Console.WriteLine("Failed to create package. Error: " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="serverModelName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean PushPackageOnDp(String serverName, String serverModelName, WqlConnectionManager connection)
        {
            serverName = serverName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            string smsDPquery;
            string SiteModelName;
            int NbPackages = 0;
            Boolean returnCode = false;
            IResultObject sccmInstance = null;
            IResultObject distributionPoint = null;
            IResultObject listOfResources = null;
            IResultObject NewdistributionPoint = null;
            try
            {

                _log.Write(_className, methodName, serverName, "Starting the function " + methodName);
                _log.Write(_className, methodName, serverName, "Replication Packages de  : " + serverModelName + " Vers : " + serverName);

                SiteModelName = GetSiteNameByServerName(serverModelName, connection);
                if (!string.IsNullOrEmpty(SiteModelName))
                {

                    smsQuery = "SELECT PackageID from SMS_DistributionPoint WHERE SiteName Like '%" + SiteModelName + "%'";
                    sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                    if (sccmInstance == null)
                    {
                        _log.Write(_className, methodName, serverName, "Aucun package disponible pour " + serverName);
                        throw new Exception("Aucun package disponible pour " + serverName);
                    }

                    smsDPquery = "SELECT * FROM SMS_DistributionPointInfo WHERE ServerName Like '%" + serverName + "%'";
                    listOfResources = connection.QueryProcessor.ExecuteQuery(smsDPquery);
                    if (listOfResources == null)
                    {
                        _log.Write(_className, methodName, serverName, "Le serveur " + serverName + " n'est pas un site DP SCCM");
                        throw new Exception("Le serveur " + serverName + " n'est pas un site DP SCCM");
                    }

                    foreach (IResultObject ResourceDP in listOfResources)
                    {
                        if (!string.IsNullOrEmpty(ResourceDP["NALPath"].StringValue))
                        {
                            NewdistributionPoint = ResourceDP;
                        }
                    }

                    if (NewdistributionPoint == null)
                    {
                        _log.Write(_className, methodName, serverName, "Le serveur " + serverName + " n'est pas un site DP SCCM ou n'exite pas dans la base.");
                        throw new Exception("Le serveur " + serverName + " n'est pas un site DP SCCM ou n'exite pas dans la base.");
                    }

                    foreach (IResultObject instance in sccmInstance)
                    {
                        _log.Write(_className, methodName, serverName, "Transmission des packages vers le serveur : " + serverName);
                        _log.Write(_className, methodName, serverName, "NalPah Serveur : " + NewdistributionPoint["NALPath"].StringValue);
                        _log.Write(_className, methodName, serverName, "Package a transmettre " + instance["PackageID"].StringValue);


                        distributionPoint = connection.CreateInstance("SMS_DistributionPoint");
                        distributionPoint["PackageID"].StringValue = instance["PackageID"].StringValue;
                        distributionPoint["ServerNALPath"].StringValue = NewdistributionPoint["NALPath"].StringValue;
                        distributionPoint["SiteCode"].StringValue = NewdistributionPoint["SiteCode"].StringValue;
                        distributionPoint["SiteName"].StringValue = NewdistributionPoint["SiteName"].StringValue;
                        distributionPoint["SourceSite"].StringValue = _serverCode;
                        distributionPoint.Put();
                        distributionPoint.Dispose();

                        NbPackages += 1;
                        _log.Write(_className, methodName, serverName, "Package transmis : " + instance["PackageID"].StringValue);
                    }
                    if (!(sccmInstance == null))
                    {
                        sccmInstance.Dispose();
                    }

                    _log.Write(_className, methodName, serverName, "Nombres de packages transmis sur " + serverName + " : " + NbPackages);
                    returnCode = true;

                }

                return returnCode;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="existingPackageID"></param>
        /// <param name="siteCode"></param>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public int AssignPackageToDistributionPoint(WqlConnectionManager connection, string existingPackageID, string siteCode, string serverName)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            int returnCode = 1;
            try
            {

                // Create the distribution point object (this is not an actual distribution point).
                IResultObject distributionPoint = connection.CreateInstance("SMS_DistributionPoint");

                // Associate the package with the new distribution point object. 
                distributionPoint["PackageID"].StringValue = existingPackageID;

                _log.Write(_className, methodName, siteCode, "Assigned the package: " + distributionPoint["PackageID"].StringValue + " to " + serverName);

                // This query selects a single distribution point based on the provided siteCode and serverName.
                string query = "SELECT * FROM SMS_SystemResourceList WHERE RoleName='SMS Distribution Point' AND SiteCode='" + siteCode + "' AND ServerName Like '%" + serverName + "%'";

                // 
                IResultObject listOfResources = connection.QueryProcessor.ExecuteQuery(query);
                foreach (IResultObject resource in listOfResources)
                {
                    distributionPoint["ServerNALPath"].StringValue = resource["NALPath"].StringValue;
                    distributionPoint["SiteCode"].StringValue = resource["SiteCode"].StringValue;
                    distributionPoint.Put();
                    returnCode = 0;
                }

                _log.Write(_className, methodName, siteCode, "Package: " + distributionPoint["PackageID"].StringValue + " Assign");
                return returnCode;
            }

            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of the function " + methodName);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SiteCode"></param>
        /// <param name="PackageID"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean AddPackage(String SiteCode, String PackageID, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsDPquery;
            Boolean returnCode = false;
            IResultObject distributionPoint = null;
            IResultObject listOfResources = null;
            IResultObject NewdistributionPoint = null;
            try
            {

                _log.Write(_className, methodName, SiteCode, "Starting the function " + methodName);

                smsDPquery = "SELECT * from SMS_DistributionPointInfo WHERE SiteCode='" + SiteCode + "'";
                listOfResources = connection.QueryProcessor.ExecuteQuery(smsDPquery);
                if (listOfResources == null)
                {
                    _log.Write(_className, methodName, SiteCode, "Aucun package disponible pour le site " + SiteCode);
                    throw new Exception("Aucun package disponible pour le site " + SiteCode);
                }

                foreach (IResultObject ResourceDP in listOfResources)
                {
                    if (!string.IsNullOrEmpty(ResourceDP["NALPath"].StringValue))
                    {
                        NewdistributionPoint = ResourceDP;
                    }
                }

                if (NewdistributionPoint == null)
                {
                    _log.Write(_className, methodName, SiteCode, "Le serveur " + SiteCode + " n'est pas un site DP SCCM ou n'exite pas dans la base.");
                    throw new Exception("Le serveur " + SiteCode + " n'est pas un site DP SCCM ou n'exite pas dans la base.");
                }


                _log.Write(_className, methodName, SiteCode, "Transmission du packages vers le serveur : " + NewdistributionPoint["ServerName"].StringValue);
                _log.Write(_className, methodName, SiteCode, "NalPah Serveur : " + NewdistributionPoint["NALPath"].StringValue);
                _log.Write(_className, methodName, SiteCode, "Package a transmettre " + PackageID);


                distributionPoint = connection.CreateInstance("SMS_DistributionPoint");
                distributionPoint["PackageID"].StringValue = PackageID;
                distributionPoint["ServerNALPath"].StringValue = NewdistributionPoint["NALPath"].StringValue;
                distributionPoint["SiteCode"].StringValue = NewdistributionPoint["SiteCode"].StringValue;
                distributionPoint["SiteName"].StringValue = NewdistributionPoint["SiteName"].StringValue;
                distributionPoint["SourceSite"].StringValue = _serverCode;
                distributionPoint.Put();
                distributionPoint.Dispose();

                _log.Write(_className, methodName, SiteCode, "Package transmis : " + PackageID);

                if (!(listOfResources == null))
                {
                    listOfResources.Dispose();
                }
                if (!(NewdistributionPoint == null))
                {
                    NewdistributionPoint.Dispose();
                }

                // connection.RefreshScf(SiteCode);
                returnCode = true;


                return returnCode;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, SiteCode, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ComputerName"></param>
        /// <param name="OldComputerName"></param>
        /// <param name="userName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string AddComputerAssociationForUser(string ComputerName, string PreviousComputerName, string userName, MigrationBehavior MigrationType, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string returnValue = "1";

            try
            {

                List<IResultObject> Users = new List<IResultObject>();
                List<SMS_R_System> sourceComputer = GetClientByName(connection, ComputerName);
                List<SMS_R_System> previousComputer = GetClientByName(connection, PreviousComputerName);


                if (sourceComputer.Count() > 1)
                {
                    throw new Exception("Several source computers exist with this name");
                }

                if (previousComputer.Count() > 1)
                {
                    throw new Exception("Several older computers exist with this name");
                }

                if (sourceComputer.Count() > 0 && previousComputer.Count() > 0
                    && sourceComputer[0].SMBIOSGUID != "" && previousComputer[0].SMBIOSGUID != "")
                {                    
                     
                    Dictionary<string, object> MigrationParams = new Dictionary<string, object>();
                    MigrationParams.Add("SourceClientResourceID", (UInt32)sourceComputer[0].ResourceID);
                    MigrationParams.Add("RestoreClientResourceID", (UInt32)previousComputer[0].ResourceID);
                    MigrationParams.Add("MigrationBehavior", (UInt32)MigrationType);

                    if (MigrationType == MigrationBehavior.CAPTUREANDRESTORESPECIFIED)
                    {
                        IResultObject UserInstance = connection.CreateEmbeddedObjectInstance("SMS_StateMigrationUserNames");
                        UserInstance["UserName"].StringValue = userName;
                        UserInstance["LocaleID"].IntegerValue = 0;
                        Users.Add(UserInstance);
                        MigrationParams.Add("UserNames", Users);

                    }
                                       
                    IResultObject execute = connection.ExecuteMethod("SMS_StateMigration", "AddAssociationEx", MigrationParams);
                    if (execute["ReturnValue"].IntegerValue == 0)
                    {
                        returnValue = "0";
                    }

                }
                else {
                    throw new Exception("Newer or older computer does not exist.");
                }

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, ComputerName, "End of the function " + methodName);
            }

            return returnValue;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean RefreshAllPackage(String serverName, WqlConnectionManager connection)
        {
            serverName = serverName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string SiteCode;
            Boolean returnCode = false;
            IResultObject distributionPoint = null;

            try
            {

                _log.Write(_className, methodName, serverName, "Starting the function " + methodName);
                _log.Write(_className, methodName, serverName, "Rafraichissement des packages sur le serveur " + serverName);

                SiteCode = GetSiteCodeByServerName(serverName, connection);
                if (!string.IsNullOrEmpty(SiteCode))
                {

                    ReinitAllPackageStatus(SiteCode, connection);

                    distributionPoint = connection.QueryProcessor.ExecuteQuery("Select * From SMS_DistributionPoint WHERE SiteCode='" + SiteCode + "'");
                    foreach (IResultObject Instance in distributionPoint)
                    {
                        Instance["RefreshNow"].BooleanValue = true;
                        Instance.Put();
                        _log.Write(_className, methodName, serverName, "Rafraichissement du package " + Instance["PackageID"].StringValue + " sur le serveur " + serverName + " OK");
                    }
                    if (!(distributionPoint == null))
                    {
                        distributionPoint.Dispose();
                    }


                    returnCode = true;
                }
                else
                {
                    _log.Write(_className, methodName, serverName, "Rafraichissement des packages sur le serveur " + serverName + " KO");
                }

                return returnCode;
            }

            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SiteCode"></param>
        /// <param name="PackageID"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean RefreshPackage(String SiteCode, String PackageID, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            Boolean returnCode = false;
            IResultObject distributionPoint = null;

            try
            {
                _log.Write(_className, methodName, SiteCode, "Starting the function " + methodName);
                distributionPoint = connection.QueryProcessor.ExecuteQuery("Select * From SMS_DistributionPoint WHERE SiteCode='" + SiteCode + "' And PackageID='" + PackageID + "'");
                foreach (IResultObject Instance in distributionPoint)
                {
                    Instance["RefreshNow"].BooleanValue = true;
                    Instance.Put();
                }
                if (!(distributionPoint == null))
                {
                    distributionPoint.Dispose();
                }

                // connection.RefreshScf(SiteCode);
                returnCode = true;


                return returnCode;
            }

            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, SiteCode, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SiteCode"></param>
        /// <param name="DistributionPointName"></param>
        /// <param name="PackageID"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean RemovePackage(string SiteCode, string DistributionPointName, string PackageID, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            Boolean returnCode = false;
            IResultObject distributionPoint = null;

            try
            {

                _log.Write(_className, methodName, SiteCode, "Starting the function " + methodName);
                _log.Write(_className, methodName, SiteCode, "Suppression du package " + PackageID + " sur le serveur " + SiteCode);

                if (!string.IsNullOrEmpty(SiteCode))
                {

                    distributionPoint = connection.QueryProcessor.ExecuteQuery("Select * From SMS_DistributionPoint WHERE SiteCode='" + SiteCode + "' AND ServerNALPath Like '%" + DistributionPointName + "%' And PackageID='" + PackageID + "'");
                    foreach (IResultObject Instance in distributionPoint)
                    {
                        Instance.Delete();
                    }
                    if (!(distributionPoint == null))
                    {
                        distributionPoint.Dispose();
                    }

                    //  connection.RefreshScf(SiteCode);
                    returnCode = true;
                    _log.Write(_className, methodName, SiteCode, "Suppression du package " + PackageID + " sur le serveur " + SiteCode + " OK");
                }
                else
                {
                    _log.Write(_className, methodName, SiteCode, "Suppression du package " + PackageID + " sur le serveur " + SiteCode + " KO");
                }

                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, SiteCode, "End of the function " + methodName);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="SiteCode"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean ReinitAllPackageStatus(String SiteCode, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            Boolean returnCode = false;
            IResultObject distributionPoint = null;

            try
            {

                _log.Write(_className, methodName, SiteCode, "Starting the function " + methodName);

                if (!string.IsNullOrEmpty(SiteCode))
                {

                    distributionPoint = connection.QueryProcessor.ExecuteQuery("Select * From SMS_DistributionPoint WHERE SiteCode='" + SiteCode + "' And Status = '1'");
                    foreach (IResultObject Instance in distributionPoint)
                    {
                        Instance["Status"].IntegerValue = 0;
                        Instance.Put();
                        _log.Write(_className, methodName, SiteCode, "Reinitialisation du package " + Instance["PackageID"].StringValue + " sur le Site " + SiteCode + " OK");
                    }
                    if (!(distributionPoint == null))
                    {
                        distributionPoint.Dispose();
                    }
                    returnCode = true;

                }
                else
                {
                    _log.Write(_className, methodName, SiteCode, "Reinitialisation des Packages sur le Site " + SiteCode + " KO");
                }

                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, SiteCode, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="SiteCode"></param>
        /// <param name="PackageID"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean ReinitPackageStatus(String SiteCode, String PackageID, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            Boolean returnCode = false;
            IResultObject distributionPoint = null;

            try
            {

                _log.Write(_className, methodName, SiteCode, "Starting the function " + methodName);

                if (!string.IsNullOrEmpty(SiteCode))
                {

                    distributionPoint = connection.QueryProcessor.ExecuteQuery("Select * From SMS_DistributionPoint WHERE SiteCode='" + SiteCode + "' And PackageID='" + PackageID + "' And Status = '1'");
                    foreach (IResultObject Instance in distributionPoint)
                    {
                        Instance["Status"].IntegerValue = 0;
                        Instance.Put();
                    }

                    returnCode = true;
                    _log.Write(_className, methodName, SiteCode, "Reinitialisation du package " + PackageID + " sur le serveur " + SiteCode + " OK");
                }
                else
                {
                    _log.Write(_className, methodName, SiteCode, "Reinitialisation du package " + PackageID + " sur le serveur " + SiteCode + " KO");
                }
                if (!(distributionPoint == null))
                {
                    distributionPoint.Dispose();
                }

                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, SiteCode, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public String GetSiteCodeByServerName(String serverName, WqlConnectionManager connection)
        {
            serverName = serverName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            string siteCode = null;
            IResultObject sccmInstance = null;

            try
            {
                _log.Write(_className, methodName, serverName, "Starting the function " + methodName);
                smsQuery = "SELECT * FROM SMS_Site where ServerName Like '%" + serverName + "%'";
                _log.Write(_className, methodName, serverName, "Recuperation du code site associe au serveur  " + serverName);
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    siteCode = instance["SiteCode"].StringValue;
                    _log.Write(_className, methodName, serverName, "Code site associe a  " + serverName + " : " + siteCode);
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return siteCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return siteCode;
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string GetSiteCodeByDistributionPointName(WqlConnectionManager connection, string DistributionPointName)
        {
            DistributionPointName = DistributionPointName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            string siteCode = null;
            IResultObject sccmInstance = null;

            try
            {
                _log.Write(_className, methodName, DistributionPointName, "Starting the function " + methodName);
                smsQuery = "SELECT * FROM SMS_DistributionPointInfo where ServerName Like '%" + DistributionPointName + "%'";
                _log.Write(_className, methodName, DistributionPointName, "Recuperation du code site associe au serveur  " + DistributionPointName);
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    siteCode = instance["SiteCode"].StringValue;
                    _log.Write(_className, methodName, DistributionPointName, "Code site assign to  " + DistributionPointName + " : " + siteCode);
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return siteCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return siteCode;
            }
            finally
            {
                _log.Write(_className, methodName, DistributionPointName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public String ParentSiteCode(WqlConnectionManager connection, String serverName)
        {
            serverName = serverName.ToUpper();
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            string reportingSiteCode = null;
            IResultObject sccmInstance = null;
            try
            {
                _log.Write(_className, methodName, serverName, "Starting the function " + methodName);

                smsQuery = "SELECT * FROM SMS_Site where ServerName='" + serverName + "'";
                _log.Write(_className, methodName, serverName, "Recuperation du code site de reporting associe au serveur  " + serverName);
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    if (instance["ServerName"].StringValue == serverName)
                    {
                        reportingSiteCode = instance["ReportingSiteCode"].StringValue;
                        _log.Write(_className, methodName, serverName, "Code site de Reporting associe a  " + serverName + " : " + reportingSiteCode);
                    }
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                if (reportingSiteCode == null)
                {
                    throw new Exception("Impossible de recuperer le code site parent");
                }
                return reportingSiteCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return reportingSiteCode;
            }
            finally
            {
                _log.Write(_className, methodName, serverName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="parentSiteCode"></param>
        /// <returns></returns>
        public Boolean RemoveSecondarySiteSystem(String siteCode, String parentSiteCode)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            string stringToSearch = "Removed site " + siteCode + " from the database";
            string _Installationpath;
            Boolean returnCode = false;
            try
            {

                _Installationpath = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\SMS\\Identification", "Installation Directory", "Nothing").ToString();

                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);
                _log.Write(_className, methodName, siteCode, "Suppression du Site  " + siteCode + " Associe au Parent : " + parentSiteCode);
                process.StartInfo.FileName = _Installationpath + "\\bin\\X64\\00000409\\preinst.exe";
                process.StartInfo.Arguments = "/RemoveSITE " + siteCode + " " + parentSiteCode;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                returnCode = output.Contains(stringToSearch);
                _log.Write(_className, methodName, siteCode, "Retour de la commande de suppression : " + returnCode);

                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        public Boolean RemoveSecondarySiteJobs(String siteCode)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            string stringToSearch = "jobs that are targeted to site " + siteCode;
            string _Installationpath;
            Boolean returnCode = false;
            try
            {
                _Installationpath = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\SMS\\Identification", "Installation Directory", "Nothing").ToString();

                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);
                _log.Write(_className, methodName, siteCode, "Suppression des Jobs en cours sur " + siteCode);
                process.StartInfo.FileName = _Installationpath + "\\bin\\X64\\00000409\\preinst.exe";
                process.StartInfo.Arguments = "/RemoveJOB " + siteCode;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                returnCode = output.Contains(stringToSearch);
                _log.Write(_className, methodName, siteCode, "Retour de la commande de suppression : " + returnCode);

                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        public Boolean RemoveRequestSchedule(String siteCode)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            System.Diagnostics.Process ListSchedule = new System.Diagnostics.Process();
            Boolean returnCode = false;
            String DirSchedule = null;
            String SReqForSite = null;
            List<string> FileList = new List<string>();

            try
            {
                DirSchedule = "F:\\Produits\\ConfigMgr\\inboxes\\schedule.box";
                SReqForSite = "SReqForSite.exe";

                if (!File.Exists(DirSchedule + "\\" + SReqForSite))
                {
                    throw new Exception("Impossible de supprimer les enregistrements planifie car SReqForSite est absent du repertoire schedule.box");
                }

                _log.Write(_className, methodName, siteCode, "Recuperation des Schedule Request en cours pour le code site " + siteCode);

                ListSchedule.StartInfo.Arguments = "-s " + siteCode + " -l";
                ListSchedule.StartInfo.FileName = DirSchedule + "\\" + SReqForSite;
                ListSchedule.StartInfo.WorkingDirectory = DirSchedule;
                ListSchedule.StartInfo.RedirectStandardOutput = true;
                ListSchedule.StartInfo.UseShellExecute = false;
                ListSchedule.StartInfo.CreateNoWindow = true;
                ListSchedule.Start();

                while (!ListSchedule.HasExited)
                {
                    while (!ListSchedule.StandardOutput.EndOfStream)
                    {
                        string Line = ListSchedule.StandardOutput.ReadLine();
                        if (!String.IsNullOrEmpty(Line) && Line.Contains("Send request found"))
                        {
                            FileList.Add(DirSchedule + Line.Replace("Send request found : .", null));
                        }
                    }
                }
                _log.Write(_className, methodName, siteCode, "Recuperation des Schedule Request en cours pour le code site " + siteCode + " OK");


                _log.Write(_className, methodName, siteCode, "Suppression de " + FileList.Count + " Requests");
                foreach (string Rfile in FileList)
                {
                    _log.Write(_className, methodName, siteCode, "Suppression du Fichier Request " + Rfile);
                    File.SetAttributes(Rfile, FileAttributes.Normal);
                    File.Delete(Rfile);
                    _log.Write(_className, methodName, siteCode, "Suppression du Fichier Request " + Rfile + " OK");
                }

                returnCode = true;
                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sitecode"></param>
        /// <returns></returns>
        public Boolean CleanPkcFile(String sitecode)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string dirName = "F:\\Produits\\ConfigMgr\\inboxes\\hman.box\\pubkey";
            string fileName = dirName + "\\" + sitecode + ".pkc";
            FileInfo pkcFile = new FileInfo(fileName);
            Boolean returnCode;

            try
            {

                _log.Write(_className, methodName, sitecode, "Starting the function " + methodName);

                if (!pkcFile.Exists)
                {
                    _log.Write(_className, methodName, sitecode, "la clef " + sitecode + ".PKC n'existe pas");
                    returnCode = true;
                }
                else
                {
                    _log.Write(_className, methodName, sitecode, "la clef " + sitecode + ".PKC existe et va etre Supprimee");
                    pkcFile.Delete();
                    returnCode = true;
                    _log.Write(_className, methodName, sitecode, "la clef " + sitecode + ".PKC Supprimee");
                }

                return returnCode;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, sitecode, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="AssignmentName"></param>
        /// <param name="ApplicationName"></param>
        /// <param name="CollectionID"></param>
        /// <returns></returns>
        public int ApplicationAssignment(WqlConnectionManager connection, string AssignmentName, string ApplicationName, string CollectionID)
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, ApplicationName, "Starting the function " + methodName);
                SMS_Application App = GetApplicationsByName(connection, ApplicationName);
                SMS_Collection Collection = GetCollectionByID(connection, CollectionID);
                IResultObject newApplicationAssingment = connection.CreateInstance("SMS_ApplicationAssignment");

                newApplicationAssingment["ApplicationName"].StringValue = App.LocalizedDisplayName;
                newApplicationAssingment["AssignmentName"].StringValue = AssignmentName;
                newApplicationAssingment["AssignedCIs"].IntegerArrayValue = new int[] { App.CI_ID };
                newApplicationAssingment["CollectionName"].StringValue = Collection.Name;
                newApplicationAssingment["CreationTime"].DateTimeValue = DateTime.Now;
                newApplicationAssingment["LocaleID"].IntegerValue = 1043;
                newApplicationAssingment["SourceSite"].StringValue = _serverCode;
                newApplicationAssingment["StartTime"].DateTimeValue = DateTime.Now;
                newApplicationAssingment["SuppressReboot"].IntegerValue = 1;
                newApplicationAssingment["NotifyUser"].BooleanValue = true;
                newApplicationAssingment["TargetCollectionID"].StringValue = CollectionID;
                newApplicationAssingment["OfferTypeID"].IntegerValue = 2;
                newApplicationAssingment["WoLEnabled"].BooleanValue = false;
                newApplicationAssingment["RebootOutsideOfServiceWindows"].BooleanValue = false;
                newApplicationAssingment["OverrideServiceWindows"].BooleanValue = false;
                newApplicationAssingment["UseGMTTimes"].BooleanValue = true;
                newApplicationAssingment.Put();

                return 0;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return 1;
            }
            finally
            {
                _log.Write(_className, methodName, ApplicationName, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        public Boolean CleanActiveDirectoryEntries(String siteCode)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);
                DirectoryEntry de;
                DirectorySearcher ds;
                SearchResultCollection sr;
                System.Net.NetworkInformation.IPGlobalProperties IPproperties;
                IPproperties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

                _log.Write(_className, methodName, siteCode, "Connexion LDAP : LDAP://" + IPproperties.DomainName + ":389");
                de = new DirectoryEntry("LDAP://" + IPproperties.DomainName + ":389");
                ds = new DirectorySearcher(de);

                if (string.IsNullOrEmpty(siteCode))
                {
                    return false;
                }
                ds.Filter = "(mSSMSSiteCode=" + siteCode + ")";
                ds.PropertiesToLoad.Add("distinguishedName");
                _log.Write(_className, methodName, siteCode, "Recherche de l'entree : (mSSMSSiteCode=" + siteCode + ")");
                sr = ds.FindAll();

                foreach (SearchResult SiteClean in sr)
                {
                    _log.Write(_className, methodName, siteCode, "Entree (mSSMSSiteCode=" + siteCode + ") trouve");
                    DirectoryEntry SiteToRemove = SiteClean.GetDirectoryEntry();
                    SiteToRemove.DeleteTree();
                    SiteToRemove.CommitChanges();
                    _log.Write(_className, methodName, siteCode, "Suppression Entree (mSSMSSiteCode=" + siteCode + ") OK");
                }

                return true;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="ServerName"></param>
        /// <param name="boundaryNames"></param>
        /// <param name="detailBoundaries"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean AddSiteBoundaries(String siteCode, String ServerName, String[] boundaryNames, String[] detailBoundaries, WqlConnectionManager connection)
        {


            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            Boolean returnCode = true;
            IResultObject boundary = null;
            int i_Bdy;
            String detailBoundary = null;

            try
            {

                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);

                if (!ExistSiteCode(siteCode, connection))
                {
                    _log.Write(_className, methodName, siteCode, "Site Code " + siteCode + " non trouve. A voir.");
                    _log.Write(_className, methodName, siteCode, "Ajout des limites de site " + siteCode + "=> KO");
                    throw new Exception("WARNING : Ajout des limites de site " + siteCode + "=> KO");
                }
                boundary = connection.CreateInstance("SMS_SCI_RoamingBoundary");
                boundary["Details"].StringArrayValue = detailBoundaries;
                boundary["DisplayNames"].StringArrayValue = boundaryNames;

                Int32[] flags = new Int32[detailBoundaries.Length];
                string[] types = new string[detailBoundaries.Length];
                for (int i_Enr = 0; i_Enr < detailBoundaries.Length; i_Enr++)
                {
                    flags[i_Enr] = 0;
                    types[i_Enr] = "IP Ranges";
                }

                boundary["Flags"].IntegerArrayValue = flags;
                boundary["Types"].StringArrayValue = types;
                boundary["siteCode"].StringValue = siteCode;

                boundary.Put();

                if (!(boundary == null))
                {
                    boundary.Dispose();
                }

                for (i_Bdy = 0; i_Bdy < detailBoundaries.Length; i_Bdy++)
                {
                    detailBoundary = detailBoundaries[i_Bdy];
                    if (ExistSiteBoundaryDetail(siteCode, detailBoundary, connection))
                    {
                        _log.Write(_className, methodName, siteCode, "Ajout de la limite de site " + detailBoundary + " => OK");
                    }
                    else
                    {
                        _log.Write(_className, methodName, siteCode, "Ajout de la limite de site " + detailBoundary + " => KO");
                        returnCode = false;
                    }
                }

                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="ServerName"></param>
        /// <param name="boundaryNames"></param>
        /// <param name="detailBoundaries"></param>
        /// <param name="ProtectedMode"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean UpdateSiteBoundaries(String siteCode, String ServerName, String boundaryNames, string detailBoundaries, Boolean ProtectedMode, WqlConnectionManager connection)
        {


            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string[] ListdetailBoundaries = null;
            Boolean returnCode = true;
            int Nbrecord = 0;
            try
            {

                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);
                if (!ExistSiteCode(siteCode, connection))
                {
                    throw new Exception("WARNING : Impossible de mettre a jours les boundarie d'un serveur qui n existe pas");
                }

                IResultObject oRoamingBoundary = connection.GetInstance("SMS_SCI_RoamingBoundary.ItemName='Roaming Boundary',ItemType='Roaming Boundary',SiteCode='" + siteCode + "'");

                foreach (IResultObject oResult in oRoamingBoundary)
                {
                    foreach (string DisplayName in oResult["DisplayNames"].StringArrayValue)
                    {

                        if (DisplayName == boundaryNames)
                        {
                            ListdetailBoundaries = oResult["Details"].StringArrayValue;
                            ListdetailBoundaries[Nbrecord] = detailBoundaries;
                            oResult["Details"].StringArrayValue = ListdetailBoundaries;
                            oResult.Put();
                            _log.Write(_className, methodName, siteCode, "Mise a jours de la boundarie " + DisplayName + " OK");
                        }
                        Nbrecord += 1;
                    }
                }
                if (!(oRoamingBoundary == null))
                {
                    oRoamingBoundary.Dispose();
                }



                if (ProtectedMode == true)
                {

                    string[] ipRanges;
                    ipRanges = new string[ListdetailBoundaries.Length * 2];
                    int ipCpt = 0;
                    foreach (string bound in ListdetailBoundaries)
                    {
                        ipRanges[ipCpt] = "IP Ranges";
                        ipCpt = ipCpt + 1;
                        ipRanges[ipCpt] = bound;
                        ipCpt = ipCpt + 1;
                    }

                    _log.Write(_className, methodName, siteCode, "Activation du mode protege");
                    _log.Write(_className, methodName, siteCode, "Nombre de boundaries associe " + ipRanges.Length);
                    IResultObject smsRole = connection.GetInstance("SMS_SCI_SysResUse.FileType=2,ItemName='[\"Display=\\\\" + ServerName + "\\\"]MSWNET:[\"SMS_SITE=" + siteCode + "\"]\\\\" + ServerName + "\\,SMS Site System',ItemType='System Resource Usage',SiteCode='" + siteCode + "'");
                    smsRole = WriteScfEmbeddedProperty(smsRole, "IsProtected", 1, "", "");
                    smsRole = WriteScfEmbeddedPropertyList(smsRole, "Protected Boundary", ipRanges);
                    smsRole.Put();

                    if (!(smsRole == null))
                    {
                        smsRole.Dispose();
                    }

                    _log.Write(_className, methodName, siteCode, "Activation du mode protege OK");
                }


                //  connection.CommitSiteControlFileUpdates(siteCode);

                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.StackTrace.ToString());
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean RemoveSiteBoundaries(String siteCode, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            Boolean returnCode = false;

            string[] aNewDetails = new string[0];
            string[] aNewTypes = new string[0];
            string[] aNewDisplayNames = new string[0];
            int[] aNewFlags = new int[0];

            try
            {
                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);

                if (!ExistSiteCode(siteCode, connection))
                {
                    return false;
                }


                if (ExistSiteBoundary(siteCode, connection))
                {
                    _log.Write(_className, methodName, siteCode, "Suppression des Boundarys associees au code site " + siteCode);

                    IResultObject oRoamingBoundary = connection.GetInstance("SMS_SCI_RoamingBoundary.FileType=2,ItemName='Roaming Boundary',ItemType='Roaming Boundary',SiteCode='" + siteCode + "'");
                    foreach (IResultObject oResult in oRoamingBoundary)
                    {
                        oResult["Details"].StringArrayValue = aNewDetails;
                        oResult["Types"].StringArrayValue = aNewTypes;
                        oResult["DisplayNames"].StringArrayValue = aNewDisplayNames;
                        oResult["Flags"].IntegerArrayValue = aNewFlags;

                        _log.Write(_className, methodName, siteCode, "Suppression des Boundarys associees au code site " + siteCode + " Effectuee");
                    }
                    oRoamingBoundary.Put();

                    if (!(oRoamingBoundary == null))
                    {
                        oRoamingBoundary.Dispose();
                    }

                    // connection.CommitSiteControlFileUpdates(siteCode);

                    returnCode = true;

                }
                else
                {
                    _log.Write(_className, methodName, siteCode, "Aucune Boundary existante concernant le code site " + siteCode);
                    returnCode = true;

                }

                return returnCode;
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean ExistSiteCode(String siteCode, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string smsQuery;
            Boolean returnCode = false;
            IResultObject sccmInstance = null;
            try
            {
                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);

                smsQuery = "SELECT * FROM SMS_Site";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    if (instance["SiteCode"].StringValue == siteCode)
                    {
                        returnCode = true;
                    }
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                if (returnCode == true)
                {
                    _log.Write(_className, methodName, siteCode, "Site Code " + siteCode + " Trouve");
                }
                else
                {
                    _log.Write(_className, methodName, siteCode, "Site Code " + siteCode + " Non Trouve");
                }

                return returnCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : Le site code renseigne est introuvable.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ParentsiteCode"></param>
        /// <param name="destSiteCode"></param>
        /// <param name="destServerName"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean AddSenderAddress(String ParentsiteCode, String destSiteCode, String destServerName, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            Boolean returnCode = false;
            IResultObject senderAddress = null;
            String AddressType = "MS_LAN";
            String ItemName = destSiteCode + "|" + AddressType;
            String ItemType = "Address";
            int Order = 1;
            Boolean UnlimitedRateForAll = true;

            try
            {

                _log.Write(_className, methodName, ParentsiteCode, "Starting the function " + methodName);
                if (!ExistSiteCode(ParentsiteCode, connection))
                {
                    _log.Write(_className, methodName, ParentsiteCode, "Site Code " + ParentsiteCode + " non trouve");
                    _log.Write(_className, methodName, ParentsiteCode, "Ajout Sender Address " + destSiteCode + "=> KO");
                    throw new Exception("WARNING : Ajout Sender Address " + destSiteCode + "=> KO");
                }

                _log.Write(_className, methodName, ParentsiteCode, "Creation du SenderAddress du site " + ParentsiteCode + " en destination de " + destSiteCode);
                senderAddress = connection.CreateInstance("SMS_SCI_Address");
                senderAddress["AddressType"].StringValue = AddressType;
                senderAddress["DesSiteCode"].StringValue = destSiteCode;
                senderAddress["ItemName"].StringValue = ItemName;
                senderAddress["ItemType"].StringValue = ItemType;
                senderAddress["Order"].IntegerValue = Order;
                senderAddress["SiteCode"].StringValue = ParentsiteCode;
                senderAddress["UnlimitedRateForAll"].BooleanValue = UnlimitedRateForAll;

                senderAddress = WriteScfEmbeddedProperty(senderAddress, "Connection Point", 0, destServerName, "SMS_SITE");
                senderAddress = WriteScfEmbeddedProperty(senderAddress, "LAN Login", 0, "", "");
                senderAddress.Put();
                if (!(senderAddress == null))
                {
                    senderAddress.Dispose();
                }

                returnCode = ExistSenderAddress(ParentsiteCode, destSiteCode, connection);
                if (returnCode)
                {
                    _log.Write(_className, methodName, ParentsiteCode, "Creation SenderAddress vers " + destSiteCode + " effectue");
                }
                else
                {
                    _log.Write(_className, methodName, ParentsiteCode, "Creation SenderAddress vers " + destSiteCode + "  impossible");
                }

                return returnCode;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, ParentsiteCode, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="destSiteCode"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean ExistSenderAddress(String siteCode, String destSiteCode, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            Boolean returnCode = false;
            IResultObject sccmInstance = null;
            try
            {
                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);
                _log.Write(_className, methodName, siteCode, "Recherche du SenderAdress " + siteCode + " Site de destination " + destSiteCode);
                string smsQuery;
                smsQuery = "SELECT * FROM SMS_SCI_Address where siteCode='" + siteCode + "' and DesSiteCode='" + destSiteCode + "'";
                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                int nbRecord = 0;
                foreach (IResultObject instance in sccmInstance)
                {
                    nbRecord = nbRecord + 1;
                }

                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                if (nbRecord > 0)
                {
                    _log.Write(_className, methodName, siteCode, "SenderAdress " + siteCode + " Trouve");
                    returnCode = true;
                }
                return returnCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : Le sender adresse n existe pas.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smsObject"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public IResultObject WriteScfEmbeddedProperty(IResultObject smsObject,
                                                            String propertyName,
                                                            int value,
                                                            String value1,
                                                            String value2)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;

            try
            {

                _log.Write(_className, methodName, propertyName, "Starting the function " + methodName);
                _log.Write(_className, methodName, propertyName, "Creation de la proprietee " + propertyName);
                Dictionary<string, IResultObject> EmbeddedProperties;
                EmbeddedProperties = smsObject.EmbeddedProperties;
                IResultObject instance;
                if (EmbeddedProperties.ContainsKey(propertyName))
                {
                    instance = EmbeddedProperties[propertyName];
                }
                else
                {
                    ConnectionManagerBase connection = smsObject.ConnectionManager;
                    instance = connection.CreateEmbeddedObjectInstance("SMS_EmbeddedProperty");
                    EmbeddedProperties.Add(propertyName, instance);
                }

                instance["PropertyName"].StringValue = propertyName;
                instance["Value"].IntegerValue = value;
                instance["Value1"].StringValue = value1;
                instance["Value2"].StringValue = value2;

                smsObject.EmbeddedProperties = EmbeddedProperties;
                _log.Write(_className, methodName, propertyName, "Creation de la proprietee " + propertyName + " OK");

                return smsObject;
            }
            catch (SmsException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, propertyName, "End of the function " + methodName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smsObject"></param>
        /// <param name="propertyListName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public IResultObject WriteScfEmbeddedPropertyList(IResultObject smsObject,
                                                                 String propertyListName,
                                                                 String[] values)
        {


            // Recuperation du nom de la methode courante                        //
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            try
            {

                _log.Write(_className, methodName, propertyListName, "Starting the function " + methodName);
                _log.Write(_className, methodName, propertyListName, "Creation de la proprietee " + propertyListName);
                // Initialisation de l objet propriete list                      //
                Dictionary<string, IResultObject> EmbeddedPropertyList;
                EmbeddedPropertyList = smsObject.EmbeddedPropertyLists;

                // Recuperation ou creation de l instance propertylist          //
                IResultObject instance;
                if (EmbeddedPropertyList.ContainsKey(propertyListName))
                {
                    instance = EmbeddedPropertyList[propertyListName];
                }
                else
                {
                    ConnectionManagerBase connection = smsObject.ConnectionManager;
                    instance = connection.CreateEmbeddedObjectInstance("SMS_EmbeddedPropertyList");
                    EmbeddedPropertyList.Add(propertyListName, instance);
                }

                // Set the property list properties.
                instance["PropertyListName"].StringValue = propertyListName;
                instance["Values"].StringArrayValue = values;
                smsObject.EmbeddedPropertyLists = EmbeddedPropertyList;

                _log.Write(_className, methodName, propertyListName, "Creation de la proprietee " + propertyListName + " OK");

                return smsObject;
            }
            catch (SmsException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, propertyListName, "End of the function " + methodName);
            }
        } // Fin methode WriteScfEmbeddedPropertyList                            //

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="detailBoundary"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean ExistSiteBoundaryDetail(String siteCode, String detailBoundary, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            Boolean returnCode = false;
            string smsQuery;
            string boundary;
            try
            {
                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);
                _log.Write(_className, methodName, siteCode, "Recheche de la limite de site " + detailBoundary);
                smsQuery = "SELECT * FROM SMS_SCI_RoamingBoundary WHERE SiteCode='" + siteCode + "'";
                IResultObject sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    boundary = string.Join(",", instance["Details"].StringArrayValue);
                    if (boundary.Contains(detailBoundary))
                    {
                        _log.Write(_className, methodName, siteCode, "La limite de site " + detailBoundary + " Existe");
                        returnCode = true;
                    }
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                return returnCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : Impossible de recuperer les details de la boundary.");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public Boolean ExistSiteBoundary(String siteCode, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            int CountBoundary = 0;
            Boolean returnCode = false;
            string smsQuery;
            string boundary;
            try
            {
                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);
                smsQuery = "SELECT * FROM SMS_SCI_RoamingBoundary WHERE SiteCode='" + siteCode + "'";
                IResultObject sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject instance in sccmInstance)
                {
                    boundary = string.Join(",", instance["Details"].StringArrayValue);
                    if (boundary.Contains("-"))
                    {
                        _log.Write(_className, methodName, siteCode, "La limite de site " + boundary + " Existe");
                        returnCode = true;
                        CountBoundary += 1;
                    }
                }
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }

                if (CountBoundary == 0)
                {
                    _log.Write(_className, methodName, siteCode, "Aucune limite de site trouve pour le site " + siteCode);
                }
                return returnCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : La boundary n existe pas.");
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                return false;
            }
            finally
            {
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <summary> 
        /// 
        /// </summary> 
        public List<SMS_Boundary> GetSiteBoundaries(string siteCode, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            List<SMS_Boundary> Boundarieslist = new List<SMS_Boundary>();
            string smsQuery;
            IResultObject sccmInstance = null;
            try
            {
                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);
                if (string.IsNullOrEmpty(siteCode))
                {
                    _log.Write(_className, methodName, methodName, "Recherche de toutes les limites de sites");
                    smsQuery = "SELECT * FROM SMS_Boundary";
                }
                else
                {
                    _log.Write(_className, methodName, siteCode, "Recherche de la limite de site " + siteCode);
                    smsQuery = "SELECT * FROM SMS_Boundary WHERE SiteCode='" + siteCode + "'";
                }

                sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                foreach (IResultObject Boundary in sccmInstance)
                {
                    Boundarieslist.Add(IresultWorker.BoundaryFromIresultObject(Boundary));
                }
                return Boundarieslist;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                if (!(sccmInstance == null))
                {
                    sccmInstance.Dispose();
                }
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <summary> 
        /// 
        /// </summary> 
        public SMS_DistributionPointList GetAllDPGroups(WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            SMS_DistributionPointList DPGroupslist = new SMS_DistributionPointList();
            string smsQueryDP = null;
            try
            {
                _log.Write(_className, methodName, methodName, "Starting the function " + methodName);
                _log.Write(_className, methodName, methodName, "Recheche de tous les groupes de DP");

                smsQueryDP = "SELECT * FROM SMS_DistributionPointInfo As DP Join " +
                                "SMS_DPGroupMembers As DPGM on DP.NALPath=DPGM.DPNALPath " +
                                "Join SMS_DistributionPointGroup As DPG on DPGM.GroupID=DPG.GroupID";
                IResultObject sccmInstanceDP = connection.QueryProcessor.ExecuteQuery(smsQueryDP);
                foreach (IResultObject MultiQuery in sccmInstanceDP)
                {

                    IResultObject DPresults = MultiQuery.GenericsArray[0];
                    IResultObject DPGresults = MultiQuery.GenericsArray[1];
                    IResultObject DPGMresults = MultiQuery.GenericsArray[2];

                    SMS_DistributionPoint DPGroupinfo = new SMS_DistributionPoint();
                    DPGroupinfo.NALPath = DPresults["NALPath"].StringValue;
                    DPGroupinfo.SiteName = DPresults["SiteName"].StringValue;
                    DPGroupinfo.ServerName = DPresults["ServerName"].StringValue;
                    DPGroupinfo.SiteCode = DPresults["Sitecode"].StringValue;
                    DPGroupinfo.Name = DPGresults["Name"].StringValue;
                    DPGroupinfo.GroupID = DPGMresults["GroupID"].StringValue;
                    DPGroupslist.SMS_DistributionPoints.Add(DPGroupinfo);

                }

                if (!(sccmInstanceDP == null))
                {
                    sccmInstanceDP.Dispose();
                }

                return DPGroupslist;

            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, methodName, "End of the function " + methodName);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="PriSiteCode"></param>
        /// <param name="siteCode"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <summary> 
        /// 
        /// </summary> 
        public Boolean RemoveSenderAddress(String PriSiteCode, String siteCode, WqlConnectionManager connection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            IResultObject sccmInstance = null;
            Boolean returnCode = false;
            string methodName = sf.GetMethod().Name;

            try
            {
                _log.Write(_className, methodName, siteCode, "Starting the function " + methodName);

                if (PriSiteCode == null)
                {
                    _log.Write(_className, methodName, siteCode, "Impossible de trouver le site parent pour " + siteCode);
                    return false;
                }

                if (ExistSenderAddress(siteCode, PriSiteCode, connection))
                {

                    _log.Write(_className, methodName, siteCode, "Recherche du SenderAdress " + siteCode);
                    string smsQuery;
                    smsQuery = "SELECT * FROM SMS_SCI_Address where siteCode='" + PriSiteCode + "' and DesSiteCode='" + siteCode + "'";
                    sccmInstance = connection.QueryProcessor.ExecuteQuery(smsQuery);
                    int nbRecord = 0;
                    foreach (IResultObject instance in sccmInstance)
                    {
                        nbRecord = nbRecord + 1;
                        instance.Delete();
                    }
                    if (!(sccmInstance == null))
                    {
                        sccmInstance.Dispose();
                    }

                    _log.Write(_className, methodName, siteCode, "Nombre de sender adress trouve associe au site " + siteCode + " : " + nbRecord);
                    _log.Write(_className, methodName, siteCode, "SenderAdress " + siteCode + " Trouve et supprime");
                    returnCode = true;
                }
                else
                {
                    returnCode = true;
                }

                //  connection.CommitSiteControlFileUpdates(siteCode);

                return returnCode;
            }
            catch (SmsQueryException ex)
            {
                _log.Write(_className, methodName, "Exception", "An error has occurred : ");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                _log.Write(_className, methodName, "Exception", ex.StackTrace);
                throw new Exception("WARNING : " + ex.Message);
            }
            finally
            {
                _log.Write(_className, methodName, siteCode, "End of the function " + methodName);
            }
        }

    }
}
