using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;

namespace ConfigMgr.Configuration.Webservice.utils.LDAP
{
    /// <summary>
    /// 
    /// </summary>
    public class LDAPAccess
    {
        /// <summary>
        /// 
        /// </summary>
        public class LDAP_Computer
        {
            public string Name;
            public List<LDAP_ComputerProperties> Properties;
        }

        /// <summary>
        /// 
        /// </summary>
        public class LDAP_ComputerProperties
        {
            public string Name;
            public string Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Domain"></param>
        /// <param name="HostName"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public List<LDAP_Computer> GetComputerDetailsLDAP(string Domain, string HostName, string UserName, string Password)
        {
            try
            {

                List<LDAP_Computer> ADComputers = new List<LDAP_Computer>();
                DirectoryEntry Entry;
                Entry = new DirectoryEntry("LDAP://" + Domain + ":389", UserName, Password, AuthenticationTypes.Secure);

                DirectorySearcher Search = new DirectorySearcher(Entry);

                Search.Filter = string.Format("(&(objectCategory=computer)(cn=*{0}*))", HostName);

                SearchResultCollection Result = Search.FindAll();

                if (Result == null)
                {
                    return null;
                }
                else
                {

                    foreach (SearchResult AdResult in Result)
                    {
                        LDAP_Computer Computer = new LDAP_Computer();
                        List<LDAP_ComputerProperties> ComputerProperties = new List<LDAP_ComputerProperties>();
                        Computer.Name = AdResult.Properties["cn"][0].ToString();

                        foreach (string PropertyNames in AdResult.Properties.PropertyNames)
                        {
                            ComputerProperties.Add(new LDAP_ComputerProperties() { Name = PropertyNames, Value = AdResult.Properties[PropertyNames][0].ToString() });
                        };

                        Computer.Properties = ComputerProperties;
                        ADComputers.Add(Computer);

                    }

                    return ADComputers;
                }
            }
            catch (Exception Ex)
            {
                return null;
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
        public bool RemoveComputerFromLDAP(string Domain, string HostName, string UserName, string Password)
        {
            try
            {
                DirectoryEntry Entry;
                DirectoryEntry entryToRemove;

                Entry = new DirectoryEntry("LDAP://" + Domain, UserName,Password);
                DirectorySearcher Search = new DirectorySearcher(Entry);
                Search.Filter = string.Format("(&(objectCategory=computer)(cn={0}))", HostName);
                SearchResult Result = Search.FindOne();

                entryToRemove = new DirectoryEntry(Result.Path, UserName, Password);
                entryToRemove.DeleteTree();
                entryToRemove.CommitChanges();
                return true;

            }
            catch (Exception Ex)
            {
                return false;
            }
        }
    }
}