using Microsoft.Extensions.Options;
using ServiPrueba.Infraestructure.Externals.Interfaces;
using ServiPrueba.Infraestructure.Externals.DTOs;
using ServiPrueba.Shared;
using Polly.Retry;
using Polly;
using ServiPrueba.Infraestructure.Configurations;

namespace ServiPrueba.Infraestructure.Externals.ApiClientCore
{
    public class DebitoApiClient : IDebitoApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;

        public DebitoApiClient(HttpClient httpClient, IOptions<ApiClientConfig> config)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(config.Value.BaseUrlCore);

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(config.Value.RetryCount, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task<Result<object>> EjecutarDebitoAsync(FuncionRequestAsientoDTO request)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync("api/FuncionDebito/Ejecutar", request);
                return await HandleResponse(response);
            });
        }

        public async Task<Result<object>> ContrasentarDebitoAsync(FuncionRequestDTO request)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync("api/FuncionDebito/Contrasentar", request);
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
