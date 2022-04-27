using MiddlewareArchivos.Enums;
using MiddlewareArchivos.Mappers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MiddlewareArchivos.Providers
{
    internal class XMLProvider
    {
        private string ConfigPath = string.Empty;
        private ConfigMapper mapper;

        public XMLProvider()
        {
            mapper = new ConfigMapper();
            string basePath = ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(EnumCarpetas.Path)];
            string carpetaConfig = ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(Enums.EnumCarpetas.CarpetaConfig)];
            ConfigPath = $"{basePath}{carpetaConfig}\\";
        }

        public XDocument GetDocument(EnumArchivosXML xmlKey)
        {
            string uri = $"C:\\Pasantia\\CONFIG\\{ConfigurationManager.AppSettings[mapper.GetKeyXML(xmlKey)]}";

            return XDocument.Load(uri);
        }
    }
}
