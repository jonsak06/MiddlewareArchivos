using MiddlewareArchivos.Controllers;
using MiddlewareArchivos.Entities;
using MiddlewareArchivos.Enums;
using MiddlewareArchivos.Providers;
using System.Diagnostics;
using System.Xml.Linq;

namespace MiddlewareArchivos
{
    public partial class Window : Form
    {
        private CarpetasController carpetasController;
        private XMLProvider provider;
        private List<Empresa> empresas;
        public Window()
        {
            InitializeComponent();
            this.carpetasController = new CarpetasController();
            this.provider = new XMLProvider();
            this.empresas = new List<Empresa>();
        }

        private void Window_Load(object sender, EventArgs e)
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

        private void btnCrearCarpetas_Click(object sender, EventArgs e)
        {
            this.carpetasController.crearCarpetas();
            MessageBox.Show("Carpetas creadas");
        }

        private void btnProcesarArchivosIn_Click(object sender, EventArgs e)
        {
            Archivo archivo = new Archivo("acme.1.Producto.prueba1");
            archivo.Contenido = File.ReadAllText($"{carpetasController.PathCarpetaInPendiente}{archivo.Nombre}");
            if (ProcesamientoController.procesarArchivoIn(archivo))
            {
                Debug.WriteLine("archivo procesado");
            }
            else
            {
                Debug.WriteLine("archivo no procesado");
            }
            //Debug.WriteLine(archivo.Contenido);
            //string[] pathsArchivosIn = Directory.GetFiles(this.carpetasController.PathCarpetaInPendiente);
            //LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeCantidadArchivosDetectados(pathsArchivosIn.Length, this.carpetasController.PathCarpetaInPendiente));
            //if (pathsArchivosIn.Length > 0)
            //{
            //    List<Archivo> archivos = new List<Archivo>();
            //    foreach (string path in pathsArchivosIn)
            //    {
            //        string[] splitedPath = path.Split("\\");
            //        string nombreArchivo = splitedPath[splitedPath.Length - 1];
            //        if (nombreArchivo.Split(".").Length == 3 || nombreArchivo.Split(".").Length == 4)
            //        {
            //            archivos.Add(new Archivo(splitedPath[splitedPath.Length - 1]));
            //        }
            //        else
            //        {
            //            //mueve archivos cuyo nombre no cumplen con el formato para procesarlos
            //            File.Move(path, $"{this.carpetasController.PathCarpetaInNoProcesado}{nombreArchivo}");
            //            LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoNoCumpleFormato(nombreArchivo));
            //            LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoMovido(nombreArchivo, this.carpetasController.PathCarpetaInNoProcesado));
            //        }

            //    }

            //    archivos = SecuenciasController.ordenarPorSecuencia(archivos); //los que no tienen secuencia quedan primero (su secuencia devuelve 0)
            //    LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivosOrdenados());


            //    foreach (Archivo archivo in archivos)
            //    {
            //        //carga de las empresas en los archivos
            //        archivo.Empresa = empresas.FirstOrDefault(e => e.Nombre == archivo.NombreEmpresa);

            //        //carga del contenido a los archivos
            //        archivo.Contenido = File.ReadAllText($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}");

            //        if (!archivo.tieneSecuencial() && !archivo.Empresa.ManejaSecuencial)
            //        {
            //            File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}");
            //            LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInEnProceso));

            //            if (ProcesamientoController.procesarArchivoIn(archivo))
            //            {
            //                LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoProcesado(archivo.Nombre));
            //            }
            //            else
            //            {
            //                LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoNoProcesado(archivo.Nombre));
            //            }

            //            File.Move($"{this.carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInProcesado}{archivo.Nombre}");
            //            LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInProcesado));

            //            Console.WriteLine($"{archivo.Nombre} procesado, no tiene secuencia");
            //        }
            //        else if ((!archivo.tieneSecuencial() && archivo.Empresa.ManejaSecuencial) || (archivo.tieneSecuencial() && !archivo.Empresa.ManejaSecuencial))
            //        {
            //            File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
            //            LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoNoCumpleFormato(archivo.Nombre));
            //            LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInNoProcesado));
            //        }
            //        else if (archivo.tieneSecuencial() && archivo.Empresa.ManejaSecuencial)
            //        {
            //            if (SecuenciasController.secuenciaCorrecta(this.carpetasController.PathCarpetaCtrl, archivo))
            //            {
            //                File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}");
            //                LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInEnProceso));

            //                if (ProcesamientoController.procesarArchivoIn(archivo))
            //                {
            //                    LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoProcesado(archivo.Nombre));
            //                }
            //                else
            //                {
            //                    LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoNoProcesado(archivo.Nombre));
            //                }

            //                File.Move($"{this.carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInProcesado}{archivo.Nombre}");
            //                LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInProcesado));

            //                string pathArchivoCtrlsec = SecuenciasController.getPathArchivoCtrlsec(this.carpetasController.PathCarpetaCtrl, archivo.NombreEmpresa);
            //                SecuenciasController.aumentarSecuencia(pathArchivoCtrlsec);// aumenta 1 en la ultima secuencia procesada (archivo ctrlsec)
            //                LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeSecuenciaAumentada(archivo.Secuencia, pathArchivoCtrlsec));

            //                Console.WriteLine($"{archivo.Nombre} procesado, tiene secuencia");
            //            }
            //            else
            //            {
            //                string pathArchivoCtrlsec = $"{this.carpetasController.PathCarpetaCtrl}{archivo.NombreEmpresa}.ctrlsec";
            //                if (archivo.Secuencia <= SecuenciasController.getUltimaSecuenciaProcesada(pathArchivoCtrlsec) ||
            //                    archivo.Secuencia > SecuenciasController.getSecuenciaFinal(pathArchivoCtrlsec))
            //                {
            //                    File.Move($"{this.carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{this.carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
            //                    LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeSecuenciaIncorrecta(archivo.Nombre));
            //                    LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoMovido(archivo.Nombre, this.carpetasController.PathCarpetaInNoProcesado));
            //                }
            //                else
            //                {
            //                    LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeArchivoPendiente(archivo.Nombre, this.carpetasController.PathCarpetaInPendiente));
            //                }
            //            }
            //        }
            //    }
            //}
            //LogsController.escribirEnLog(this.carpetasController.PathCarpetaInLog, LogsController.mensajeSeparador());
            //MessageBox.Show($"Finalizado el procesamiento de archivos de IN");

        }

        private void btnProcesarArchivosOut_Click(object sender, EventArgs e)
        {
            string[] pathsArchivosOut = Directory.GetFiles(this.carpetasController.PathCarpetaOutEnProceso);
            foreach (string path in pathsArchivosOut)
            {
                string[] splitedPath = path.Split("\\");
                string nombreArchivo = splitedPath[splitedPath.Length - 1];
                File.Copy(path, $"{this.carpetasController.PathCarpetaOutBackup}{nombreArchivo}");
                File.Move(path, $"{this.carpetasController.PathCarpetaOutPendiente}{nombreArchivo}");
                MessageBox.Show($"Finalizado el procesamiento de archivos de OUT");
            }
        }

    }
}