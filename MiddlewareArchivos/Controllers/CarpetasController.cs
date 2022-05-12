using MiddlewareArchivos.Enums;
using MiddlewareArchivos.Mappers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareArchivos.Controllers
{
    internal sealed class CarpetasController
    {
        private static CarpetasController instance = new CarpetasController();
        private ConfigMapper mapper;
        private string Path;
        public readonly string PathCarpetaIn, PathCarpetaOut, PathCarpetaConfig, PathCarpetaCtrl, PathCarpetaInPendiente, PathCarpetaInEnProceso,
            PathCarpetaInProcesado, PathCarpetaInNoProcesado, PathCarpetaInLog, PathCarpetaOutPendiente, PathCarpetaOutEnProceso, PathCarpetaOutBackup, PathCarpetaOutLog;
        static CarpetasController() { }
        public static CarpetasController Instance
        {
            get { return instance; }
        }
        private CarpetasController()
        {
            mapper = new ConfigMapper();
            Path = ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(EnumCarpetas.Path)];

            PathCarpetaIn = $"{Path}{ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(EnumCarpetas.CarpetaIn)]}\\";
            PathCarpetaOut = $"{Path}{ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(EnumCarpetas.CarpetaOut)]}\\";
            PathCarpetaConfig = $"{Path}{ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(EnumCarpetas.CarpetaConfig)]}\\";
            PathCarpetaCtrl = $"{Path}{ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(EnumCarpetas.CarpetaCtrl)]}\\";

            PathCarpetaInPendiente = $"{PathCarpetaIn}{ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(EnumCarpetas.CarpetaPendienteIn)]}\\";
            PathCarpetaInEnProceso = $"{PathCarpetaIn}{ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(EnumCarpetas.CarpetaEnProcesoIn)]}\\";
            PathCarpetaInProcesado = $"{PathCarpetaIn}{ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(EnumCarpetas.CarpetaProcesadoIn)]}\\";
            PathCarpetaInNoProcesado = $"{PathCarpetaIn}{ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(EnumCarpetas.CarpetaNoProcesadoIn)]}\\";
            PathCarpetaInLog = $"{PathCarpetaIn}{ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(EnumCarpetas.CarpetaLogIn)]}\\";

            PathCarpetaOutPendiente = $"{PathCarpetaOut}{ConfigurationManager.AppSettings.Get(mapper.GetKeyCarpeta(EnumCarpetas.CarpetaPendienteOut))}\\";
            PathCarpetaOutEnProceso = $"{PathCarpetaOut}{ConfigurationManager.AppSettings.Get(mapper.GetKeyCarpeta(EnumCarpetas.CarpetaEnProcesoOut))}\\";
            PathCarpetaOutBackup = $"{PathCarpetaOut}{ConfigurationManager.AppSettings.Get(mapper.GetKeyCarpeta(EnumCarpetas.CarpetaBackupOut))}\\";
            PathCarpetaOutLog = $"{PathCarpetaOut}{ConfigurationManager.AppSettings.Get(mapper.GetKeyCarpeta(EnumCarpetas.CarpetaLogOut))}\\";
        }
        public void crearCarpetas()
        {
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
        }

        public bool existenCarpetas()
        {
            List<string> carpetasPrincipales = new List<string> { PathCarpetaIn, PathCarpetaOut, PathCarpetaConfig, PathCarpetaCtrl };
            List<string> subCarpetas = new List<string> { PathCarpetaInPendiente, PathCarpetaInEnProceso, PathCarpetaInProcesado, PathCarpetaInNoProcesado, PathCarpetaInLog,
                PathCarpetaOutEnProceso, PathCarpetaOutPendiente, PathCarpetaOutBackup, PathCarpetaOutLog };

            foreach (string carpeta in carpetasPrincipales)
            {
                if (!Directory.Exists(carpeta))
                {
                    return false;
                }
            }

            foreach (string subCarpeta in subCarpetas)
            {
                if (!Directory.Exists(subCarpeta))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
