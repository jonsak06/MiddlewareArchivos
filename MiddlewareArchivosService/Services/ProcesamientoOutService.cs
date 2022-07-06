using MiddlewareArchivosService.Controllers;
using MiddlewareArchivosService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareArchivosService.Services
{
    public class ProcesamientoOutService
    {
        private readonly NLog.Logger _loggerOut;
        private readonly CarpetasController _carpetasController;
        public ProcesamientoOutService()
        {
            _loggerOut = NLog.LogManager.GetLogger("loggerOut");
            _carpetasController = CarpetasController.Instance;
        }
        public async Task ProcesarArchivosOutAsync(List<Empresa> empresas, ProcesamientoController procesamientoController)
        {
            if (procesamientoController.token == String.Empty)
            {
                _loggerOut.Error("Error al solicitar token de autenticación");
                return;
            }

            _loggerOut.Info($"--- Iniciado el procesamiento de archivos en {_carpetasController.PathCarpetaOut} ---");

            foreach (var empresa in empresas)
            {
                if (!await procesamientoController.procesarArchivosOutAsync(empresa))
                {
                    _loggerOut.Error($"Error en el procesamiento de archivos para la empresa [{empresa.Id}] {empresa.Nombre}");
                }
            }

            string[] pathsArchivosOut = Directory.GetFiles(_carpetasController.PathCarpetaOutEnProceso);
            if (pathsArchivosOut.Length > 0)
            {
                _loggerOut.Info($"Se generaron {pathsArchivosOut.Length} archivos nuevos en {_carpetasController.PathCarpetaOutEnProceso}");
                foreach (string pathArchivo in pathsArchivosOut)
                {
                    string[] splitedPath = pathArchivo.Split("\\");
                    string nombreArchivo = splitedPath[splitedPath.Length - 1];

                    string pathArchivoBackup = $"{_carpetasController.PathCarpetaOutBackup}{nombreArchivo}";
                    if (!File.Exists(pathArchivoBackup))
                        File.Copy(pathArchivo, pathArchivoBackup);
                    else
                        File.Delete(pathArchivo);

                    string pathArchivoPendiente = $"{_carpetasController.PathCarpetaOutPendiente}{nombreArchivo}";
                    if (!File.Exists(pathArchivoPendiente))
                        File.Move(pathArchivo, pathArchivoPendiente);
                    else
                        File.Delete(pathArchivo);
                }
                _loggerOut.Info($"Movidos los archivos a {_carpetasController.PathCarpetaOutPendiente} y respaldados en {_carpetasController.PathCarpetaOutBackup}");
            }
            else
            {
                _loggerOut.Info($"No se generaron nuevos archivos en {_carpetasController.PathCarpetaOutEnProceso}");
            }

            _loggerOut.Info($"--- Finalizado el procesamiento de archivos en {_carpetasController.PathCarpetaOut} ---");
        }
    }
}
