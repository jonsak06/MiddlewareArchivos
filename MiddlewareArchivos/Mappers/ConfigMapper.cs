using MiddlewareArchivos.Constants;
using MiddlewareArchivos.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareArchivos.Mappers
{
    internal class ConfigMapper
    {
        public string GetKeyCarpeta(EnumCarpetas key)
        {
            switch (key)
            {
                case EnumCarpetas.Path: return CarpetasConstants.Path;

                case EnumCarpetas.CarpetaIn: return CarpetasConstants.CarpetaIn;
                case EnumCarpetas.CarpetaPendienteIn: return CarpetasConstants.CarpetaPendienteIn;
                case EnumCarpetas.CarpetaEnProcesoIn: return CarpetasConstants.CarpetaEnProcesoIn;
                case EnumCarpetas.CarpetaProcesadoIn: return CarpetasConstants.CarpetaProcesadoIn;
                case EnumCarpetas.CarpetaNoProcesadoIn: return CarpetasConstants.CarpetaNoProcesadoIn;
                case EnumCarpetas.CarpetaLogIn: return CarpetasConstants.CarpetaLogIn;

                case EnumCarpetas.CarpetaOut: return CarpetasConstants.CarpetaOut;
                case EnumCarpetas.CarpetaEnProcesoOut: return CarpetasConstants.CarpetaEnProcesoOut;
                case EnumCarpetas.CarpetaPendienteOut: return CarpetasConstants.CarpetaPendienteOut;
                case EnumCarpetas.CarpetaBackupOut: return CarpetasConstants.CarpetaBackupOut;
                case EnumCarpetas.CarpetaLogOut: return CarpetasConstants.CarpetaLogOut;

                case EnumCarpetas.CarpetaConfig: return CarpetasConstants.CarpetaConfig;
                case EnumCarpetas.CarpetaCtrl: return CarpetasConstants.CarpetaCtrl;
                default: return string.Empty;
            }

        }
        public string GetKeyXML(EnumArchivosXML key)
        {
            switch (key)
            {
                case EnumArchivosXML.Empresas: return ArchivosXMLConstants.Empresas;
                case EnumArchivosXML.Interfaces: return ArchivosXMLConstants.Interfaces;
                case EnumArchivosXML.Urls: return ArchivosXMLConstants.Urls;
                case EnumArchivosXML.Usuarios: return ArchivosXMLConstants.Usuarios;
                default: return string.Empty;
            }
        }

        public string GetNombreInterfaz(EnumInterfaces interfaz)
        {
            switch (interfaz)
            {
                case EnumInterfaces.Producto: return InterfacesConstants.Producto;
                case EnumInterfaces.CodigoBarras: return InterfacesConstants.CodigoBarras;
                case EnumInterfaces.Agenda: return InterfacesConstants.Agenda;
                case EnumInterfaces.Agente: return InterfacesConstants.Agente;
                case EnumInterfaces.AnulacionReferenciaRecepcion: return InterfacesConstants.AnulacionReferenciaRecepcion;
                case EnumInterfaces.Pedido: return InterfacesConstants.Pedido;
                case EnumInterfaces.ReferenciaRecepcion: return InterfacesConstants.ReferenciaRecepcion;
                case EnumInterfaces.ModificarDetalleReferencia: return InterfacesConstants.ModificarDetalleReferencia;
                case EnumInterfaces.ProductoProveedor: return InterfacesConstants.ProductoProveedor;
                case EnumInterfaces.Salida: return InterfacesConstants.Salida;
                default: return String.Empty;
            }
        }

        public string GetMetodoSalida(EnumMetodosSalida metodo)
        {
            switch (metodo)
            {
                case EnumMetodosSalida.MetodoSalida: return MetodosSalidaConstants.MetodoSalida;
                case EnumMetodosSalida.Polling: return MetodosSalidaConstants.Polling;
                case EnumMetodosSalida.Webhook: return MetodosSalidaConstants.Webhook;
                default: return String.Empty;
            }
        }
    }
}
