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
    internal class ArchivosXmlController
    {
        private string NombreArchivoEmpresas, NombreArchivoUrls, NombreArchivoInterfaces, NombreArchivoUsuarios;
        private string PathArchivoEmpresas, PathArchivoUrls, PathArchivoInterfaces, PathArchivoUsuarios;
        public ArchivosXmlController()
        {
            ConfigMapper mapper = new ConfigMapper();
            CarpetasController carpetasController = CarpetasController.Instance;
            string pathConfig = carpetasController.PathCarpetaConfig;


            this.NombreArchivoEmpresas = ConfigurationManager.AppSettings[mapper.GetKeyXML(EnumArchivosXML.Empresas)];
            this.NombreArchivoUrls = ConfigurationManager.AppSettings[mapper.GetKeyXML(EnumArchivosXML.Urls)];
            this.NombreArchivoInterfaces = ConfigurationManager.AppSettings[mapper.GetKeyXML(EnumArchivosXML.Interfaces)];
            this.NombreArchivoUsuarios = ConfigurationManager.AppSettings[mapper.GetKeyXML(EnumArchivosXML.Usuarios)];

            this.PathArchivoEmpresas = $"{pathConfig}{NombreArchivoEmpresas}";
            this.PathArchivoUrls = $"{pathConfig}{NombreArchivoUrls}";
            this.PathArchivoInterfaces = $"{pathConfig}{NombreArchivoInterfaces}";
            this.PathArchivoUsuarios = $"{pathConfig}{NombreArchivoUsuarios}";
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
