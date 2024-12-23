namespace ServiPrueba.Infraestructure.Configurations
{
    // Configuración
    public class ApiClientConfig
    {
        public string BaseUrlCore { get; set; } = string.Empty;
        public string BaseUrlPago { get; set; } = string.Empty;
        public string BaseUrlOnBoarding { get; set; } = string.Empty;
        public int RetryCount { get; set; } = 3;
    }
}
