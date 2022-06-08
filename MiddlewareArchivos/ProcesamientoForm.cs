using MiddlewareArchivos.Controllers;
using MiddlewareArchivos.Entities;
using MiddlewareArchivos.Enums;
using MiddlewareArchivos.Mappers;
using MiddlewareArchivos.Providers;
using System.Configuration;
using System.Diagnostics;
using System.Xml.Linq;

namespace MiddlewareArchivos
{
    public partial class ProcesamientoForm : Form
    {
        private CarpetasController carpetasController;
        private XMLProvider provider;
        private List<Empresa> empresas;
        private string metodoSalida;
        public ProcesamientoForm()
        {
            InitializeComponent();
            this.carpetasController = CarpetasController.Instance;
            this.provider = new XMLProvider();
            this.empresas = new List<Empresa>();

            ConfigMapper mapper = new ConfigMapper();
            this.metodoSalida = ConfigurationManager.AppSettings[mapper.GetMetodoSalida(EnumMetodosSalida.MetodoSalida)];
        }

        private void ProcesamientoForm_Load(object sender, EventArgs e)
        {
            //creación de instancias de empresas
            XDocument xmlEmpresas = provider.GetDocument(EnumArchivosXML.Empresas);
            this.empresas = xmlEmpresas.Root.Elements().Select(e => new Empresa()
            {
                Id = long.Parse(e.Elements().FirstOrDefault(e => e.Name == "Id").Value),
                Nombre = e.Elements().FirstOrDefault(e => e.Name == "Nombre").Value,
                ManejaSecuencial = bool.Parse(e.Elements().FirstOrDefault(e => e.Name == "Secuencia").Value)
            }).ToList();

            //creación de archivos .ctrlsec
            foreach (Empresa empresa in this.empresas)
            {
                if (empresa.ManejaSecuencial)
                {
                    if (SecuenciasController.crearCtrlsec(empresa.Nombre, this.carpetasController.PathCarpetaCtrl))
                    {
                        LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoCtrlsecCreado(empresa.Nombre, carpetasController.PathCarpetaCtrl));
                    }
                }
            }
            LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeSeparador());
        }

        private async void btnProcesarArchivosIn_Click(object sender, EventArgs e)
        {
            btnProcesarArchivosIn.Enabled = false;
            ProcesamientoController procesamientoController = await ProcesamientoController.CreateAsync();
            string pathCarpetaInLog = this.carpetasController.PathCarpetaInLog;

            #region testeo con un archivo
            //Archivo archivo = new Archivo("acme.1.Producto.prueba1");
            //archivo.Contenido = File.ReadAllText($"C:\\Pasantia\\IN\\PENDIENTE\\acme.1.Producto.prueba1");
            //archivo.Empresa = empresas.FirstOrDefault(e => e.Nombre == archivo.NombreEmpresa);

            #endregion

            if (procesamientoController.token == String.Empty)
            {
                LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeFalloAutenticacion());
                MessageBox.Show($"Error de autenticación");
                btnProcesarArchivosIn.Enabled = true;
                return;
            }

            string[] pathsArchivosIn = Directory.GetFiles(this.carpetasController.PathCarpetaInPendiente);
            LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeCantidadArchivosDetectados(pathsArchivosIn.Length, this.carpetasController.PathCarpetaInPendiente));
            if (pathsArchivosIn.Length > 0)
            {
                List<Archivo> archivos = new List<Archivo>();
                foreach (string path in pathsArchivosIn)
                {
                    string[] splitedPath = path.Split("\\");
                    string nombreArchivo = splitedPath[splitedPath.Length - 1];
                    if (nombreArchivo.Split(".").Length == 3 || nombreArchivo.Split(".").Length == 4)
                    {
                        archivos.Add(new Archivo(splitedPath[splitedPath.Length - 1]));
                    }
                    else
                    {
                        //mueve archivos cuyo nombre no cumplen con el formato para procesarlos
                        File.Move(path, $"{this.carpetasController.PathCarpetaInNoProcesado}{nombreArchivo}");
                        LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeNombreArchivoNoCumpleFormato(nombreArchivo));
                        LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoMovido(nombreArchivo, this.carpetasController.PathCarpetaInNoProcesado));
                    }
                }

                archivos = SecuenciasController.ordenarPorSecuencia(archivos);
                LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivosOrdenados());


                foreach (Archivo archivo in archivos)
                {
                    //carga de las empresas en los archivos
                    archivo.Empresa = empresas.FirstOrDefault(e => e.Nombre == archivo.NombreEmpresa);

                    //carga del contenido a los archivos
                    archivo.Contenido = File.ReadAllText($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}");

                    if (archivo.Empresa == null)
                    {
                        LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeEmpresaInexistente(archivo.NombreEmpresa));
                        File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                        LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInNoProcesado));
                    }
                    else if (!InterfacesController.existeInterfaz(archivo.Api))
                    {
                        LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeInterfazInexistente(archivo.Api));
                        File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                        LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInNoProcesado));
                    }
                    else if (!procesamientoController.empresaCorrecta(archivo))//si id empresa del nombre del archivo != id empresa del contenido
                    {
                        LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeEmpresaIncorrecta(archivo.NombreEmpresa));
                        File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                        LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInNoProcesado));
                    }
                    else if (!archivo.tieneSecuencial() && !archivo.Empresa.ManejaSecuencial)
                    {
                        File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}");
                        LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInEnProceso));

                        if (await procesamientoController.procesarArchivoInAsync(archivo))
                        {
                            LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoProcesado(archivo.Nombre));
                        }
                        else
                        {
                            LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoNoProcesado(archivo.Nombre));
                        }

                        File.Move($"{this.carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInProcesado}{archivo.Nombre}");
                        LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInProcesado));

                    }
                    else if ((!archivo.tieneSecuencial() && archivo.Empresa.ManejaSecuencial) || (archivo.tieneSecuencial() && !archivo.Empresa.ManejaSecuencial))
                    {
                        File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                        LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeNombreArchivoNoCumpleFormato(archivo.Nombre));
                        LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInNoProcesado));
                    }
                    else if (archivo.tieneSecuencial() && archivo.Empresa.ManejaSecuencial)
                    {
                        if (SecuenciasController.secuenciaCorrecta(this.carpetasController.PathCarpetaCtrl, archivo))
                        {
                            File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}");
                            LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInEnProceso));

                            if (await procesamientoController.procesarArchivoInAsync(archivo))
                            {
                                LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoProcesado(archivo.Nombre));
                            }
                            else
                            {
                                LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoNoProcesado(archivo.Nombre));
                            }

                            File.Move($"{this.carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInProcesado}{archivo.Nombre}");
                            LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInProcesado));

                            string pathArchivoCtrlsec = SecuenciasController.getPathArchivoCtrlsec(this.carpetasController.PathCarpetaCtrl, archivo.NombreEmpresa);
                            SecuenciasController.aumentarSecuencia(pathArchivoCtrlsec);// aumenta 1 en la ultima secuencia procesada (en archivo ctrlsec)
                            LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeSecuenciaAumentada(archivo.Secuencia, pathArchivoCtrlsec));
                        }
                        else
                        {
                            string pathArchivoCtrlsec = SecuenciasController.getPathArchivoCtrlsec(this.carpetasController.PathCarpetaCtrl, archivo.NombreEmpresa);
                            if (archivo.Secuencia <= SecuenciasController.getUltimaSecuenciaProcesada(pathArchivoCtrlsec) ||
                                archivo.Secuencia > SecuenciasController.getSecuenciaFinal(pathArchivoCtrlsec))
                            {
                                File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                                LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeSecuenciaIncorrecta(archivo.Nombre));
                                LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInNoProcesado));
                            }
                            else
                            {
                                LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeArchivoPendienteProcesamiento(archivo.Nombre, this.carpetasController.PathCarpetaInPendiente));
                            }
                        }
                    }
                }
            }
            LogsController.escribirEnLog(pathCarpetaInLog, LogsController.mensajeSeparador());
            MessageBox.Show($"Finalizado el procesamiento de archivos de IN");
            btnProcesarArchivosIn.Enabled = true;
        }

        private async void btnProcesarArchivosOut_Click(object sender, EventArgs e)
        {
            btnProcesarArchivosOut.Enabled = false;
            ProcesamientoController procesamientoController = await ProcesamientoController.CreateAsync();
            if(!await procesamientoController.procesarArchivosOutAsync(empresas[0], this.metodoSalida))
            {
                //log
                MessageBox.Show($"Error al obtener ejecuciones de la interfaz \"Salida\"");
                btnProcesarArchivosOut.Enabled = true;
                return;
            }

            string[] pathsArchivosOut = Directory.GetFiles(this.carpetasController.PathCarpetaOutEnProceso);
            if (pathsArchivosOut.Length > 0)
            {
                foreach (string pathArchivo in pathsArchivosOut)
                {
                    string[] splitedPath = pathArchivo.Split("\\");
                    string nombreArchivo = splitedPath[splitedPath.Length - 1];

                    string pathArchivoBackup = $"{this.carpetasController.PathCarpetaOutBackup}{nombreArchivo}";
                    if (!File.Exists(pathArchivoBackup))
                        File.Copy(pathArchivo, pathArchivoBackup);
                    else
                        File.Delete(pathArchivo);

                    string pathArchivoPendiente = $"{this.carpetasController.PathCarpetaOutPendiente}{nombreArchivo}";
                    if (!File.Exists(pathArchivoPendiente))
                        File.Move(pathArchivo, pathArchivoPendiente);
                    else
                        File.Delete(pathArchivo);
                }
                MessageBox.Show($"Finalizado el procesamiento de archivos de OUT");
            } 
            else
            {
                MessageBox.Show($"No se generaron nuevos archivos");
            }
            btnProcesarArchivosOut.Enabled = true;
        }

    }
}