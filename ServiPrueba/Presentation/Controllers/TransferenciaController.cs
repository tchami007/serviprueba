using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiPrueba.Application.DTOs;
using ServiPrueba.Application.Services;

namespace ServiPrueba.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferenciaController : ControllerBase
    {
        private readonly ITransferenciaService _transferenciaService;

        public TransferenciaController(ITransferenciaService transferenciaService)
        {
            _transferenciaService = transferenciaService;
        }

        /// <summary>
        /// RealizarTransferencia: Realiza una transferencia entre dos cuentas.
        /// </summary>
        /// <param name="transferenciaRequest">DTO con los detalles de la transferencia.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost("Transferencia")]
        public async Task<ActionResult> RealizarTransferencia([FromBody] TransferenciaRequestDTO transferenciaRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _transferenciaService.RealizarTransferenciaAsync(transferenciaRequest);

            if (!result._success)
            {
                return BadRequest(new
                {
                    Mensaje = "Error al realizar la transferencia",
                    Detalle = result._errorMessage
                });
            }

            return Ok(new
            {
                Mensaje = result._value
            });
        }
    }
}
