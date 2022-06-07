using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MiddlewareArchivos.Providers
{
    internal class Authenticator
    {
        public static async Task<string> GetTokenAsync()
        {
            var provider = new XMLProvider();
            var documentoUrls = provider.GetDocument(Enums.EnumArchivosXML.Urls);
            var documentoUsuarios = provider.GetDocument(Enums.EnumArchivosXML.Usuarios);

            var tokenUrl = documentoUrls.Descendants("Url").Where(e => e.Element("Destino").Value == "Connection").Elements().Where(e => e.Name == "Direccion").FirstOrDefault().Value;
            var clientId = documentoUsuarios.Descendants("Usuario").Elements().Where(e => e.Name == "ClientId").FirstOrDefault().Value;
            var clientSecret = documentoUsuarios.Descendants("Usuario").Elements().Where(e => e.Name == "ClientSecret").FirstOrDefault().Value;

            using (var client = new HttpClient())
            {
                var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = tokenUrl,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Scope = "api"
                });

                if (response.IsError)
                {
                    throw new Exception($"An error ocurred while retrieving an access token: {response.Error}");
                }

                return response.AccessToken;
            }
        }
    }
}
