using MiddlewareArchivosService.Enums;
using MiddlewareArchivosService.Mappers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareArchivosService.Controllers
{
    internal class ArchivosXMLController
    {
        private string PathArchivoEmpresas, PathArchivoUrls, PathArchivoInterfaces, PathArchivoUsuarios;
        public ArchivosXMLController()
        {
            ConfigMapper mapper = new ConfigMapper();
            CarpetasController carpetasController = CarpetasController.Instance;
            string pathConfig = carpetasController.PathCarpetaConfig;

            this.PathArchivoEmpresas = $"{pathConfig}{System.Configuration.ConfigurationManager.AppSettings[mapper.GetKeyXML(EnumArchivosXML.Empresas)]}";
            this.PathArchivoUrls = $"{pathConfig}{System.Configuration.ConfigurationManager.AppSettings[mapper.GetKeyXML(EnumArchivosXML.Urls)]}";
            this.PathArchivoInterfaces = $"{pathConfig}{System.Configuration.ConfigurationManager.AppSettings[mapper.GetKeyXML(EnumArchivosXML.Interfaces)]}";
            this.PathArchivoUsuarios = $"{pathConfig}{System.Configuration.ConfigurationManager.AppSettings[mapper.GetKeyXML(EnumArchivosXML.Usuarios)]}";
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
