using MiddlewareArchivos.Entities;
using MiddlewareArchivos.Enums;
using MiddlewareArchivos.Mappers;
using MiddlewareArchivos.Providers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareArchivos.Controllers
{
    internal class ProcesamientoController
    {
        private EndpointProvider endpointProvider;
        public string token;
        private string pathCarpetaProcesadoIn, pathCarpetaEnProcesoOut;
        ConfigMapper mapper;

        private ProcesamientoController()
        {
            this.endpointProvider = new EndpointProvider();
            this.pathCarpetaProcesadoIn = CarpetasController.Instance.PathCarpetaInProcesado;
            this.pathCarpetaEnProcesoOut = CarpetasController.Instance.PathCarpetaOutEnProceso;
            mapper = new ConfigMapper();
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
            var det = JObject.Parse(detalles);
            using (StreamWriter sw = File.AppendText($"{this.pathCarpetaProcesadoIn}{nombreArchivo}.err"))
            {
                sw.WriteLine(det);
            }
        }
        private void generarArchivoOut(string nombreEmpresa, int numeroEjecucion, string nombreInterfaz, string contenido)
        {
            var cont = JObject.Parse(contenido);
            using (StreamWriter sw = File.AppendText($"{this.pathCarpetaEnProcesoOut}{nombreEmpresa}.{numeroEjecucion}.{nombreInterfaz}"))
            {
                sw.WriteLine(cont);
            }
        }
        public bool empresaCorrecta(Archivo archivo)
        {
            long codigoEmpresaContenido = long.Parse(JObject.Parse(archivo.Contenido)["empresa"].ToString());
            if (codigoEmpresaContenido == archivo.Empresa.Id)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> procesarArchivoInAsync(Archivo archivo)
        {
            var requestUri = new Uri($"{this.endpointProvider.getApiGatewayUrl()}{this.endpointProvider.getEndpointPost(archivo.Api)}");

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
            {
                client.Timeout = TimeSpan.FromMinutes(10);
                request.Content = new StringContent(archivo.Contenido, Encoding.UTF8, "application/json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);


                using (var response = await client.SendAsync(request))
                {
                    var details = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        generarArchivoErr(archivo.Nombre, details);
                        return false;
                    }
                }
            }

        }
        public async Task<bool> procesarArchivosOutAsync(Empresa empresa, string metodoSalida)//controlar errores y revisar que devuelve cuando no hay ejecuciones
        {
            var metodoPolling = this.mapper.GetMetodoSalida(EnumMetodosSalida.Polling);
            var metodoWebhook = this.mapper.GetMetodoSalida(EnumMetodosSalida.Webhook);

            if (metodoSalida == metodoPolling)
            {
                var requestUri = new Uri($"{this.endpointProvider.getApiGatewayUrl()}{this.endpointProvider.getEndpointGet(mapper.GetNombreInterfaz(EnumInterfaces.Salida))}?empresa={empresa.Id}");
                string contenido = null;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

                    using (var response = await client.GetAsync(requestUri))
                    {
                        if (response.IsSuccessStatusCode)
                            contenido = await response.Content.ReadAsStringAsync();
                    }
                }

                if (contenido == null)
                    return false;

                if (contenido != String.Empty)
                {
                    //creación de colección con ejecuciones
                    var json = JObject.Parse(contenido);
                    var ejecucionesJson = json["ejecuciones"];
                    var ejecuciones = new NameValueCollection();
                    foreach (var ejec in ejecucionesJson)
                    {
                        ejecuciones.Add(ejec["codigoInterfazExterna"].ToString(), ejec["numeroInterfazEjecucion"].ToString());
                    }

                    foreach (string interfaz in ejecuciones.AllKeys)
                    {
                        int codigoInterfaz = int.Parse(interfaz);
                        string endpoint = this.endpointProvider.getEndpointGet(codigoInterfaz);
                        string nombreInterfaz = endpoint.Split("/")[0];
                        var ejecucionesDeInterfaz = ejecuciones[interfaz].Split(",");

                        foreach (string ejecucion in ejecucionesDeInterfaz)
                        {
                            int numeroEjecucion = int.Parse(ejecucion);

                            requestUri = new Uri($"{this.endpointProvider.getApiGatewayUrl()}{endpoint}?nroEjecucion={numeroEjecucion}&empresa={empresa.Id}");

                            using (var client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

                                using (var response = await client.GetAsync(requestUri))
                                {
                                    if (response.IsSuccessStatusCode)
                                        contenido = await response.Content.ReadAsStringAsync();
                                        Debug.WriteLine(contenido);
                                }
                            }
                            generarArchivoOut(empresa.Nombre, numeroEjecucion, nombreInterfaz, contenido);
                        }

                    }
                    return true;
                }

            }
            else if (metodoSalida == metodoWebhook)
            {
                return false;
            }
            return false;
        }

    }
}
