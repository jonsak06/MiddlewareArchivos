using MiddlewareArchivos.Controllers;
using MiddlewareArchivos.Entities;
using MiddlewareArchivos.Enums;
using MiddlewareArchivos.Providers;
using System.Xml.Linq;

namespace MiddlewareArchivos
{
    public partial class Window : Form
    {
        private CarpetasController carpetasController;
        private XMLProvider provider;
        public Window()
        {
            InitializeComponent();
            this.carpetasController = new CarpetasController();
            this.provider = new XMLProvider();
        }

        private void Window_Load(object sender, EventArgs e)
        {

        }

        private void btnCrearCarpetas_Click(object sender, EventArgs e)
        {
            carpetasController.crearCarpetas();
        }

        private void btnGenerarEmpresas_Click(object sender, EventArgs e)
        {
            XDocument xmlEmpresas = provider.GetDocument(EnumArchivosXML.Empresas);
            List<Empresa> empresas = xmlEmpresas.Root.Elements().Select(e => new Empresa()
            {
                Id = long.Parse(e.Elements().FirstOrDefault(e => e.Name == "Id").Value),
                Nombre = e.Elements().FirstOrDefault(e => e.Name == "Nombre").Value,
                ManejaSecuencial = bool.Parse(e.Elements().FirstOrDefault(e => e.Name == "Secuencia").Value)
            }).ToList();

            foreach (Empresa empresa in empresas)
            {
                if (empresa.ManejaSecuencial)
                {
                    if (SecuenciasController.crearCtrlsec(empresa.Nombre, carpetasController.PathCarpetaCtrl))
                    {
                        LogsController.escribirEnLog(carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoCtrlsecCreado(empresa.Nombre, carpetasController.PathCarpetaCtrl));
                    }
                }
            }
        }
    }
}