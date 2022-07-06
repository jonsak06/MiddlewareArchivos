using System.Configuration;

string Path = $"{ConfigurationManager.AppSettings["Path"]}";

string PathCarpetaIn = $"{Path}{ConfigurationManager.AppSettings["CarpetaIN"]}\\";
string PathCarpetaOut = $"{Path}{ConfigurationManager.AppSettings["CarpetaOUT"]}\\";
string PathCarpetaConfig = $"{Path}{ConfigurationManager.AppSettings["CarpetaCONFIG"]}\\";
string PathCarpetaCtrl = $"{Path}{ConfigurationManager.AppSettings["CarpetaCTRL"]}\\";

string PathCarpetaInPendiente = $"{PathCarpetaIn}{ConfigurationManager.AppSettings["CarpetaPendienteIN"]}\\";
string PathCarpetaInEnProceso = $"{PathCarpetaIn}{ConfigurationManager.AppSettings["CarpetaEnProcesoIN"]}\\";
string PathCarpetaInProcesado = $"{PathCarpetaIn}{ConfigurationManager.AppSettings["CarpetaProcesadoIN"]}\\";
string PathCarpetaInNoProcesado = $"{PathCarpetaIn}{ConfigurationManager.AppSettings["CarpetaNoProcesadoIN"]}\\";
string PathCarpetaInLog = $"{PathCarpetaIn}{ConfigurationManager.AppSettings["CarpetaLogIN"]}\\";

string PathCarpetaOutEnProceso = $"{PathCarpetaOut}{ConfigurationManager.AppSettings["CarpetaEnProcesoOUT"]}\\";
string PathCarpetaOutPendiente = $"{PathCarpetaOut}{ConfigurationManager.AppSettings["CarpetaPendienteOUT"]}\\";
string PathCarpetaOutBackup = $"{PathCarpetaOut}{ConfigurationManager.AppSettings["CarpetaBackupOUT"]}\\";
string PathCarpetaOutLog = $"{PathCarpetaOut}{ConfigurationManager.AppSettings["CarpetaLogOUT"]}\\";

List<string> carpetasPrincipales = new List<string> { PathCarpetaIn, PathCarpetaOut, PathCarpetaConfig, PathCarpetaCtrl };
List<string> subCarpetas = new List<string> { PathCarpetaInPendiente, PathCarpetaInEnProceso, PathCarpetaInProcesado, PathCarpetaInNoProcesado, PathCarpetaInLog,
                PathCarpetaOutEnProceso, PathCarpetaOutPendiente, PathCarpetaOutBackup, PathCarpetaOutLog };


foreach (string carpeta in carpetasPrincipales)
{
    if (!Directory.Exists(carpeta))
    {
        Directory.CreateDirectory(carpeta);
    }
}

foreach (string subCarpeta in subCarpetas)
{
    if (!Directory.Exists(subCarpeta))
    {
        Directory.CreateDirectory(subCarpeta);
    }
}

Console.WriteLine("Proceso finalizado");