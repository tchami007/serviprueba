using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly;
using ServiPrueba.Infraestructure.Externals.DTOs;
using ServiPrueba.Infraestructure.Externals.Interfaces;
using ServiPrueba.Shared;
using ServiPrueba.Infraestructure.Configurations;

namespace ServiPrueba.Infraestructure.Externals.ApiClientCore
{
    // Implementaciones
    public class CreditoApiClient : ICreditoApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;

        public CreditoApiClient(HttpClient httpClient, IOptions<ApiClientConfig> config)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(config.Value.BaseUrlCore);

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(config.Value.RetryCount, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task<Result<object>> EjecutarCreditoAsync(FuncionRequestAsientoDTO request)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync("api/FuncionCredito/Ejecutar", request);
                return await HandleResponse(response);
            });
        }

        public async Task<Result<object>> ContrasentarCreditoAsync(FuncionRequestDTO request)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync("api/FuncionCredito/Contrasentar", request);
                return await HandleResponse(response);
            });
        }

        private static async Task<Result<object>> HandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<object>();
                return Result<object>.Success(result);
            }

            var error = await response.Content.ReadAsStringAsync();
            return Result<object>.Failure(error);
        }
    }

}
