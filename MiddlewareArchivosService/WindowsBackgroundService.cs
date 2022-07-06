using MiddlewareArchivosService.Entities;
using MiddlewareArchivosService.Services;

namespace MiddlewareArchivosService
{
    public class WindowsBackgroundService : BackgroundService
    {
        private readonly ProcesamientoInService _procesamientoInService;
        ProcesamientoOutService _procesamientoOutService;
        ConfiguracionEmpresasService _configuracionEmpresasService;
        private readonly ILogger<WindowsBackgroundService> _logger;
        private List<Empresa> empresas;
        private readonly int _intervaloEnMinutos;

        public WindowsBackgroundService(ProcesamientoInService procesamientoInService,
                                ProcesamientoOutService procesamientoOutService, ConfiguracionEmpresasService configuracionEmpresasService,
                                ILogger<WindowsBackgroundService> logger) {
            _procesamientoInService = procesamientoInService;
            _procesamientoOutService = procesamientoOutService;
            _configuracionEmpresasService = configuracionEmpresasService;
            _logger = logger;
            _intervaloEnMinutos = int.Parse(System.Configuration.ConfigurationManager.AppSettings["IntervaloProcesamiento"]);
        }
            

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    this.empresas = _configuracionEmpresasService.GetEmpresas();
                    await _procesamientoInService.ProcesarArchivosInAsync(this.empresas);
                    await _procesamientoOutService.ProcesarArchivosOutAsync(this.empresas);
                    _logger.LogInformation("Procesamiento finalizado");

                    await Task.Delay(TimeSpan.FromMinutes(_intervaloEnMinutos), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }
        }
    }
}