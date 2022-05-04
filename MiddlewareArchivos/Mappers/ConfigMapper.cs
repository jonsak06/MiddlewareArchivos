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
                case EnumCarpetas.Path: return NombresCarpetasConstants.Path;

                case EnumCarpetas.CarpetaIn: return NombresCarpetasConstants.CarpetaIn;
                case EnumCarpetas.CarpetaPendienteIn: return NombresCarpetasConstants.CarpetaPendienteIn;
                case EnumCarpetas.CarpetaEnProcesoIn: return NombresCarpetasConstants.CarpetaEnProcesoIn;
                case EnumCarpetas.CarpetaProcesadoIn: return NombresCarpetasConstants.CarpetaProcesadoIn;
                case EnumCarpetas.CarpetaNoProcesadoIn: return NombresCarpetasConstants.CarpetaNoProcesadoIn;
                case EnumCarpetas.CarpetaLogIn: return NombresCarpetasConstants.CarpetaLogIn;

                case EnumCarpetas.CarpetaOut: return NombresCarpetasConstants.CarpetaOut;
                case EnumCarpetas.CarpetaEnProcesoOut: return NombresCarpetasConstants.CarpetaEnProcesoOut;
                case EnumCarpetas.CarpetaPendienteOut: return NombresCarpetasConstants.CarpetaPendienteOut;
                case EnumCarpetas.CarpetaBackupOut: return NombresCarpetasConstants.CarpetaBackupOut;
                case EnumCarpetas.CarpetaLogOut: return NombresCarpetasConstants.CarpetaLogOut;

                case EnumCarpetas.CarpetaConfig: return NombresCarpetasConstants.CarpetaConfig;
                case EnumCarpetas.CarpetaCtrl: return NombresCarpetasConstants.CarpetaCtrl;
                default: return string.Empty;
            }

        }
        public string GetKeyXML(EnumArchivosXML key)
        {
            switch (key)
            {
                case EnumArchivosXML.Empresas: return NombresArchivosXMLConstants.Empresas;
                case EnumArchivosXML.Interfaces: return NombresArchivosXMLConstants.Interfaces;
                case EnumArchivosXML.Urls: return NombresArchivosXMLConstants.Urls;
                case EnumArchivosXML.Usuarios: return NombresArchivosXMLConstants.Usuarios;
                default: return string.Empty;
            }
        }

        public string GetNombreInterfaz(EnumInterfaces interfaz)
        {
            switch (interfaz)
            {
                case EnumInterfaces.Producto: return NombresInterfacesConstants.Producto;
                case EnumInterfaces.CodigoBarras: return NombresInterfacesConstants.CodigoBarras;
                case EnumInterfaces.Agenda: return NombresInterfacesConstants.Agenda;
                case EnumInterfaces.Agente: return NombresInterfacesConstants.Agente;
                case EnumInterfaces.AnulacionReferenciaRecepcion: return NombresInterfacesConstants.AnulacionReferenciaRecepcion;
                case EnumInterfaces.Pedido: return NombresInterfacesConstants.Pedido;
                case EnumInterfaces.ReferenciaRecepcion: return NombresInterfacesConstants.ReferenciaRecepcion;
                case EnumInterfaces.ModificarDetalleReferencia: return NombresInterfacesConstants.ModificarDetalleReferencia;
                case EnumInterfaces.ProductoProveedor: return NombresInterfacesConstants.ProductoProveedor;
                default: return String.Empty;
            }
        }
    }
}
