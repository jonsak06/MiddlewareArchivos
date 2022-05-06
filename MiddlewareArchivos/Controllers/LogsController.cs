using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareArchivos.Controllers
{
    internal class LogsController
    {
        public static void escribirEnLog(string pathCarpetaLog, string mensaje)
        {
            using (StreamWriter sw = File.AppendText($"{pathCarpetaLog}log"))
            {
                sw.WriteLine(mensaje);
            }
        }
        public static string mensajeCantidadArchivosDetectados(int cantidadArchivos, string pathCarpetaPendienteIn)
        {
            return $"{cantidadArchivos} archivos detectados en la carpeta {getNombreCarpeta(pathCarpetaPendienteIn)} | {DateTime.UtcNow.AddHours(-3)}";
        }
        public static string mensajeArchivosOrdenados()
        {
            return $"Archivos ordenados por secuencia para su procesamiento | {DateTime.UtcNow.AddHours(-3)}";
        }
        public static string mensajeArchivoMovido(string nombreArchivo, string pathCarpetaDestino)
        {
            return $"Movido el archivo {nombreArchivo} a la carpeta {getNombreCarpeta(pathCarpetaDestino)} | {DateTime.UtcNow.AddHours(-3)}";
        }
        public static string mensajeArchivoProcesado(string nombreArchivo)
        {
            return $"Procesado el archivo {nombreArchivo} exitosamente | {DateTime.UtcNow.AddHours(-3)}";
        }
        public static string mensajeArchivoNoProcesado(string nombreArchivo)
        {
            return $"El archivo {nombreArchivo} no se pudo procesar correctamente | {DateTime.UtcNow.AddHours(-3)}";
        }
        public static string mensajeSecuenciaAumentada(int secuencia, string pathArchivoCtrlsec)
        {
            return $"Aumentado a {secuencia} la última secuencia procesada en {getNombreArchivo(pathArchivoCtrlsec)} | {DateTime.UtcNow.AddHours(-3)}";
        }
        public static string mensajeArchivoCtrlsecCreado(string nombreEmpresa, string pathCarpetaCtrl)
        {
            return $"Se ha creado el archivo {nombreEmpresa}.ctrlsec en la carpeta {getNombreCarpeta(pathCarpetaCtrl)} | {DateTime.UtcNow.AddHours(-3)}";
        }
        public static string mensajeSecuenciaIncorrecta(string nombreArchivo)
        {
            return $"Secuencia del archivo {nombreArchivo} incorrecta | {DateTime.UtcNow.AddHours(-3)}";
        }
        public static string mensajeArchivoPendienteProcesamiento(string nombreArchivo, string pathCarpetaPendiente)
        {
            return $"Archivo {nombreArchivo} se mantiene en la carpeta {getNombreCarpeta(pathCarpetaPendiente)} para su futuro procesamiento | {DateTime.UtcNow.AddHours(-3)}";
        }
        public static string mensajeNombreArchivoNoCumpleFormato(string nombreArchivo)
        {
            return $"Nombre de archivo {nombreArchivo} no cumple con el formato necesario para su procesamiento | {DateTime.UtcNow.AddHours(-3)}";
        }
        public static string mensajeSeparador()
        {
            return "------------------------------------------------------------------------------------------------------";
        }
        private static string getNombreCarpeta(string pathCarpeta)
        {
            string[] splitedPath = pathCarpeta.Split('\\');
            return splitedPath[splitedPath.Length - 2];
        }
        private static string getNombreArchivo(string pathArchivo)
        {
            string[] splitedPath = pathArchivo.Split('\\');
            return splitedPath[splitedPath.Length - 1];
        }
    }
}
