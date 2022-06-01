using MiddlewareArchivos.Enums;
using MiddlewareArchivos.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareArchivos.Controllers
{
    internal static class InterfacesController
    {
        private static ConfigMapper mapper = new ConfigMapper();
        private static List<string> getListaInterfaces()
        {
            List<string> listaInterfaces = new List<string>();
            foreach (EnumInterfaces i in Enum.GetValues(typeof(EnumInterfaces)))
            {
                listaInterfaces.Add(mapper.GetNombreInterfaz(i));
            }
            return listaInterfaces;
        }
        public static bool existeInterfaz(string interfaz)
        {
            return getListaInterfaces().Contains(interfaz);
        }
    }
}
