using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareArchivosService.Entities
{
    public class Archivo
    {
        public string Nombre { get; }
        public string NombreEmpresa { get; }
        public int Secuencia { get; }
        public string Api { get; }
        public string Texto { get; }
        public Empresa? Empresa { get; set; }
        public string Contenido { get; set; }
        public Archivo(string nombre)
        {
            Nombre = nombre;
            NombreEmpresa = Nombre.Split(".")[0];
            if (tieneSecuencial())
            {
                Secuencia = int.Parse(Nombre.Split(".")[1]);
                Api = Nombre.Split(".")[2];
                Texto = Nombre.Split(".")[3];
            }
            else
            {
                Api = Nombre.Split('.')[1];
                Texto = Nombre.Split('.')[2];
            }
        }
        public bool tieneSecuencial()
        {
            return Nombre.Split(".").Count() - 1 == 3;
        }
    }
}
