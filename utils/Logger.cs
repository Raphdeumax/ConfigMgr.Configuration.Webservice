//*******************************************************************************//
//  Developpement :  Raphael DELPLANQUE                                          //
//*******************************************************************************//
using System;
using System.Configuration;
using System.IO;
using System.Threading;
namespace ConfigMgr.Configuration.Webservice.utils
{
    //****************************************************************************//
    //  Cette classe definit Un logger proprietaire.                              //
    //  PARAM:     Aucun.                                                         //
    //  Cette classe retourne :                                                   //
    //             un objet de type LogMsgClass auquel on peut appliquer les      //
    //             methodes suivantes :                                           //
    //                  Logger() -> Constructeur                                  //
    //                  SetPrefixe()                                              //
    //                  Write()                                                   //
    //****************************************************************************//
    /// <summary>
    /// Cette classe definit Un logger proprietaire.
    /// PARAM:     Aucun.
    /// Cette class retourne :
    ///            un objet de type Logger auquel on peut appliquer les
    ///            methodes suivantes :
    ///                  Logger() -> Constructeur
    ///                  SetPrefixe()
    ///                  Write()
    /// </summary>
    public class Logger
    {
        //************************************************************************//
        // Initialisation des variables de classe                                 //
        //************************************************************************//
        private string _logName = ConfigurationManager.AppSettings["LogFileName"];
        private string _hostName = System.Environment.MachineName;
        // Variable Verrou d'acces en ecriture pour gestion des acces concurrents //          
        private static object _fileLock = new object();
        static int NbBoucle = 0;
        //************************************************************************//
        //  Constructeur de Logger                                                //
        //************************************************************************//
        /// <summary>
        /// Constructeur de Logger 
        /// </summary>
        public Logger()
        {
        } // Fin Constructeur Logger                                              //
        //************************************************************************//
        //  Cette methode construit le prefixe des messages de la log d execution.//
        //  PARAM:     Nom de la classe appelante.                                //
        //  Cette methode retourne:                                               //
        //             Le prefixe d une entree dans la log d execution.           //
        //************************************************************************//
        /// <summary>
        /// Cette methode construit le prefixe des messages de la log d execution.
        /// </summary>
        /// <param name="className">Nom de la classe appelante.</param>
        /// <returns>Le prefixe d une entree dans la log d execution.</returns>
        private string SetPrefixe(string className)
        {
            string prefixe;
            Calendrier calendar = new Calendrier();
            string dateHeure = calendar.GetDateTime();
            prefixe = dateHeure + " " + _hostName + " " + className;
            return prefixe;
        } // Fin methode SetPrefixe                                               //
        //************************************************************************//
        //  Cette methode ecrit un message dans la log d execution.               //
        //  PARAM:     Nom de la classe,                                          //
        //             Nom de la methode,                                         //
        //             Identifiant de l evenement,                                //
        //             Texte.                                                     //
        //  Cette methode retourne:                                               //
        //             Aucun Code Retour.                                         //
        //************************************************************************//
        /// <summary>
        /// Cette methode ecrit un message dans la log d execution. 
        /// </summary>
        /// <param name="className">Nom de la classe</param>
        /// <param name="methodName">Nom de la methode</param>
        /// <param name="identifiant">Identifiant de l evenement</param>
        /// <param name="text">Texte</param>
        public void Write(string className,
                          string methodName,
                          string identifiant,
                          string text)
        {
            lock (_fileLock)
            {
                System.IO.StreamWriter sw = null;
                
                string msgPrefixe = SetPrefixe(className);
                string msg;
                string TraceLog;
                TraceLog = "<![LOG[" + text + "]LOG]!><time=\"" + DateTime.Now.ToString("HH:mm:ss.fff") + "-60\" date=\"" + DateTime.Now.ToString("MM-dd-yyyy") + "\" component=\"" + methodName + "\" context=\"\" type=\"0\" thread=\""+ Thread.CurrentThread.ManagedThreadId +"\" file=\"" + className + ".cs\">";

                msg = TraceLog;
                try
                {
                    if(!File.Exists(_logName))
                    {
                        Directory.CreateDirectory(new FileInfo(_logName).Directory.FullName);
                    }

                    sw = new System.IO.StreamWriter(_logName, true, System.Text.ASCIIEncoding.Default);
                    sw.WriteLine(msg);
                    sw.Close();
                    sw.Dispose();

                    FileInfo filelog = new FileInfo(_logName);
                    long SizeInBytes = filelog.Length;
                    long SizeMaxInBytes = 4 * 1024 * 1024;
                    if (SizeInBytes > SizeMaxInBytes)
                    {
                        if (ConfigurationManager.AppSettings["LogDebug"] == "on")
                        {
                            File.Move(_logName, _logName.Replace(System.IO.Path.GetFileNameWithoutExtension(_logName), System.IO.Path.GetFileNameWithoutExtension(_logName) + "_" + DateTime.Now.ToString("hh_mm_MM_dd_yyyy")));
                        }
                        else
                        {
                            File.Delete(_logName);
                        }
                    }

                }
                catch (Exception)
                {
                    NbBoucle += 1;
                    if (NbBoucle < 10)
                        Write(className, methodName, identifiant, text);
                    else
                        throw;
                }
                finally
                {
                    sw = null;
                }
            } // Fin lock _fileLock                                               //
        } // Fin methode Write                                                    //
    } // Fin class Logger                                                         //
    //****************************************************************************//
    //  Cette class definit les methodes relatives aux manipulations des dates.   //
    //  PARAM:     Aucun.                                                         //
    //  Cette class retourne :                                                    //
    //             un objet de type DateClass auquel on peut appliquer les        //
    //             methodes suivantes :                                           //
    //                  Calendrier() -> Constructeur                              //
    //                  GetDateTime()                                             //
    //****************************************************************************//
    /// <summary>
    /// Cette classe definit les methodes relatives aux manipulations des dates.
    /// PARAM:     Aucun.
    /// Cette class retourne :
    ///             un objet de type DateClass auquel on peut appliquer les    
    ///             methodes suivantes : 
    ///                  Calendrier() -> Constructeur 
    ///                  GetDateTime()
    /// </summary>
    public class Calendrier
    {
        //************************************************************************//
        // Initialisation des variables de classe                                 //
        //************************************************************************//
        private DateTime _dateCourante = DateTime.Now;
        //private string _className = "DateClass";
        //public Logger _log = new Logger();
        //************************************************************************//
        //  Constructeur de Calendrier                                            //
        //************************************************************************//
        /// <summary>
        /// Constructeur de Calendrier
        /// </summary>
        public Calendrier()
        {
        } // Fin Constructeur Calendrier                                          //
        //************************************************************************//
        //  Cette methode retourne la date et l heure normalisees.                //
        //  PARAM:     Aucun.                                                     //
        //  Cette methode retourne:                                               //
        //             La date au format DD/MM/YYYY HH:MM:SS.                     //
        //************************************************************************//
        /// <summary>
        /// Cette methode retourne la date et l heure courantes normalisees.
        /// </summary>
        /// <returns>La date au format DD/MM/YYYY HH:MM:SS.</returns>
        public string GetDateTime()
        {
            string dateHeure = _dateCourante.ToString();
            return dateHeure;
        } // Fin methode GetDateTime                                               //

    } // Fin class Calendrier                                                      //


