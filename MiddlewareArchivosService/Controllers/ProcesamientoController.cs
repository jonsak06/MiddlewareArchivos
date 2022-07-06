using MiddlewareArchivosService.Entities;
using MiddlewareArchivosService.Enums;
using MiddlewareArchivosService.Mappers;
using MiddlewareArchivosService.Providers;
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

namespace MiddlewareArchivosService.Controllers
{
    public class ProcesamientoController
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
        private void generarArchivoErr(string nombreArchivo, JObject detalles)
        {
            var pathArchivo = $"{this.pathCarpetaProcesadoIn}{nombreArchivo}.err";
            if (!File.Exists(pathArchivo))
            {
                using (StreamWriter sw = File.AppendText(pathArchivo))
                {
                    sw.WriteLine(detalles);
                }
            }
        }
        private void generarArchivoOut(string nombreEmpresa, int numeroEjecucion, string nombreInterfaz, string contenido)
        {
            var pathArchivo = $"{this.pathCarpetaEnProcesoOut}{numeroEjecucion}.{nombreEmpresa}.{nombreInterfaz}";
            if (!File.Exists(pathArchivo))
            {
                var cont = JObject.Parse(contenido);
                using (StreamWriter sw = File.AppendText(pathArchivo))
                {
                    sw.WriteLine(cont);
                }
            }
        }
        private SortedDictionary<string, string> crearDiccionarioEjecuciones(string contenido)
        {
            var json = JObject.Parse(contenido);
            var ejecucionesJson = json["ejecuciones"];
            var ejecuciones = new SortedDictionary<string, string>();

            foreach (var ejec in ejecucionesJson)
            {
                ejecuciones.Add(ejec["numeroInterfazEjecucion"].ToString(), ejec["codigoInterfazExterna"].ToString());
            }
            return ejecuciones;
        }
        private async Task<KeyValuePair<bool, string>> realizarGetRequest(Uri requestUri)//key = succesful response?, value = contenido
        {
            string contenido = String.Empty;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

                using (var response = await client.GetAsync(requestUri))
                {
                    contenido = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return new KeyValuePair<bool, string>(true, contenido);
                    }
                    else
                    {
                        return new KeyValuePair<bool, string>(false, contenido);
                    }
                }
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
        public async Task<KeyValuePair<bool,string>> procesarArchivoInAsync(Archivo archivo)
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
                    var details = JObject.Parse(await response.Content.ReadAsStringAsync());
                    if (response.IsSuccessStatusCode)
                    {
                        var numeroEjecucion = details["numeroInterfaz"].ToString();
                        return new KeyValuePair<bool, string>(true,numeroEjecucion);
                    }
                    else
                    {
                        generarArchivoErr(archivo.Nombre, details);
                        return new KeyValuePair<bool, string>(false,string.Empty);
                    }
                }
            }

        }
        public async Task<bool> procesarArchivosOutAsync(Empresa empresa)
        {
            var polling = this.mapper.GetMetodoSalida(EnumMetodosSalida.Polling);
            var webhook = this.mapper.GetMetodoSalida(EnumMetodosSalida.Webhook);

            if (empresa.MetodoSalida == polling)
            {
                return await procesarConPolling(empresa);
            }
            else if (empresa.MetodoSalida == webhook)
            {
                return await procesarConWebhook(empresa);
            }
            return false;
        }

        private async Task<bool> procesarConPolling(Empresa empresa)
        {
            var loggerOut = NLog.LogManager.GetLogger("loggerOut");

            //Obtención de ejecuciones pendientes
            var requestUri = new Uri($"{this.endpointProvider.getApiGatewayUrl()}{this.endpointProvider.getEndpointEjecucionesPendientes()}?empresa={empresa.Id}");
            var contenido = await realizarGetRequest(requestUri);

            if (!contenido.Key)
                return false;

            //Crea diccionario key = número de ejecucion, value = codigo de interfaz
            var ejecuciones = crearDiccionarioEjecuciones(contenido.Value);

            if (ejecuciones.Count() == 0)
            {
                loggerOut.Info($"No se encontraron ejecuciones pendientes para la empresa [{empresa.Id}] {empresa.Nombre}");
                return true;
            }

            loggerOut.Info($"Se encontraron {ejecuciones.Count} ejecuciones pendientes para la empresa [{empresa.Id}] {empresa.Nombre}");
            foreach (KeyValuePair<string, string> kvp in ejecuciones)
            {
                int numeroEjecucion = int.Parse(kvp.Key);
                int codigoInterfaz = int.Parse(kvp.Value);

                //Consulta estado de ejecución
                string endpoint = this.endpointProvider.getEndpointConsultarEstado();
                requestUri = new Uri($"{this.endpointProvider.getApiGatewayUrl()}{endpoint}?nroEjecucion={numeroEjecucion}&empresa={empresa.Id}");
                contenido = await realizarGetRequest(requestUri);

                if (!contenido.Key)
                {
                    loggerOut.Warn($"La ejecución {numeroEjecucion} de la empresa [{empresa.Id}] {empresa.Nombre} no está lista para su lectura");
                    continue;
                }

                endpoint = this.endpointProvider.getEndpointGet(codigoInterfaz);
                string nombreInterfaz = endpoint.Split("/")[0];

                requestUri = new Uri($"{this.endpointProvider.getApiGatewayUrl()}{endpoint}?nroEjecucion={numeroEjecucion}&empresa={empresa.Id}");
                contenido = await realizarGetRequest(requestUri);

                generarArchivoOut(empresa.Nombre, numeroEjecucion, nombreInterfaz, contenido.Value);
                loggerOut.Info($"Generado el archivo {numeroEjecucion}.{empresa.Nombre}.{nombreInterfaz} en {this.pathCarpetaEnProcesoOut}");

                //Confirmar lectura de ejecucion
                //    endpoint = this.endpointProvider.getEndpointPost(this.mapper.GetNombreInterfaz(EnumInterfaces.Salida));
                //    requestUri = new Uri($"{this.endpointProvider.getApiGatewayUrl()}{endpoint}");

                //    using (var client = new HttpClient())
                //    using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
                //    {
                //        string cont = $"{{'empresa': {empresa.Id}, 'numeroInterfazEjecucion': {numeroEjecucion}, 'codigoInterfazExterna': {codigoInterfaz}, 'resultado': true}}";
                //        request.Content = new StringContent(cont, Encoding.UTF8, "application/json");
                //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);

                //        using (var response = await client.SendAsync(request))
                //        {
                //            var details = await response.Content.ReadAsStringAsync();
                //            if (response.IsSuccessStatusCode)
                //            {
                //                loggerOut.Info($"Confirmada la lectura de la ejecucion {numeroEjecucion} de la empresa [{empresa.Id}] {empresa.Nombre}");
                //            }
                //            else
                //            {
                //                loggerOut.Error($"Error al confirmar lectura de la ejecución {numeroEjecucion} de la empresa [{empresa.Id}] {empresa.Nombre}. Detalles: {details}");
                //            }
                //        }
                //    }

            }
            return true;
        }
        private async Task<bool> procesarConWebhook(Empresa empresa)
        {
            var loggerOut = NLog.LogManager.GetLogger("loggerOut");

            loggerOut.Info($"Método Webhook aún no implementado (Empresa: [{empresa.Id}] {empresa.Nombre})");
            return false;
        }
    }

}