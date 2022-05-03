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
    internal class ProcesamientoController
    {
        private EndpointProvider endpointProvider;
        private string token;

        private async Task<ProcesamientoController> InitializeAsync()
        {
            this.token = await getTokenAutenticacion();
            return this;
        }
        public static Task<ProcesamientoController> CreateAsync()
        {
            var ret = new ProcesamientoController();
            return ret.InitializeAsync();
        }
        private ProcesamientoController()
        {
            this.endpointProvider = new EndpointProvider();
        }
        private async Task<string> getTokenAutenticacion()
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
        private void generarArchivoErr(string nombreArchivo, string pathCarpetaProcesado, string detalles)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> procesarArchivoIn(Archivo archivo)
        {
            if (InterfacesController.existeInterfaz(archivo.Api))
            {
                var requestUri = new Uri($"{this.endpointProvider.getApiGatewayUrl()}{this.endpointProvider.getEndpointPost(archivo.Api)}");

                if (this.token != String.Empty)
                {
                    using (var client = new HttpClient())
                    using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
                    {
                        request.Content = new StringContent(archivo.Contenido, Encoding.UTF8, "application/json");
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);


                        using (var response = await client.SendAsync(request))
                        {
                            var details = response.Content.ReadAsStringAsync();
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                Debug.WriteLine(response.StatusCode.ToString());
                                Debug.WriteLine($"Detalles: {details.Result}");
                                return true;
                            }
                            else
                            {
                                //generarArchivoErr(archivo.Nombre, "...", details.Result);
                                Debug.WriteLine(response.StatusCode.ToString());
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
