using MiddlewareArchivos.Entities;
using MiddlewareArchivos.Enums;
using MiddlewareArchivos.Mappers;
using MiddlewareArchivos.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiddlewareArchivos.Controllers
{
    internal static class ProcesamientoController
    {
        private static async Task<string> getTokenAutenticacion()
        {
            try
            {
                var token = await Authenticator.GetTokenAsync();
                return token;
            }
            catch (Exception ex)
            {
                //ver que hacer en caso de error al conectarse
                Debug.WriteLine(ex);
                return String.Empty;
            }
        }
        private static void generarArchivoErr(string nombreArchivo, string pathCarpetaProcesado, string detalles)
        {
            throw new NotImplementedException();
        }
        public static async Task<bool> procesarArchivoIn(Archivo archivo)
        {
            if(InterfacesController.existeInterfaz(archivo.Api))
            {
                ConfigMapper mapper = new ConfigMapper();
                EndpointProvider endpointProvider = new EndpointProvider();
                Debug.WriteLine(endpointProvider.getEndpointPost(archivo.Api));
                var requestUri = new Uri($"{endpointProvider.getApiGatewayUrl()}{endpointProvider.getEndpointPost(archivo.Api)}");
                var token = await getTokenAutenticacion();

                if (token != String.Empty)
                {
                    using (var client = new HttpClient())
                    using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
                    {
                        request.Content = new StringContent(archivo.Contenido, Encoding.UTF8, "application/json");
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


                        using (var response = client.SendAsync(request))
                        {
                            var details = response.Result.Content.ReadAsStringAsync();
                            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                Debug.WriteLine(response.Result.StatusCode.ToString());
                                Debug.WriteLine($"Detalles: {details.Result}");
                                return true;
                            }
                            else
                            {
                                //generarArchivoErr(archivo.Nombre, "...", details.Result);
                                Debug.WriteLine(response.Result.StatusCode.ToString());
                                Debug.WriteLine($"Detalles: {details.Result}");
                                return false;
                            }

                        }
                    }
                }
            }
            return false;
            
        }
    }
}
