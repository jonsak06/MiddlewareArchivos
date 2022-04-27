using MiddlewareArchivos.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiddlewareArchivos.Controllers
{
    internal class ArchivosController
    {
        public static bool procesarArchivoIn(Archivo archivo)
        {
            //get token de autenticación a las APIs
            //try
            //{
            //    var token = Authenticator.GetTokenAsync();
            //    Console.WriteLine(token.Result);
            //    if (archivo.Api == "Producto")
            //    {
            //        using (var client = new HttpClient())
            //        using (var request = new HttpRequestMessage(HttpMethod.Post, "https://desarrollo56.lis-cloud.com:2030/api/Producto/CreateOrUpdate"))
            //        {
            //            client.Timeout = TimeSpan.FromSeconds(10);
            //            Console.WriteLine("creando producto...");
            //            request.Content = new StringContent(JsonSerializer.Serialize(archivo.Contenido), Encoding.UTF8, "application/json");
            //            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Result);


            //            using (var response = await client.SendAsync(request))
            //            {
            //                Console.WriteLine(response.StatusCode.ToString());
            //                var details = response.Content.ReadAsStringAsync();
            //                Console.WriteLine($"producto creado, detalles: {details}");
            //                //return true;


            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}
            //return false;
            Console.WriteLine("Procesando archivo...");
            Thread.Sleep(1000);
            return true; //true si se procesa correctamente, sino se genera .err y devuelve false
        }
    }
}
