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
    internal class ArchivosXMLController
    {
        private string PathArchivoEmpresas, PathArchivoUrls, PathArchivoInterfaces, PathArchivoUsuarios;
        public ArchivosXMLController()
        {
            ConfigMapper mapper = new ConfigMapper();
            CarpetasController carpetasController = CarpetasController.Instance;
            string pathConfig = carpetasController.PathCarpetaConfig;

            this.PathArchivoEmpresas = $"{pathConfig}{ConfigurationManager.AppSettings[mapper.GetKeyXML(EnumArchivosXML.Empresas)]}";
            this.PathArchivoUrls = $"{pathConfig}{ConfigurationManager.AppSettings[mapper.GetKeyXML(EnumArchivosXML.Urls)]}";
            this.PathArchivoInterfaces = $"{pathConfig}{ConfigurationManager.AppSettings[mapper.GetKeyXML(EnumArchivosXML.Interfaces)]}";
            this.PathArchivoUsuarios = $"{pathConfig}{ConfigurationManager.AppSettings[mapper.GetKeyXML(EnumArchivosXML.Usuarios)]}";
        }
        public bool existenArchivosXml()
        {
            List<string> pathArchivos = new List<string> { PathArchivoEmpresas, PathArchivoUrls, PathArchivoInterfaces, PathArchivoUsuarios };

            foreach(string archivo in pathArchivos)
            {
                if (!File.Exists(archivo))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
