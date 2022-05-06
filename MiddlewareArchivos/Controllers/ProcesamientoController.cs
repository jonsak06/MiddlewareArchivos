using MiddlewareArchivos.Entities;
using MiddlewareArchivos.Enums;
using MiddlewareArchivos.Providers;
using Newtonsoft.Json.Linq;
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
        public string token;
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
                request.Content = new StringContent(archivo.Contenido, Encoding.UTF8, "application/json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.token);


                using (var response = await client.SendAsync(request))
                {
                    var details = response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        generarArchivoErr(archivo.Nombre, details.Result);
                        return false;
                    }
                }
            }
            
        }
    }
}
