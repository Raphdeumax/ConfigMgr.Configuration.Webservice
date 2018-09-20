//*******************************************************************************//
//  Developpement :  Raphael DELPLANQUE                                          //
//*******************************************************************************//

using System;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using System.Configuration;
using Microsoft.Win32;
using Microsoft.ConfigurationManagement.ManagementProvider;


namespace ConfigMgr.Configuration.Webservice.utils
{
    /**
     * cette classe permet de gerer les connexion WMI SMS
     * 
     * 
     * 
     * 
     * */
    /// <summary> 
    /// 
    /// </summary> 
    public class Connection
    {
        private string _className = "Connection";
        /// <summary> 
        /// 
        /// </summary> 
        public Logger _log = new Logger();

        /// <summary> 
        /// 
        /// </summary> 
        public String _sccmUser;

        /// <summary> 
        /// 
        /// </summary> 
        public String _sccmUserDomain;
        /// <summary> 
        /// 
        /// </summary> 
        public String _sccmUserPassword;
        /// <summary> 
        /// 
        /// </summary> 
        public String _serverName;
        /// <summary> 
        /// 
        /// </summary> 
        public String _serverCode;
        /// <summary> 
        /// 
        /// </summary> 
        public Boolean _integratedAuthent;

        /// <summary> 
        /// 
        /// </summary> 
        public Connection()
        {
            _sccmUser = ConfigurationManager.AppSettings["SccmUser"];
            _sccmUserDomain = ConfigurationManager.AppSettings["SccmUserDomain"];
            _sccmUserPassword = ConfigurationManager.AppSettings["SccmUserPassword"];

            _serverName = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\SMS\\Identification", "Server", "Nothing").ToString();
            _serverCode = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\SMS\\Identification", "Site Code", "Nothing").ToString();

            if (_sccmUser == "")
            {
                _integratedAuthent = true;
            }
            else
            {
                _integratedAuthent = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public WqlConnectionManager Connect()
        {

            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string userName;
            WqlConnectionManager connection = new WqlConnectionManager();
            SmsNamedValuesDictionary namedValues = new SmsNamedValuesDictionary();
            try
            {
                _log.Write(_className, methodName, _serverCode, "Starting the function " + methodName);
                _log.Write(_className, methodName, _serverCode, "WQL connection in progress : " + _serverName + " | Site code : " + _serverCode);
                connection = new WqlConnectionManager(namedValues);
                if (!string.IsNullOrEmpty(_sccmUserDomain) & !string.IsNullOrEmpty(_sccmUser) & !string.IsNullOrEmpty(_sccmUserPassword))
                {                    
                    userName = _sccmUserDomain + "\\" + _sccmUser;
                    _log.Write(_className, methodName, _serverCode, "Associated user : " + userName);
                    connection.Connect(_serverName, userName, _sccmUserPassword);
                }
                else
                {
                    connection.Connect(_serverName);
                }

                _log.Write(_className, methodName, _serverCode, "WQL connection on " + _serverName + " OK");

                return connection;
            }
            catch (SmsException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : General connection failed");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                connection.Close();
                connection.Dispose();
                throw new Exception("WARNING : " + ex.Message.ToString());
            }
            catch (UnauthorizedAccessException ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : Authentication failed");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                connection.Close();
                connection.Dispose();
                throw new Exception("WARNING : " + ex.Message.ToString());
            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : DCOM Connection failed");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                connection.Close();
                connection.Dispose();
                throw new Exception("WARNING : " + ex.Message.ToString());
            }
            finally
            {
                _log.Write(_className, methodName, _serverCode, "End of the function " + methodName);
            }
        }


        /// <summary> 
        /// 
        /// </summary> 
        public Boolean Disconnect(WqlConnectionManager wqlConnection)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            try
            {
                _log.Write(_className, methodName, _serverCode, "Starting the function " + methodName);
                _log.Write(_className, methodName, _serverCode, "Closing WQL connection " + _serverName);
                wqlConnection.Close();
                wqlConnection.Dispose();
                _log.Write(_className, methodName, _serverCode, "WQL Connection Opening " + _serverName + " OK");
                return true;

            }
            catch (Exception ex)
            {
                _log.Write(_className, methodName, "Exception", "WARNING : Error closing WQL connection");
                _log.Write(_className, methodName, "Exception", ex.Message.ToString());
                throw new Exception("WARNING : " + ex.Message.ToString());
            }
            finally
            {
                _log.Write(_className, methodName, _serverCode, "End of the function " + methodName);
            }
        }

    }
}