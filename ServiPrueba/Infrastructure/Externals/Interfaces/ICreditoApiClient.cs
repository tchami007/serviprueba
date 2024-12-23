using ServiPrueba.Infraestructure.Externals.DTOs;
using ServiPrueba.Shared;

namespace ServiPrueba.Infraestructure.Externals.Interfaces
{
    public interface ICreditoApiClient
    {
        Task<Result<object>> EjecutarCreditoAsync(FuncionRequestAsientoDTO request);
        Task<Result<object>> ContrasentarCreditoAsync(FuncionRequestDTO request);
    }
}
