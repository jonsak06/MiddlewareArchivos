using MiddlewareArchivosService.Enums;
using MiddlewareArchivosService.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MiddlewareArchivosService.Providers
{
    internal class XMLProvider
    {
        private string ConfigPath = string.Empty;
        private ConfigMapper mapper;

        public XMLProvider()
        {
            mapper = new ConfigMapper();
            string basePath = System.Configuration.ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(EnumCarpetas.Path)];
            string carpetaConfig = System.Configuration.ConfigurationManager.AppSettings[mapper.GetKeyCarpeta(Enums.EnumCarpetas.CarpetaConfig)];
            ConfigPath = $"{basePath}{carpetaConfig}\\";
        }

        public XDocument GetDocument(EnumArchivosXML xmlKey)
        {
            string uri = $"C:\\Pasantia\\CONFIG\\{System.Configuration.ConfigurationManager.AppSettings[mapper.GetKeyXML(xmlKey)]}";

            return XDocument.Load(uri);
        }
    }
}
