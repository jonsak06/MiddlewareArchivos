using MiddlewareArchivos.Entities;
using MiddlewareArchivos.Enums;
using MiddlewareArchivos.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
        private string pathCarpetaProcesadoIn;

        private ProcesamientoController()
        {
            this.endpointProvider = new EndpointProvider();
            this.pathCarpetaProcesadoIn = CarpetasController.Instance.PathCarpetaInProcesado;
        }
        private async Task<ProcesamientoController> InitializeAsync()
        {
            this.token = await getTokenAutenticacionAsync();
            return this;
        }
        public static Task<ProcesamientoController> CreateAsync()
        {
            var ret = new ProcesamientoController();
            return ret.InitializeAsync();
        }
        private async Task<string> getTokenAutenticacionAsync()
        {
            try
            {
                var token = await Authenticator.GetTokenAsync();
                return token;
            }
            catch
            {
                return String.Empty;
            }
        }
        private void generarArchivoErr(string nombreArchivo, string detalles)
        {
            using (StreamWriter sw = File.AppendText($"{this.pathCarpetaProcesadoIn}{nombreArchivo}.err"))
            {
                sw.WriteLine(detalles);
            }
        }
        public async Task<EnumRetornoProcesamiento> procesarArchivoInAsync(Archivo archivo)
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
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                //Debug.WriteLine(response.StatusCode.ToString());
                                //Debug.WriteLine($"Detalles: {details.Result}");
                                return EnumRetornoProcesamiento.Procesado;
                            }
                            else
                            {
                                //Debug.WriteLine(response.StatusCode.ToString());
                                //Debug.WriteLine($"Detalles: {details.Result}");
                                generarArchivoErr(archivo.Nombre, details.Result);
                                return EnumRetornoProcesamiento.ErrorProcesamiento;
                            }

                        }
                    }
                }
                else
                {
                    return EnumRetornoProcesamiento.ErrorAutenticacion;
                }
            }
            else
            {
                return EnumRetornoProcesamiento.ErrorInterfaz;
            }

        }
    }
}
