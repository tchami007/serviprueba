using System.ComponentModel.DataAnnotations;

namespace ServiPrueba.Application.DTOs
{
    public class TransferenciaRequestDTO
    {
        [Required]
        public decimal NumeroCuentaDesde { get; set; }

        [Required]
        public decimal NumeroCuentaHasta { get; set; }

        [Required]
        public decimal Importe { get; set; }

        [Required]
        public DateTime FechaNegocio { get; set; }
    }
}
