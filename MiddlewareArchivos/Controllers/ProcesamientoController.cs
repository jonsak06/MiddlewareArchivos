using MiddlewareArchivos.Entities;
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
        private static string getTokenAutenticacion()
        {
            try
            {
                var token = Authenticator.GetTokenAsync();
                return token.Result;
            }
            catch (Exception ex)
            {
                //ver que hacer en caso de error al conectarse
                Console.WriteLine(ex);
                return String.Empty;
            }
        }
        private static void generarArchivoErr(string nombreArchivo, string pathCarpetaProcesado, string detalles)
        {
            throw new NotImplementedException();
        }
        public static bool procesarArchivoIn(Archivo archivo)
        {
            EndpointProvider endpointProvider = new EndpointProvider();
            Debug.WriteLine(endpointProvider.getEndpointPost(archivo.Api));
            var requestUri = new Uri($"{endpointProvider.getApiGatewayUrl()}{endpointProvider.getEndpointPost(archivo.Api)}");
            var token = getTokenAutenticacion();
            Debug.WriteLine(token);
            if(token != String.Empty)
            {
                switch(archivo.Api)
                {
                    case "Producto":
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
                    default:
                        return false;
                }
            }
            return false;
            
        }
    }
}
