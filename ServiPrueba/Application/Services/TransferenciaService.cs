using ServiPrueba.Application.DTOs;
using ServiPrueba.Infraestructure.Externals.DTOs;
using ServiPrueba.Infraestructure.Externals.Interfaces;
using ServiPrueba.Shared;
using ServiPrueba.Shared.Log;

namespace ServiPrueba.Application.Services
{
    public interface ITransferenciaService
    {
        Task<Result<string>> RealizarTransferenciaAsync(TransferenciaRequestDTO transferenciaRequest);
    }

    public class TransferenciaService : ITransferenciaService
    {
        private readonly IDebitoApiClient _debitoApiClient;
        private readonly ICreditoApiClient _creditoApiClient;
        private readonly IMessagePublisher _logService;

        public TransferenciaService(IDebitoApiClient debitoApiClient, 
            ICreditoApiClient creditoApiClient,
            IMessagePublisher logService)
        {
            _debitoApiClient = debitoApiClient;
            _creditoApiClient = creditoApiClient;
            _logService = logService;   
        }

        public async Task<Result<string>> RealizarTransferenciaAsync(TransferenciaRequestDTO transferenciaRequest)
        {
            // Realizar el débito desde la cuenta origen
            var debitoRequest = new FuncionRequestAsientoDTO
            {
                NumeroCuenta = transferenciaRequest.NumeroCuentaDesde,
                FechaMovimiento = transferenciaRequest.FechaNegocio,
                CodigoMovimiento = 0, // Código para débito
                Importe = transferenciaRequest.Importe
            };

            var debitoResult = await _debitoApiClient.EjecutarDebitoAsync(debitoRequest);
            if (!debitoResult._success)
            {
                return Result<string>.Failure($"Error al debitar: {debitoResult._errorMessage}");
            }

            // Realizar el crédito en la cuenta destino
            var creditoRequest = new FuncionRequestAsientoDTO
            {
                NumeroCuenta = transferenciaRequest.NumeroCuentaHasta,
                FechaMovimiento = transferenciaRequest.FechaNegocio,
                CodigoMovimiento = 1, // Código para crédito
                Importe = transferenciaRequest.Importe
            };

            var creditoResult = await _creditoApiClient.EjecutarCreditoAsync(creditoRequest);
            if (!creditoResult._success)
            {
                // Intentar revertir el débito en caso de error
                var revertirDebitoRequest = new FuncionRequestDTO
                {
                    NumeroCuenta = transferenciaRequest.NumeroCuentaDesde,
                    FechaMovimiento = transferenciaRequest.FechaNegocio,
                    NumeroComprobante = Convert.ToDecimal(debitoResult._value), // Supuesto número de comprobante del débito
                    CodigoMovimiento = 0, // Código para débito
                    Importe = transferenciaRequest.Importe
                };
                await _debitoApiClient.ContrasentarDebitoAsync(revertirDebitoRequest);

                return Result<string>.Failure($"Error al acreditar: {creditoResult._errorMessage}. Débito revertido.");
            }

            _logService.PublishLogAsync("Informacion","Transferencia Realizada",$"Cuenta Desde:{transferenciaRequest.NumeroCuentaDesde} Cuenta Hasta:{transferenciaRequest.NumeroCuentaHasta} Importe:{transferenciaRequest.Importe} Fecha: {transferenciaRequest.FechaNegocio} ");

            return Result<string>.Success("Transferencia realizada con éxito.");
        }
    }
}
