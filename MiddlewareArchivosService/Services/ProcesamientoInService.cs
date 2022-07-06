using MiddlewareArchivosService.Controllers;
using MiddlewareArchivosService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareArchivosService.Services
{
    public class ProcesamientoInService
    {
        private readonly CarpetasController _carpetasController;
        private readonly NLog.Logger _loggerIn;
        public ProcesamientoInService()
        {
            _carpetasController = CarpetasController.Instance;
            _loggerIn = NLog.LogManager.GetLogger("loggerIn");
        }
        public async Task ProcesarArchivosInAsync(List<Empresa> empresas)
        {
            ProcesamientoController _procesamientoController = await ProcesamientoController.CreateAsync();
            string pathCarpetaInLog = _carpetasController.PathCarpetaInLog;

            if (_procesamientoController.token == String.Empty)
            {
                _loggerIn.Error("Error al solicitar token de autenticación");
                return;
            }

            _loggerIn.Info($"\nIniciado el procesamiento de archivos en {_carpetasController.PathCarpetaIn}");

            string[] pathsArchivosIn = Directory.GetFiles(_carpetasController.PathCarpetaInPendiente);
            _loggerIn.Info($"{pathsArchivosIn.Length} archivos detectados en la carpeta {_carpetasController.PathCarpetaInPendiente}");
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
                        File.Move(path, $"{_carpetasController.PathCarpetaInNoProcesado}{nombreArchivo}");
                        _loggerIn.Info($"Nombre de archivo {nombreArchivo} no cumple con el formato necesario para su procesamiento");
                        _loggerIn.Info($"Movido el archivo {nombreArchivo} a la carpeta {_carpetasController.PathCarpetaInNoProcesado}");
                    }
                }

                archivos = SecuenciasController.ordenarPorSecuencia(archivos);
                _loggerIn.Info("Archivos ordenados por secuencia para su procesamiento");


                foreach (Archivo archivo in archivos)
                {
                    //carga de las empresas en los archivos
                    archivo.Empresa = empresas.FirstOrDefault(e => e.Nombre == archivo.NombreEmpresa);

                    //carga del contenido a los archivos
                    archivo.Contenido = File.ReadAllText($"{_carpetasController.PathCarpetaInPendiente}{archivo.Nombre}");

                    if (archivo.Empresa == null)
                    {
                        _loggerIn.Error($"Empresa {archivo.NombreEmpresa} no existe en el registro");
                        File.Move($"{_carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{_carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                        _loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {_carpetasController.PathCarpetaInNoProcesado}");
                    }
                    else if (!InterfacesController.existeInterfaz(archivo.Api))
                    {
                        _loggerIn.Error($"Interfaz {archivo.Api} no existe en el registro");
                        File.Move($"{_carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{_carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                        _loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {_carpetasController.PathCarpetaInNoProcesado}");
                    }
                    else if (!_procesamientoController.empresaCorrecta(archivo))//si id empresa del nombre del archivo != id empresa del contenido
                    {
                        _loggerIn.Error($"Código de la empresa {archivo.NombreEmpresa} no corresponde con el declarado en el contenido del archivo");
                        File.Move($"{_carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{_carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                        _loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {_carpetasController.PathCarpetaInNoProcesado}");
                    }
                    else if (!archivo.tieneSecuencial() && !archivo.Empresa.ManejaSecuencial)
                    {
                        File.Move($"{_carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{_carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}");
                        _loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {_carpetasController.PathCarpetaInEnProceso}");

                        var resultado = await _procesamientoController.procesarArchivoInAsync(archivo);
                        if (resultado.Key)
                        {
                            _loggerIn.Info($"Procesado el archivo {archivo.Nombre} exitosamente");
                            File.Move($"{_carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}", $"{_carpetasController.PathCarpetaInProcesado}{resultado.Value}.{archivo.Nombre}");
                        }
                        else
                        {
                            _loggerIn.Error($"Archivo {archivo.Nombre} no se pudo procesar correctamente, generado {archivo.Nombre}.err");
                            File.Move($"{_carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}", $"{_carpetasController.PathCarpetaInProcesado}{archivo.Nombre}");
                        }

                        _loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {_carpetasController.PathCarpetaInProcesado}");

                    }
                    else if ((!archivo.tieneSecuencial() && archivo.Empresa.ManejaSecuencial) || (archivo.tieneSecuencial() && !archivo.Empresa.ManejaSecuencial))
                    {
                        File.Move($"{_carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{_carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                        _loggerIn.Error($"Nombre de archivo {archivo.Nombre} no cumple con el formato necesario para su procesamiento");
                        _loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {_carpetasController.PathCarpetaInNoProcesado}");
                    }
                    else if (archivo.tieneSecuencial() && archivo.Empresa.ManejaSecuencial)
                    {
                        if (SecuenciasController.secuenciaCorrecta(_carpetasController.PathCarpetaCtrl, archivo))
                        {
                            File.Move($"{_carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{_carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}");
                            _loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {_carpetasController.PathCarpetaInEnProceso}");

                            var resultado = await _procesamientoController.procesarArchivoInAsync(archivo);
                            if (resultado.Key)
                            {
                                _loggerIn.Info($"Procesado el archivo {archivo.Nombre} exitosamente");
                                File.Move($"{_carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}", $"{_carpetasController.PathCarpetaInProcesado}{resultado.Value}.{archivo.Nombre}");
                            }
                            else
                            {
                                _loggerIn.Error($"Archivo {archivo.Nombre} no se pudo procesar correctamente, generado {archivo.Nombre}.err");
                                File.Move($"{_carpetasController.PathCarpetaInEnProceso}{archivo.Nombre}", $"{_carpetasController.PathCarpetaInProcesado}{archivo.Nombre}");
                            }

                            _loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {_carpetasController.PathCarpetaInProcesado}");

                            string pathArchivoCtrlsec = SecuenciasController.getPathArchivoCtrlsec(_carpetasController.PathCarpetaCtrl, archivo.NombreEmpresa);
                            SecuenciasController.aumentarSecuencia(pathArchivoCtrlsec);// aumenta 1 en la ultima secuencia procesada (en archivo ctrlsec)
                            _loggerIn.Info($"Aumentado a {archivo.Secuencia} la última secuencia procesada en {pathArchivoCtrlsec}");
                        }
                        else
                        {
                            string pathArchivoCtrlsec = SecuenciasController.getPathArchivoCtrlsec(_carpetasController.PathCarpetaCtrl, archivo.NombreEmpresa);
                            if (archivo.Secuencia <= SecuenciasController.getUltimaSecuenciaProcesada(pathArchivoCtrlsec) ||
                                archivo.Secuencia > SecuenciasController.getSecuenciaFinal(pathArchivoCtrlsec))
                            {
                                File.Move($"{_carpetasController.PathCarpetaInPendiente}{archivo.Nombre}", $"{_carpetasController.PathCarpetaInNoProcesado}{archivo.Nombre}");
                                _loggerIn.Error($"Secuencia del archivo {archivo.Nombre} incorrecta");
                                _loggerIn.Info($"Movido el archivo {archivo.Nombre} a la carpeta {_carpetasController.PathCarpetaInNoProcesado}");
                            }
                            else
                            {
                                _loggerIn.Info($"Archivo {archivo.Nombre} se mantiene en la carpeta {_carpetasController.PathCarpetaInPendiente} para su futuro procesamiento");
                            }
                        }
                    }
                }
            }
            _loggerIn.Info($"\nFinalizado el procesamiento de archivos en {_carpetasController.PathCarpetaIn}\n");
        }
    }
}
