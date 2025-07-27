using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Events
{
    public class PurchaseEventDto
    {
        [Required]
        public int EventId { get; set; } // Nuevo: ID del evento a comprar, requerido

        [Required]
        public int UserId { get; set; } // Nuevo: ID del usuario que realiza la compra, requerido para registrar como participante

        [Required]
        public decimal Amount { get; set; } // Nuevo: Monto de la compra, debe coincidir con el precio base o promocional

        [Required]
        public string Currency { get; set; } // Nuevo: Moneda del pago (por ejemplo, "MXN"), requerida para procesar con Stripe
    }
}
