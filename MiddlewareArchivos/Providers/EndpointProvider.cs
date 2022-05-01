using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MiddlewareArchivos.Providers
{
    internal class EndpointProvider
    {
        private XMLProvider provider;
        private XDocument documentoUrls;
        private XDocument documentoInterfaces;
        public EndpointProvider()
        {
            provider = new XMLProvider();
            documentoUrls = provider.GetDocument(Enums.EnumArchivosXML.Urls);
            documentoInterfaces = provider.GetDocument(Enums.EnumArchivosXML.Interfaces);
        }

        public string getApiGatewayUrl()
        {
            return documentoUrls.Descendants("Url").Where(e => e.Element("Destino").Value == "ApiGateway").Elements().Where(e => e.Name == "Direccion").FirstOrDefault().Value;
        }

        public string getEndpointPost(string api)
        {
            return getEndpoint("Post", api);
        }
        public string getEndpointGet(string api)
        {
            return getEndpoint("Get", api);
        }

        private string getEndpoint(string method, string api)
        {
            return documentoInterfaces.Descendants("Interfaz").Where(e => e.Element("Nombre").Value == api).Elements().Where(e => e.Name == method).FirstOrDefault().Value;

        }
    }
}
