//*******************************************************************************//
//  Developpement :  Raphael DELPLANQUE                                          //
//*******************************************************************************//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ConfigMgr.Configuration.Webservice.utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class GlobalUtils
    {
        /// <summary>
        /// Interprete la chaine retournee par une web methode du web service SCCM
        /// </summary>
        /// <param name="RetourWS">Chaine de retour de la web methode à interpreter</param>
        /// <returns>Liste des chaines composant le retour de la Web methode du Web service</returns>
        /// <remarks></remarks>
        public static List<string> InterpreteRetourWS(string RetourWS)
        {    //Les separateur peuvent être | ou ;
            //Si OK, il se peut que RetourWS ne commence pas par un nombre. Dans ce cas, retourner 0 en 1er element de la liste
            List<string> strInfosList = new List<string>();

            if (!string.IsNullOrEmpty(RetourWS))
            {
                List<string> temptab = new List<string>();


                if (RetourWS.Contains("|"))
                {
                    temptab.AddRange(RetourWS.Split(Convert.ToChar("|")));
                }
                else if (RetourWS.Contains(";"))
                {
                    temptab.AddRange(RetourWS.Split(Convert.ToChar(";")));
                }
                else
                {
                    temptab.Add(RetourWS);
                }

                string strResult = temptab[0];

                //Le 1er de la liste doit être numerique. S'il n'est pas numerique, inserer 0
                int intres = 0;
                bool isInt = int.TryParse(strResult, out intres);
                if (!isInt)
                    strInfosList.Add("0");

                strInfosList.AddRange(temptab);
            }
            else
            {
                throw new Exception("The web method does not return a normal value");
            }

            return strInfosList;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum TargetTypes
    {
        /// <summary>
        /// 
        /// </summary>
        None=0,
        /// <summary>
        /// 
        /// </summary>
        SMS = 2
    }
}