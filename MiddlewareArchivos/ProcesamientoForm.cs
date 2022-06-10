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
                    SecuenciasController.crearCtrlsec(empresa.Nombre, this.carpetasController.PathCarpetaCtrl);
                }
            }
        }

        private async void btnProcesarArchivosIn_Click(object sender, EventArgs e)
        {
            btnProcesarArchivosIn.Enabled = false;
            var loggerIn = NLog.LogManager.GetLogger("loggerIn");

            ProcesamientoController procesamientoController = await ProcesamientoController.CreateAsync();
            string pathCarpetaInLog = this.carpetasController.PathCarpetaInLog;

            if (procesamientoController.token == String.Empty)
            {
                loggerIn.Error("Error al solicitar token de autenticación");
                MessageBox.Show($"Error de autenticación");
                btnProcesarArchivosIn.Enabled = true;
                return;
            }

            loggerIn.Info($"Iniciado el procesamiento de archivos en {this.carpetasController.PathCarpetaIn}");

            string[] pathsArchivosIn = Directory.GetFiles(this.carpetasController.PathCarpetaInPendiente);
            loggerIn.Info($"{pathsArchivosIn.Length} archivos detectados en la carpeta {this.carpetasController.PathCarpetaInPendiente}");
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
                        loggerIn.Info($"Nombre de archivo {nombreArchivo} no cumple con el formato necesario para su procesamiento");
                        loggerIn.Info($"Movido el archivo {nombreArchivo} a la carpeta {this.carpetasController.PathCarpetaInNoProcesado}");
                    }
                }

                archivos = SecuenciasController.ordenarPorSecuencia(archivos);
                loggerIn.Info("Archivos ordenados por secuencia para su procesamiento");


                foreach (Archivo archivo in archivos)
                {
                    //carga de las empresas en los archivos
                    archivo.Empresa = empresas.FirstOrDefault(e => e.Nombre == archivo.NombreEmpresa);

                    //carga del contenido a los archivos
                    archivo.Contenido = File.ReadAllText($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}");

                    if (archivo.Empresa == null)
                    {
                        loggerIn.Error($"Empresa {archivo.NombreEmpresa} no existe en el registro");
                        File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                        loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {this.carpetasController.PathCarpetaInNoProcesado}");
                    }
                    else if (!InterfacesController.existeInterfaz(archivo.Api))
                    {
                        loggerIn.Error($"Interfaz {archivo.Api} no existe en el registro");
                        File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                        loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {this.carpetasController.PathCarpetaInNoProcesado}");
                    }
                    else if (!procesamientoController.empresaCorrecta(archivo))//si id empresa del nombre del archivo != id empresa del contenido
                    {
                        loggerIn.Error($"Código de la empresa {archivo.NombreEmpresa} no corresponde con el declarado en el contenido del archivo");
                        File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                        loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {this.carpetasController.PathCarpetaInNoProcesado}");
                    }
                    else if (!archivo.tieneSecuencial() && !archivo.Empresa.ManejaSecuencial)
                    {
                        File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}");
                        loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {this.carpetasController.PathCarpetaInEnProceso}");

                        if (await procesamientoController.procesarArchivoInAsync(archivo))
                        {
                            loggerIn.Info($"Procesado el archivo {archivo.Nombre} exitosamente");
                        }
                        else
                        {
                            loggerIn.Error($"Archivo {archivo.Nombre} no se pudo procesar correctamente, generado {archivo.Nombre}.err");
                        }

                        File.Move($"{this.carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInProcesado}{archivo.Nombre}");
                        loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {this.carpetasController.PathCarpetaInProcesado}");

                    }
                    else if ((!archivo.tieneSecuencial() && archivo.Empresa.ManejaSecuencial) || (archivo.tieneSecuencial() && !archivo.Empresa.ManejaSecuencial))
                    {
                        File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                        loggerIn.Error($"Nombre de archivo {archivo.Nombre} no cumple con el formato necesario para su procesamiento");
                        loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {this.carpetasController.PathCarpetaInNoProcesado}");
                    }
                    else if (archivo.tieneSecuencial() && archivo.Empresa.ManejaSecuencial)
                    {
                        if (SecuenciasController.secuenciaCorrecta(this.carpetasController.PathCarpetaCtrl, archivo))
                        {
                            File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}");
                            loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {this.carpetasController.PathCarpetaInEnProceso}");

                            if (await procesamientoController.procesarArchivoInAsync(archivo))
                            {
                                loggerIn.Info($"Procesado el archivo {archivo.Nombre} exitosamente");
                            }
                            else
                            {
                                loggerIn.Error($"Archivo {archivo.Nombre} no se pudo procesar correctamente, generado {archivo.Nombre}.err");
                            }

                            File.Move($"{this.carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInProcesado}{archivo.Nombre}");
                            loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {this.carpetasController.PathCarpetaInProcesado}");

                            string pathArchivoCtrlsec = SecuenciasController.getPathArchivoCtrlsec(this.carpetasController.PathCarpetaCtrl, archivo.NombreEmpresa);
                            SecuenciasController.aumentarSecuencia(pathArchivoCtrlsec);// aumenta 1 en la ultima secuencia procesada (en archivo ctrlsec)
                            loggerIn.Info($"Aumentado a {archivo.Secuencia} la última secuencia procesada en {pathArchivoCtrlsec}");
                        }
                        else
                        {
                            string pathArchivoCtrlsec = SecuenciasController.getPathArchivoCtrlsec(this.carpetasController.PathCarpetaCtrl, archivo.NombreEmpresa);
                            if (archivo.Secuencia <= SecuenciasController.getUltimaSecuenciaProcesada(pathArchivoCtrlsec) ||
                                archivo.Secuencia > SecuenciasController.getSecuenciaFinal(pathArchivoCtrlsec))
                            {
                                File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                                loggerIn.Error($"Secuencia del archivo {archivo.Nombre} incorrecta");
                                loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {this.carpetasController.PathCarpetaInNoProcesado}");
                            }
                            else
                            {
                                loggerIn.Info($"Archivo {archivo.Nombre} se mantiene en la carpeta {this.carpetasController.PathCarpetaInPendiente} para su futuro procesamiento");
                            }
                        }
                    }
                }
            }
            loggerIn.Info($"Finalizado el procesamiento de archivos en {this.carpetasController.PathCarpetaIn}");
            MessageBox.Show($"Finalizado el procesamiento de archivos en {this.carpetasController.PathCarpetaIn}");
            btnProcesarArchivosIn.Enabled = true;
        }

        private async void btnProcesarArchivosOut_Click(object sender, EventArgs e)
        {
            btnProcesarArchivosOut.Enabled = false;
            var loggerOut = NLog.LogManager.GetLogger("loggerOut");
            ProcesamientoController procesamientoController = await ProcesamientoController.CreateAsync();
            
            if (procesamientoController.token == String.Empty)
            {
                loggerOut.Error("Error al solicitar token de autenticación");
                MessageBox.Show($"Error de autenticación");
                btnProcesarArchivosOut.Enabled = true;
                return;
            }

            loggerOut.Info($"Iniciado el procesamiento de archivos en {this.carpetasController.PathCarpetaOut} con el método {this.metodoSalida}");

            foreach(var empresa in this.empresas)
            {
                if(!await procesamientoController.procesarArchivosOutAsync(empresa, this.metodoSalida))
                {
                    loggerOut.Error($"Error al procesar los archivos para la empresa {empresa.Id} ({empresa.Nombre})");
                }
            }

            string[] pathsArchivosOut = Directory.GetFiles(this.carpetasController.PathCarpetaOutEnProceso);
            if (pathsArchivosOut.Length > 0)
            {
                loggerOut.Info($"Se generaron {pathsArchivosOut.Length} archivos nuevos en {this.carpetasController.PathCarpetaOutEnProceso}");
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
                loggerOut.Info($"Movidos los archivos a {this.carpetasController.PathCarpetaOutPendiente} y respaldados en {this.carpetasController.PathCarpetaOutBackup}");
                MessageBox.Show($"Finalizado el procesamiento de archivos en {this.carpetasController.PathCarpetaOut}");
            } 
            else
            {
                loggerOut.Info($"No se generaron nuevos archivos en {this.carpetasController.PathCarpetaOutEnProceso}");
                MessageBox.Show($"No se generaron nuevos archivos en {this.carpetasController.PathCarpetaOutEnProceso}");
            }

            loggerOut.Info($"Finalizado el procesamiento de archivos en {this.carpetasController.PathCarpetaOut}");
            btnProcesarArchivosOut.Enabled = true;
        }

    }
}