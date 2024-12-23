using ServiPrueba.Infraestructure.Externals.DTOs;
using ServiPrueba.Shared;

namespace ServiPrueba.Infraestructure.Externals.Interfaces
{
    public interface IDebitoApiClient
    {
        Task<Result<object>> EjecutarDebitoAsync(FuncionRequestAsientoDTO request);
        Task<Result<object>> ContrasentarDebitoAsync(FuncionRequestDTO request);
    }
}
