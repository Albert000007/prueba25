using BaylongoApi.DTOs.Dance;
using BaylongoApi.DTOs.Events;
using BaylongoApi.Services;
using BaylongoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaylongoApi.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController(IEventService eventService, ILogger<EventsController> logger) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto) // Existente: Crea un nuevo evento
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await eventService.CreateEventAsync(dto, userId);
            return CreatedAtAction(nameof(GetEventById), new { eventId = result.EventId }, result);
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventById(int eventId) // Existente: Obtiene un evento por ID
        {
            var result = await eventService.GetEventByIdAsync(eventId);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveEvents() // Existente: Obtiene eventos activos
        {
            var result = await eventService.GetActiveEventsAsync();
            return Ok(result);
        }

        [HttpGet("past")]
        public async Task<IActionResult> GetPastEvents() // Existente: Obtiene eventos pasados
        {
            var result = await eventService.GetPastEventsAsync();
            return Ok(result);
        }

        [HttpPut("public-event")]
        [Authorize]
        public async Task<IActionResult> UpdateEventStatus([FromBody] UpdateEventStatusDto dto) // Existente: Actualiza el estado del evento
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await eventService.UpdateEventStatusAsync(dto, userId);
            return Ok(result);
        }

        [HttpPost("{eventId}/main-image")]
        [Authorize]
        public async Task<IActionResult> UploadMainImage([FromForm] UploadMainImageDto dto) // Existente: Sube la imagen principal del evento
        {
            var result = await eventService.UploadMainImageAsync(dto);
            return Ok(new { ImageUrl = result });
        }

        [HttpPost("add-participant")]
        [Authorize]
        public async Task<IActionResult> AddParticipant([FromForm] AddParticipantDto dto) // Existente: Añade un participante al evento
        {
            var result = await eventService.AddParticipantAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{eventId}/participants/{participantId}")]
        [Authorize]
        public async Task<IActionResult> RemoveParticipant(int eventId, int participantId) // Existente: Elimina un participante del evento
        {
            var result = await eventService.RemoveParticipantAsync(eventId, participantId);
            return Ok(result);
        }

        [HttpGet("recommendations")]
        [Authorize]
        public async Task<IActionResult> GetRecommendedEvents() // Existente: Obtiene recomendaciones de eventos
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await eventService.GetRecommendedEventsAsync(userId);
            return Ok(result);
        }

        [HttpPut("{eventId}/dance-types")]
        [Authorize]
        public async Task<IActionResult> UpdateEventDanceTypes(int eventId, [FromBody] UpdateEventDanceTypesDto dto) // Existente: Actualiza los tipos de baile
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await eventService.UpdateEventDanceTypesAsync(eventId, dto, userId);
            return Ok(result);
        }

        [HttpPost("{eventId}/purchase")]
        [Authorize]
        public async Task<IActionResult> PurchaseEvent(int eventId, [FromBody] PurchaseEventDto dto)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return Unauthorized("Debes estar logueado para realizar una compra");

            if (eventId != dto.EventId)
                return BadRequest("El ID del evento en la URL no coincide con el DTO");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("No se pudo identificar al usuario");

            var userId = int.Parse(userIdClaim);
            dto.UserId = userId;
            var purchaseResult = await eventService.PurchaseEventAsync(dto);

            var eventDetails = await eventService.GetEventByIdAsync(eventId);
            var qrCodeUrl = purchaseResult.QRCodeUrl; // Usar la URL generada en PurchaseEventAsync

            var purchaseResponse = new
            {
                PurchaseSuccess = true,
                Event = eventDetails,
                UserId = userId,
                UserName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuario Anónimo",
                OrganizationId = eventDetails.OrganizationId,
                OrganizationName = eventDetails.OrganizationName,
                QRCodeUrl = qrCodeUrl
            };

            return Ok(purchaseResponse);
        }
        [HttpPost("{eventId}/purchase/{purchaseId}/regenerate-qr")]
        [Authorize]
        public async Task<IActionResult> RegeneratePurchaseQRCode(int eventId, int purchaseId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("No se pudo identificar al usuario");

            var userId = int.Parse(userIdClaim);
            var qrCodeUrl = await eventService.RegeneratePurchaseQRCodeAsync(eventId, purchaseId, userId);
            return Ok(new { QRCodeUrl = qrCodeUrl });
        }
        [HttpGet("user/{userId}/purchase-qrs")]
        [Authorize]
        public async Task<IActionResult> GetUserPurchaseQRCodes(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != currentUserId)
                return Unauthorized("No tienes permiso para ver los códigos QR de otro usuario");

            var qrCodes = await eventService.GetUserPurchaseQRCodesAsync(userId);
            return Ok(qrCodes);
        }

        [HttpPost("{eventId}/attend-class")]
        [Authorize]
        public async Task<IActionResult> AttendClass(int eventId, [FromBody] AttendClassDto dto) // Nuevo: Registra la asistencia a una clase
        {
            if (eventId != dto.EventId)
                return BadRequest("El ID del evento en la URL no coincide con el DTO");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            dto.UserId = userId;
            var result = await eventService.AttendClassAsync(dto);
            return Ok(result);
        }

        [HttpGet("user/{userId}/purchases")]
        [Authorize]
        public async Task<IActionResult> GetUserEventPurchases(int userId) // Nuevo: Lista los eventos comprados por un usuario
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != currentUserId)
                return Unauthorized("No tienes permiso para ver las compras de otro usuario");

            var result = await eventService.GetUserEventPurchasesAsync(userId);
            return Ok(result);
        }
    }
}