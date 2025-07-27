using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Dance;
using BaylongoApi.DTOs.Events;
using BaylongoApi.DTOs.Events.Purchase;
using System.Collections;

namespace BaylongoApi.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventResponseDto> CreateEventAsync(CreateEventDto dto, int userId); // Existente: Crea un nuevo evento
        Task<EventResponseDto> UpdateEventAsync(UpdateEventDto dto); // Existente: Actualiza un evento existente
        Task<bool> DeleteEventAsync(int eventId); // Existente: Elimina un evento
        Task<EventResponseDto> GetEventByIdAsync(int eventId); // Existente: Obtiene un evento por ID
        Task<bool> DeleteEventImageAsync(int imageId); // Existente: Elimina la imagen de un evento
        Task<string> UploadMainImageAsync(UploadMainImageDto dto); // Existente: Sube la imagen principal del evento
        Task<ParticipantDetailDto> AddParticipantAsync(AddParticipantDto dto); // Existente: Añade un participante al evento
        Task<bool> RemoveParticipantAsync(int eventId, int participantId); // Existente: Elimina un participante del evento
        Task<IEnumerable<EventResponseActiveDto>> GetActiveEventsAsync(); // Existente: Obtiene eventos activos
        Task<IEnumerable<EventResponsePastDto>> GetPastEventsAsync(); // Existente: Obtiene eventos pasados
        Task<EventStatusIdResponse> UpdateEventStatusAsync(UpdateEventStatusDto dto, int userId); // Existente: Actualiza el estado del evento
        Task<List<EventRecommendationDto>> GetRecommendedEventsAsync(int userId); // Existente: Obtiene recomendaciones de eventos
        Task<EventDanceTypesResponseDto> UpdateEventDanceTypesAsync(int eventId, UpdateEventDanceTypesDto dto, int userId); // Existente: Actualiza los tipos de baile del evento
        Task<PurchaseResultDto> PurchaseEventAsync(PurchaseEventDto dto); // Nuevo: Procesa la compra de un evento o paquete de clases
        Task<bool> AttendClassAsync(AttendClassDto dto); // Nuevo: Registra la asistencia a una clase y consume un crédito
        Task<IEnumerable<UserEventPurchaseDto>> GetUserEventPurchasesAsync(int userId); // Nuevo: Lista los eventos comprados por un usuario
        Task<string> GeneratePurchaseQRCodeAsync(int eventId, int userId, int eventPurchaseId);
        Task<string> RegeneratePurchaseQRCodeAsync(int eventId, int purchaseId, int userId); 
        Task<IEnumerable> GetUserPurchaseQRCodesAsync(int userId);
    }
}
