using MiddlewareArchivos.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareArchivos.Controllers
{
    internal class SecuenciasController
    {
        public static void aumentarSecuencia(string pathArchivoCtrlsec)
        {
            string[] lineas = File.ReadAllLines(pathArchivoCtrlsec);
            string lineaUltimaSecuencia = lineas[2];
            int nuevaUltimaSecuencia = int.Parse(lineaUltimaSecuencia.Split(":")[1]) + 1;
            lineas[2] = $"{lineaUltimaSecuencia.Split(':')[0]}:{nuevaUltimaSecuencia}";
            File.WriteAllLines(pathArchivoCtrlsec, lineas);
        }
        private static int getSecuenciaInicial(string pathArchivoCtrlsec)
        {
            string[] lineas = File.ReadAllLines(pathArchivoCtrlsec);
            return int.Parse(lineas[0].Split(":")[1]);
        }
        public static int getSecuenciaFinal(string pathArchivoCtrlsec)
        {
            string[] lineas = File.ReadAllLines(pathArchivoCtrlsec);
            return int.Parse(lineas[1].Split(":")[1]);
        }
        public static int getUltimaSecuenciaProcesada(string pathArchivoCtrlsec)
        {
            string[] lineas = File.ReadAllLines(pathArchivoCtrlsec);
            return int.Parse(lineas[2].Split(":")[1]); //lineas[0] = Inicio:1, lineas[1] = Fin:9999999, lineas[2] = Ultima secuencia procesada:n
        }
        public static string getPathArchivoCtrlsec(string pathCarpetaCtrl, string nombreEmpresa)
        {
            return $"{pathCarpetaCtrl}{nombreEmpresa}.ctrlsec";
        }
        public static List<Archivo> ordenarPorSecuencia(List<Archivo> archivos)
        {
            return archivos.OrderBy(a => a.Secuencia).ToList();
        }
        public static bool secuenciaCorrecta(string pathCarpetaCtrl, Archivo archivo)
        {
            string pathArchivoCtrlsec = getPathArchivoCtrlsec(pathCarpetaCtrl, archivo.NombreEmpresa);
            int secuenciaInicial = getSecuenciaInicial(pathArchivoCtrlsec);
            int secuenciaFinal = getSecuenciaFinal(pathArchivoCtrlsec);
            int secuenciaActual = getUltimaSecuenciaProcesada(pathArchivoCtrlsec);
            if (archivo.Secuencia == secuenciaActual + 1 && archivo.Secuencia >= secuenciaInicial && archivo.Secuencia <= secuenciaFinal)
            {
                return true;
            }
            return false;
        }
        public static bool crearCtrlsec(string nombreEmpresa, string pathCarpetaCtrl)
        {
            string pathArchivoCtrlsec = $"{pathCarpetaCtrl}{nombreEmpresa}.ctrlsec";
            if (!File.Exists(pathArchivoCtrlsec))
            {
                using (StreamWriter sw = File.CreateText(pathArchivoCtrlsec))
                {
                    sw.WriteLine("Inicio:1");
                    sw.WriteLine("Fin:9999999");
                    sw.WriteLine("Ultima secuencia procesada:0");
                }
                return true;
            }
            return false;
        }
    }
}
