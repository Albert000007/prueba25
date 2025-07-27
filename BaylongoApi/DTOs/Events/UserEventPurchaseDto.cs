namespace BaylongoApi.DTOs.Events
{
    public class UserEventPurchaseDto
    {
        public int EventId { get; set; } // Nuevo: ID del evento comprado

        public string Title { get; set; } // Nuevo: Título del evento

        public string Description { get; set; } // Nuevo: Descripción del evento

        public int? ContentTypeId { get; set; } // Nuevo: ID del tipo de contenido

        public string ContentType { get; set; } // Nuevo: Nombre del tipo de contenido

        public DateTime StartDate { get; set; } // Nuevo: Fecha de inicio del evento

        public DateTime EndDate { get; set; } // Nuevo: Fecha de fin del evento

        public DateTime? ExpirationDate { get; set; } // Nuevo: Fecha de caducidad del evento (para clases y cursos)

        public int? Credits { get; set; } // Nuevo: Número total de créditos comprados (para clases)

        public int? CreditsRemaining { get; set; } // Nuevo: Créditos restantes, calculado restando los créditos usados

        public decimal AmountPaid { get; set; } // Nuevo: Monto pagado por el evento

        public string Currency { get; set; } // Nuevo: Moneda del pago

        public DateTime PurchaseDate { get; set; } // Nuevo: Fecha de compra del evento
    }
}