    //****************************************************************************//
    //  Cette classe definit les methodes relatives aux manipulations des         //
    //  fichiers.                                                                 //
    //  PARAM:     Aucun.                                                         //
    //  Cette class retourne :                                                    //
    //             un objet de type FileClass auquel on peut appliquer les        //
    //             methodes suivantes :                                           //
    //                  FileClass() -> Constructeur                               //
    //                  CheckFile()                                               //
    //****************************************************************************//
    /// <summary>
    /// Cette class definit les methodes relatives aux manipulations des fichiers.
    /// PARAM:     Aucun.                                                         
    /// Cette class retourne :                                                    
    ///             un objet de type FileClass auquel on peut appliquer les       
    ///             methodes suivantes :                                          
    ///                  FileClass() -> Constructeur                                                                     
    ///                  CheckFile()                                    
    /// </summary>
    public class FileClass
    {
        //************************************************************************//
        // Initialisation des variables de classe                                 //
        //************************************************************************//
        private string _className = "FileClass";

        /// <summary>
        /// 
        /// </summary>
        public Logger _log = new Logger();
        //************************************************************************//
        //  Constructeur de FileClass                                             //
        //************************************************************************//
        /// <summary>
        /// Constructeur de FileClass
        /// </summary>
        public FileClass()
        {
        } // Fin Constructeur FileClass                                           //
        //************************************************************************//
        //  Cette methode verifie l existence d un fichier dont le nom est fourni //
        //  en arguments.                                                         //
        //  PARAM:     un nom de fichier.                                         //
        //  Cette methode retourne :                                              //
        //             OK si le fichier existe,                                   //
        //             KO sinon.                                                  //
        //************************************************************************//
        /// <summary>
        /// Cette methode verifie l existence d un fichier dont le nom est fourni
        /// en arguments.
        /// </summary>
        /// <param name="fileName">un nom de fichier.</param>
        /// <returns>OK si le fichier existe,
        /// KO sinon.</returns>
        public string CheckFile(string fileName)
        {
            System.Diagnostics.StackFrame sf;
            sf = new System.Diagnostics.StackFrame();
            string methodName = sf.GetMethod().Name;
            string returnCode;
            if (!System.IO.File.Exists(fileName))
            {

                returnCode = "OK";
                _log.Write(_className, methodName, "File", "Fichier Log Cree");

            }
            else
            {
                returnCode = "OK";
            }
            return returnCode;
        } // Fin methode CheckFile                                                    //
    } // Fin class Fileclass                                                          //

} // Fin namespace ConfigMgr.Configuration.Webservice.utils                                 //