using MiddlewareArchivosService.Controllers;
using MiddlewareArchivosService.Entities;
using MiddlewareArchivosService.Enums;
using MiddlewareArchivosService.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MiddlewareArchivosService.Services
{
    public class ConfiguracionEmpresasService
    {
        private readonly XMLProvider _provider;
        private readonly CarpetasController _carpetasController;
        public List<Empresa> empresas;
        public ConfiguracionEmpresasService()
        {
            _provider = new XMLProvider();
            empresas = ConfigurarEmpresas();
            _carpetasController = CarpetasController.Instance;
            CrearArchivosCtrlsec();
        }
        private List<Empresa> ConfigurarEmpresas()
        {
            XDocument xmlEmpresas = _provider.GetDocument(EnumArchivosXML.Empresas);
            return xmlEmpresas.Root.Elements().Select(e => new Empresa()
            {
                Id = long.Parse(e.Elements().FirstOrDefault(e => e.Name == "Id").Value),
                Nombre = e.Elements().FirstOrDefault(e => e.Name == "Nombre").Value,
                ManejaSecuencial = bool.Parse(e.Elements().FirstOrDefault(e => e.Name == "Secuencia").Value),
                MetodoSalida = e.Elements().FirstOrDefault(e => e.Name == "MetodoSalida").Value,
                WebhookSecret = e.Elements().FirstOrDefault(e => e.Name == "WebhookSecret").Value
            }).ToList();
            
        }
        private void CrearArchivosCtrlsec()
        {
            foreach (Empresa empresa in this.empresas)
            {
                if (empresa.ManejaSecuencial)
                {
                    SecuenciasController.crearCtrlsec(empresa.Nombre, this._carpetasController.PathCarpetaCtrl);
                }
            }
        }

    }
}
