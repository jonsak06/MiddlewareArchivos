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
            return documentoInterfaces.Descendants("Interfaz").Where(e => e.Element("Nombre").Value == api).Elements().Where(e => e.Name == "Post").FirstOrDefault().Value;
        }
        public string getEndpointGet(int codigoInterfaz)
        {
            return documentoInterfaces.Descendants("Interfaz").Where(e => e.Element("Codigo").Value == codigoInterfaz.ToString()).Elements().Where(e => e.Name == "Get").FirstOrDefault().Value;
        }
        public string getEndpointEjecucionesPendientes()
        {
            return documentoInterfaces.Descendants("Interfaz").Where(e => e.Element("Nombre").Value == "Salida").Elements().Where(e => e.Name == "GetEjecuciones").FirstOrDefault().Value;
        }
        public string getEndpointConsultarEstado()
        {
            return documentoInterfaces.Descendants("Interfaz").Where(e => e.Element("Nombre").Value == "Salida").Elements().Where(e => e.Name == "ConsultarEstado").FirstOrDefault().Value;
        }

    }
}
