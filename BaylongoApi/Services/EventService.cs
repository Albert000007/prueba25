using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Dance;
using BaylongoApi.DTOs.Events;
using BaylongoApi.DTOs.Events.Purchase;
using BaylongoApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Imaging;
using QRCoder;
using System.Collections;

namespace BaylongoApi.Services
{
    public class EventService(BaylongoContext context, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor) : IEventService
    {
        public async Task<EventResponseDto> CreateEventAsync(CreateEventDto dto, int userId)
        {
            // Método: Crea un nuevo evento tras validar organización, permisos, fechas, precios, créditos y capacidad
            var organization = await context.Organizations
                .FirstOrDefaultAsync(o => o.OrganizationId == dto.OrganizationId && o.UserId == userId);
            if (organization == null)
                throw new UnauthorizedAccessException("No tienes permisos para crear eventos en esta organización");

            var contentPermission = await context.OrganizationContentPermissions
                .AnyAsync(ocp => ocp.OrganizationId == dto.OrganizationId && ocp.ContentTypeId == dto.ContentTypeId);
            if (!contentPermission)
                throw new UnauthorizedAccessException("La organización no tiene permisos para crear este tipo de contenido");

            var paymentMethodExists = await context.PaymentMethods
                .AnyAsync(p => p.PaymentMethodId == dto.PaymentMethodId);
            if (!paymentMethodExists)
                throw new ArgumentException("El método de pago especificado no existe");

            var contentTypeExists = await context.ContentTypes
                .AnyAsync(ct => ct.ContentTypeId == dto.ContentTypeId);
            if (!contentTypeExists)
                throw new ArgumentException("El tipo de contenido especificado no existe");

            if (dto.StartDate >= dto.EndDate)
                throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de finalización");

            if (dto.PromoStartDate.HasValue && dto.PromoEndDate.HasValue &&
                dto.PromoStartDate >= dto.PromoEndDate)
                throw new ArgumentException("Las fechas promocionales no son válidas");

            if (dto.ExpirationDate.HasValue && dto.ExpirationDate <= dto.StartDate)
                throw new ArgumentException("La fecha de caducidad debe ser posterior a la fecha de inicio");

            if (dto.BasePrice.HasValue && dto.BasePrice <= 0)
                throw new ArgumentException("El precio base debe ser mayor que cero");

            if (dto.PromotionalPrice.HasValue && dto.PromotionalPrice >= dto.BasePrice)
                throw new ArgumentException("El precio promocional debe ser menor al precio base");

            if (dto.ContentTypeId == 2 && (!dto.Credits.HasValue || dto.Credits <= 0)) // Suponiendo ContentTypeId 2 = Clase
                throw new ArgumentException("Las clases deben tener un número de créditos mayor a cero");

            if (dto.Capacity.HasValue && dto.Capacity <= 0)
                throw new ArgumentException("La capacidad debe ser mayor que cero");

            var newEvent = new Event
            {
                OrganizationId = dto.OrganizationId,
                StripeAccountId = dto.StripeAccountId,
                UserId = userId,
                EventStatusId = 1, // Borrador
                ContentTypeId = dto.ContentTypeId,
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate.ToUniversalTime(),
                EndDate = dto.EndDate.ToUniversalTime(),
                Capacity = dto.Capacity,
                ExpirationDate = dto.ExpirationDate?.ToUniversalTime(),
                Credits = dto.Credits,
                BasePrice = dto.BasePrice,
                PromotionalPrice = dto.PromotionalPrice,
                PromoStartDate = dto.PromoStartDate?.ToUniversalTime(),
                PromoEndDate = dto.PromoEndDate?.ToUniversalTime(),
                PaymentMethodId = dto.PaymentMethodId,
                PaymentLink = dto.PaymentLink,
                Location = dto.Location,
                Address = dto.Address,
                Country = dto.Country,
                CityId = dto.CityId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Events.AddAsync(newEvent);
            await context.SaveChangesAsync();

            return await GetEventResponse(newEvent.EventId);
        }

        public async Task<PurchaseResultDto> PurchaseEventAsync(PurchaseEventDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.EventId <= 0) throw new ArgumentException("EventId inválido", nameof(dto.EventId));
            if (dto.Amount <= 0) throw new ArgumentException("Monto inválido", nameof(dto.Amount));
            if (dto.UserId <= 0) throw new ArgumentException("UserId inválido", nameof(dto.UserId));

            var eventEntity = await context.Events
                .Include(e => e.Organization)
                .FirstOrDefaultAsync(e => e.EventId == dto.EventId);

            if (eventEntity == null)
                throw new ArgumentException("Evento no encontrado", nameof(dto.EventId));

            if (eventEntity.EventStatusId != 2)
                throw new ArgumentException("El evento no está publicado", nameof(dto.EventId));

            // Registrar la compra
            var purchase = new EventPurchase
            {
                EventId = dto.EventId,
                UserId = dto.UserId,
                Amount = dto.Amount,
                PurchaseDate = DateTime.UtcNow,
                StatusId = 1 // Completada
            };

            context.EventPurchases.Add(purchase);
            await context.SaveChangesAsync();

            // Generar código QR
            var token = Guid.NewGuid().ToString();
            var expiresAt = DateTime.UtcNow.AddHours(8);

            var request = httpContextAccessor.HttpContext.Request;
            var verificationUrl = $"{request.Scheme}://{request.Host}/api/events/verify-purchase/{token}";

            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(verificationUrl, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);

            var uploadsPath = Path.Combine(environment.WebRootPath, "Uploads", "qrcodes");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{token}.png";
            var filePath = Path.Combine(uploadsPath, fileName);
            await File.WriteAllBytesAsync(filePath, qrCodeBytes);

            var qrCodeUrl = $"{request.Scheme}://{request.Host}/Uploads/qrcodes/{fileName}";

            // Guardar token en la base de datos
            var purchaseToken = new PurchaseToken
            {
                EventPurchaseId = purchase.PurchaseId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            context.PurchaseTokens.Add(purchaseToken);
            await context.SaveChangesAsync();

            // Obtener datos completos del evento
            var eventResponse = await GetEventResponse(eventEntity.EventId);

            return new PurchaseResultDto
            {
                PurchaseId = purchase.PurchaseId,
                QRCodeUrl = qrCodeUrl,
                QRExpiresAt = expiresAt,
                Event = eventResponse
            };
        }

        public async Task<bool> AttendClassAsync(AttendClassDto dto)
        {
            // Método: Registra la asistencia a una clase validando que sea una clase, caducidad, inscripción y créditos disponibles
            var currentDate = DateTime.UtcNow;

            var eventEntity = await context.Events
                .FirstOrDefaultAsync(e => e.EventId == dto.EventId);
            if (eventEntity == null)
                throw new KeyNotFoundException("Evento no encontrado");

            if (eventEntity.ContentTypeId != 2) // Suponiendo ContentTypeId 2 = Clase
                throw new InvalidOperationException("El evento no es una clase");

            if (eventEntity.ExpirationDate.HasValue && eventEntity.ExpirationDate < currentDate)
                throw new InvalidOperationException("El paquete de clases ha caducado");

            var participant = await context.EventParticipants
                .FirstOrDefaultAsync(ep => ep.EventId == dto.EventId && ep.UserId == dto.UserId && ep.RoleId == 4);
            if (participant == null)
                throw new InvalidOperationException("El usuario no está inscrito en este paquete de clases");

            // Calcular créditos usados, manejar caso donde no haya registros
            var creditsUsed = await context.ClassAttendances
                .Where(ca => ca.EventId == dto.EventId && ca.UserId == dto.UserId)
                .SumAsync(ca => ca.CreditsUsed);
            var creditsRemaining = eventEntity.Credits - creditsUsed;
            if (creditsRemaining <= 0)
                throw new InvalidOperationException("No hay créditos disponibles para este paquete de clases");

            if (dto.AttendanceDate.Date != currentDate.Date)
                throw new InvalidOperationException("La fecha de asistencia debe ser el día actual");

            var attendance = new ClassAttendance
            {
                EventId = dto.EventId,
                UserId = dto.UserId,
                AttendanceDate = dto.AttendanceDate.ToUniversalTime(),
                CreditsUsed = 1,
                CreatedAt = DateTime.UtcNow
            };
            await context.ClassAttendances.AddAsync(attendance);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<UserEventPurchaseDto>> GetUserEventPurchasesAsync(int userId)
        {
            // Verificar si el usuario existe
            var userExists = await context.Users.AnyAsync(u => u.UserId == userId);
            if (!userExists)
                throw new Exception($"El usuario con no existe");

            // Consultar EventPurchases
            var purchases = await context.EventPurchases
                .Include(ep => ep.Event)
                    .ThenInclude(e => e.ContentType)
                .Include(ep => ep.Event)
                    .ThenInclude(e => e.Payments)
                .Where(ep => ep.UserId == userId)
                .ToListAsync();

            // Mapear a UserEventPurchaseDto
            return purchases.Select(ep => new UserEventPurchaseDto
            {
                EventId = ep.EventId,
                Title = ep.Event.Title,
                Description = ep.Event.Description,
                ContentTypeId = ep.Event.ContentTypeId,
                ContentType = ep.Event.ContentType != null ? ep.Event.ContentType.Name : "N/A",
                StartDate = ep.Event.StartDate,
                EndDate = ep.Event.EndDate,
                ExpirationDate = ep.Event.ExpirationDate,
                Credits = ep.Event.Credits,
                CreditsRemaining = ep.Event.Credits - context.ClassAttendances
                    .Where(ca => ca.EventId == ep.EventId && ca.UserId == userId)
                    .Sum(ca => ca.CreditsUsed),
                AmountPaid = ep.Event.Payments
                    .Where(p => p.UserId == userId)
                    .Sum(p => p.Amount),
                Currency = ep.Event.Payments
                    .FirstOrDefault(p => p.UserId == userId)?.Currency,
                PurchaseDate = ep.PurchaseDate // Asumiendo que EventPurchases tiene un campo CreatedAt
            }).ToList();
        }

        private async Task<EventResponseDto> GetEventResponse(int eventId)
        {
            // Método: Obtiene los detalles de un evento incluyendo participantes y tipos de baile
            var eventData = await context.Events
                .Include(e => e.Organization)
                .Include(e => e.EventStatus)
                .Include(e => e.PaymentMethod)
                .Include(e => e.ContentType)
                .Include(e => e.EventParticipants)
                    .ThenInclude(ep => ep.Participant)
                        .ThenInclude(p => p.ParticipantType)
                .Include(e => e.EventParticipants)
                    .ThenInclude(ep => ep.Role)
                .Include(e => e.EventDanceTypes)
                    .ThenInclude(edt => edt.DanceType)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventData == null)
                throw new KeyNotFoundException("Evento no encontrado");

            return new EventResponseDto
            {
                EventId = eventData.EventId,
                MainImageUrl = eventData.MainImageUrl,
                OrganizationId = eventData.OrganizationId,
                OrganizationName = eventData.Organization.Name,
                Title = eventData.Title,
                Description = eventData.Description,
                StartDate = eventData.StartDate,
                EndDate = eventData.EndDate,
                ContentTypeId = eventData.ContentTypeId,
                ContentType = eventData.ContentType?.Name,
                Capacity = eventData.Capacity,
                ExpirationDate = eventData.ExpirationDate,
                Credits = eventData.Credits,
                BasePrice = eventData.BasePrice,
                PromotionalPrice = eventData.PromotionalPrice,
                PromoStartDate = eventData.PromoStartDate,
                PromoEndDate = eventData.PromoEndDate,
                PaymentMethodId = eventData.PaymentMethodId,
                PaymentMethod = eventData.PaymentMethod?.Name,
                PaymentLink = eventData.PaymentLink,
                Location = eventData.Location,
                Address = eventData.Address,
                Country = eventData.Country,
                CityId = eventData.CityId,
                EventStatusId = eventData.EventStatusId,
                EventStatus = eventData.EventStatus?.Name,
                Participants = eventData.EventParticipants.Select(ep => new ParticipantDetailDto
                {
                    ParticipantId = ep.ParticipantId,
                    Name = ep.Participant.Name,
                    Description = ep.Participant.Description,
                    PhotoUrl = ep.Participant.LogoUrl,
                    Type = ep.Participant.ParticipantType?.Name,
                    Role = ep.Role?.Name
                }).ToList(),
                CreatedAt = eventData.CreatedAt,
                UpdatedAt = eventData.UpdatedAt,
                DanceTypes = eventData.EventDanceTypes.Select(edt => new DanceTypeDto
                {
                    DanceTypeId = edt.DanceType.DanceTypeId,
                    Name = edt.DanceType.Name,
                    Description = edt.DanceType.Description,
                    IsPrimary = edt.IsPrimary
                }).ToList()
            };
        }

        public async Task<IEnumerable<EventResponseActiveDto>> GetActiveEventsAsync()
        {
            // Método: Obtiene la lista de eventos activos basados en la fecha actual y estado publicado
            var currentDate = DateTime.UtcNow;

            var activeEvents = await context.Events
                .Include(e => e.Organization)
                .Include(e => e.EventStatus)
                .Include(e => e.PaymentMethod)
                .Include(e => e.ContentType)
                .Include(e => e.EventParticipants)
                    .ThenInclude(ep => ep.Participant)
                        .ThenInclude(p => p.ParticipantType)
                .Include(e => e.EventParticipants)
                    .ThenInclude(ep => ep.Role)
                .Where(e => e.StartDate <= currentDate && e.EndDate >= currentDate && e.EventStatusId == 2)
                .ToListAsync();

            return activeEvents.Select(e => new EventResponseActiveDto
            {
                EventId = e.EventId,
                MainImageUrl = e.MainImageUrl,
                OrganizationId = e.OrganizationId,
                OrganizationName = e.Organization.Name,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                ContentTypeId = e.ContentTypeId,
                ContentType = e.ContentType?.Name,
                Capacity = e.Capacity,
                ExpirationDate = e.ExpirationDate,
                Credits = e.Credits,
                BasePrice = (decimal)e.BasePrice,
                PromotionalPrice = e.PromotionalPrice,
                PromoStartDate = e.PromoStartDate,
                PromoEndDate = e.PromoEndDate,
                PaymentMethodId = e.PaymentMethodId,
                PaymentMethod = e.PaymentMethod?.Name,
                PaymentLink = e.PaymentLink,
                Location = e.Location,
                Address = e.Address,
                Country = e.Country,
                EventStatusId = e.EventStatusId,
                EventStatus = e.EventStatus?.Name,
                Participants = e.EventParticipants.Select(ep => new ParticipantDetailDto
                {
                    ParticipantId = ep.ParticipantId,
                    Name = ep.Participant.Name,
                    Description = ep.Participant.Description,
                    PhotoUrl = ep.Participant.LogoUrl,
                    Type = ep.Participant.ParticipantType?.Name,
                    Role = ep.Role?.Name
                }).ToList(),
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                Price = (e.PromoStartDate <= currentDate && currentDate <= e.PromoEndDate && e.PromotionalPrice.HasValue)
                    ? e.PromotionalPrice.Value
                    : e.BasePrice
            }).ToList();
        }

        public Task<EventResponseDto> UpdateEventAsync(UpdateEventDto dto)
        {
            // Método: Placeholder para actualizar un evento (no implementado)
            throw new NotImplementedException();
        }

        public Task<bool> DeleteEventAsync(int eventId)
        {
            // Método: Placeholder para eliminar un evento (no implementado)
            throw new NotImplementedException();
        }

        public Task<bool> DeleteEventImageAsync(int imageId)
        {
            // Método: Placeholder para eliminar la imagen de un evento (no implementado)
            throw new NotImplementedException();
        }

        public async Task<string> UploadMainImageAsync(UploadMainImageDto dto)
        {
            // Método: Sube una imagen principal para un evento y actualiza la URL en la base de datos
            var existingEvent = await context.Events.FindAsync(dto.EventId);
            if (existingEvent == null)
                throw new KeyNotFoundException("Evento no encontrado");

            var imageUrl = await UploadImage(dto.Image, "events/main");

            if (!string.IsNullOrEmpty(existingEvent.MainImageUrl))
            {
                await DeleteImageFile(existingEvent.MainImageUrl);
            }

            existingEvent.MainImageUrl = imageUrl;
            existingEvent.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return imageUrl;
        }

        public async Task<ParticipantDetailDto> AddParticipantAsync(AddParticipantDto dto)
        {
            // Método: Añade un participante a un evento y crea su registro en la base de datos
            var existingEvent = await context.Events.FindAsync(dto.EventId);
            if (existingEvent == null)
                throw new KeyNotFoundException("Evento no encontrado");

            var participant = new Participant
            {
                Name = dto.Name,
                Description = dto.Description,
                ParticipantTypeId = dto.ParticipantTypeId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (dto.Photo != null)
            {
                participant.LogoUrl = await UploadImage(dto.Photo, "participants");
            }

            context.Participants.Add(participant);
            await context.SaveChangesAsync();

            var eventParticipant = new EventParticipant
            {
                EventId = dto.EventId,
                ParticipantId = participant.ParticipantId,
                RoleId = dto.RoleId,
                CreatedAt = DateTime.UtcNow
            };

            context.EventParticipants.Add(eventParticipant);
            await context.SaveChangesAsync();

            var participantType = await context.ParticipantTypes
                .FirstOrDefaultAsync(pt => pt.ParticipantTypeId == dto.ParticipantTypeId);

            var role = await context.ParticipantRoles
                .FirstOrDefaultAsync(r => r.RoleId == dto.RoleId);

            return new ParticipantDetailDto
            {
                ParticipantId = participant.ParticipantId,
                Name = participant.Name,
                Description = participant.Description,
                PhotoUrl = participant.LogoUrl,
                Type = participantType?.Name,
                Role = role?.Name
            };
        }

        public async Task<bool> RemoveParticipantAsync(int eventId, int participantId)
        {
            // Método: Elimina un participante de un evento y opcionalmente su registro si no está en otros eventos
            var eventExists = await context.Events.AnyAsync(e => e.EventId == eventId);
            if (!eventExists)
            {
                throw new KeyNotFoundException("Evento no encontrado");
            }

            var participantExists = await context.Participants.AnyAsync(p => p.ParticipantId == participantId);
            if (!participantExists)
            {
                throw new KeyNotFoundException("Participante no encontrado");
            }

            var eventParticipant = await context.EventParticipants
                .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.ParticipantId == participantId);

            if (eventParticipant == null)
            {
                throw new InvalidOperationException("El participante no está asociado a este evento");
            }

            context.EventParticipants.Remove(eventParticipant);

            var isParticipantInOtherEvents = await context.EventParticipants
                .AnyAsync(ep => ep.ParticipantId == participantId && ep.EventId != eventId);

            if (!isParticipantInOtherEvents)
            {
                var participant = await context.Participants.FindAsync(participantId);

                if (!string.IsNullOrEmpty(participant.LogoUrl))
                {
                    await DeleteImageFile(participant.LogoUrl);
                }

                context.Participants.Remove(participant);
            }

            await context.SaveChangesAsync();
            return true;
        }

        private async Task<string> UploadImage(IFormFile imageFile, string subfolder)
        {
            // Método: Sube una imagen al servidor y genera una URL única
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("No se ha proporcionado una imagen válida");

            if (imageFile.Length > 5 * 1024 * 1024) // 5MB
                throw new ArgumentException("El tamaño de la imagen excede el límite permitido (5MB)");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Tipo de archivo no permitido. Solo se aceptan JPG, JPEG, PNG y WEBP");

            var uploadsPath = Path.Combine(environment.WebRootPath, "Uploads", subfolder);
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var request = httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}/Uploads/{subfolder}/{fileName}";
        }

        private async Task DeleteImageFile(string imageUrl)
        {
            // Método: Elimina un archivo de imagen del servidor basado en su URL
            if (string.IsNullOrEmpty(imageUrl))
                return;

            try
            {
                var filePath = imageUrl.Replace(
                    $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}/",
                    string.Empty);

                filePath = Path.Combine(environment.WebRootPath, filePath);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar archivo de imagen: {ex.Message}");
            }
        }

        public async Task<EventResponseDto> GetEventByIdAsync(int eventId)
        {
            // Método: Obtiene un evento específico por su ID usando GetEventResponse
            return await GetEventResponse(eventId);
        }

        public async Task<IEnumerable<EventResponsePastDto>> GetPastEventsAsync()
        {
            // Método: Obtiene la lista de eventos pasados basados en la fecha actual
            var currentDate = DateTime.UtcNow;

            var pastEvents = await context.Events
                .Include(e => e.Organization)
                .Include(e => e.EventStatus)
                .Include(e => e.PaymentMethod)
                .Include(e => e.EventParticipants)
                    .ThenInclude(ep => ep.Participant)
                        .ThenInclude(p => p.ParticipantType)
                .Include(e => e.EventParticipants)
                    .ThenInclude(ep => ep.Role)
                .Where(e => e.EndDate < currentDate)
                .ToListAsync();

            return pastEvents.Select(e => new EventResponsePastDto
            {
                EventId = e.EventId,
                MainImageUrl = e.MainImageUrl,
                OrganizationId = e.OrganizationId,
                OrganizationName = e.Organization.Name,
                Title = e.Title,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                PaymentMethodId = e.PaymentMethodId,
                PaymentMethod = e.PaymentMethod?.Name,
                PaymentLink = e.PaymentLink,
                Location = e.Location,
                Address = e.Address,
                Country = e.Country,
                EventStatusId = e.EventStatusId,
                EventStatus = e.EventStatus?.Name,
                Participants = e.EventParticipants.Select(ep => new ParticipantDetailDto
                {
                    ParticipantId = ep.ParticipantId,
                    Name = ep.Participant.Name,
                    Description = ep.Participant.Description,
                    PhotoUrl = ep.Participant.LogoUrl,
                    Type = ep.Participant.ParticipantType?.Name,
                    Role = ep.Role?.Name
                }).ToList(),
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                EventPast = true
            }).ToList();
        }

        public async Task<EventStatusIdResponse> UpdateEventStatusAsync(UpdateEventStatusDto dto, int userId)
        {
            // Método: Actualiza el estado de un evento tras validar permisos y existencia
            var eventEntity = await context.Events
                .Include(e => e.EventStatus)
                .FirstOrDefaultAsync(e => e.EventId == dto.EventId);

            if (eventEntity == null)
            {
                throw new KeyNotFoundException("Event not found");
            }

            var statusExists = await context.EventStatuses
                .AnyAsync(s => s.EventStatusId == dto.EventStatusId);

            if (!statusExists)
            {
                throw new ArgumentException("Invalid status ID");
            }

            if (userId != eventEntity.UserId)
            {
                throw new ArgumentException("No se puede publicar el evento por que no fue creado por este usuario");
            }

            eventEntity.EventStatusId = dto.EventStatusId;
            eventEntity.UpdatedAt = DateTime.UtcNow;

            try
            {
                await context.SaveChangesAsync();
                return new EventStatusIdResponse
                {
                    Success = true,
                    Message = "El evento se publicó correctamente",
                };
            }
            catch (Exception)
            {
                return new EventStatusIdResponse
                {
                    Success = false,
                    Message = "No se logró publicar el evento",
                };
            }
        }

        public async Task<List<EventRecommendationDto>> GetRecommendedEventsAsync(int userId)
        {
            // Método: Genera recomendaciones de eventos basadas en las preferencias de baile del usuario
            var userPreferences = await context.UserDancePreferences
                .Where(udp => udp.UserId == userId)
                .ToListAsync();

            if (!userPreferences.Any())
                return new List<EventRecommendationDto>();

            var activeEvents = await context.Events
                .Include(e => e.EventDanceTypes)
                    .ThenInclude(edt => edt.DanceType)
                .Include(e => e.Organization)
                .Where(e => e.StartDate >= DateTime.UtcNow && e.EventStatusId == 2)
                .ToListAsync();

            var recommendations = new List<EventRecommendationDto>();

            foreach (var ev in activeEvents)
            {
                var eventDances = ev.EventDanceTypes.Select(edt => edt.DanceTypeId).ToList();
                var matchingPreferences = userPreferences
                    .Where(up => eventDances.Contains(up.DanceTypeId))
                    .ToList();

                if (!matchingPreferences.Any())
                    continue;

                var matchScore = matchingPreferences.Average(up => up.PreferenceLevel) * 20;
                var matchingDanceNames = matchingPreferences
                    .Select(up => context.DanceTypes.First(dt => dt.DanceTypeId == up.DanceTypeId).Name)
                    .ToList();

                var eventDto = await GetEventResponse(ev.EventId);

                recommendations.Add(new EventRecommendationDto
                {
                    Event = eventDto,
                    MatchScore = matchScore,
                    MatchingDances = matchingDanceNames
                });
            }

            return recommendations.OrderByDescending(r => r.MatchScore).ToList();
        }

        public async Task<EventDanceTypesResponseDto> UpdateEventDanceTypesAsync(int eventId, UpdateEventDanceTypesDto dto, int userId)
        {
            // Método: Actualiza los tipos de baile asociados a un evento tras validar permisos y existencia
            var eventEntity = await context.Events
                .Include(e => e.EventDanceTypes)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventEntity == null)
            {
                throw new KeyNotFoundException("Evento no encontrado");
            }

            if (eventEntity.UserId != userId)
            {
                throw new UnauthorizedAccessException("No tienes permiso para modificar este evento");
            }

            var existingDanceTypes = await context.DanceTypes
                .Where(dt => dto.DanceTypeIds.Contains(dt.DanceTypeId))
                .ToListAsync();

            if (existingDanceTypes.Count != dto.DanceTypeIds.Count)
            {
                var invalidIds = dto.DanceTypeIds.Except(existingDanceTypes.Select(dt => dt.DanceTypeId));
                throw new ArgumentException($"Los siguientes IDs de tipo de baile no son válidos: {string.Join(", ", invalidIds)}");
            }

            if (dto.PrimaryDanceTypeId.HasValue && !dto.DanceTypeIds.Contains(dto.PrimaryDanceTypeId.Value))
            {
                throw new ArgumentException("El baile principal debe estar incluido en la lista de tipos de baile");
            }

            var existingRelations = eventEntity.EventDanceTypes.ToList();
            var relationsToRemove = existingRelations
                .Where(er => !dto.DanceTypeIds.Contains(er.DanceTypeId))
            .ToList();

            context.EventDanceTypes.RemoveRange(relationsToRemove);

            var existingDanceTypeIds = existingRelations.Select(er => er.DanceTypeId).ToList();
            var relationsToAdd = dto.DanceTypeIds
                .Except(existingDanceTypeIds)
                .Select(dtId => new EventDanceType
                {
                    EventId = eventId,
                    DanceTypeId = dtId,
                    IsPrimary = dto.PrimaryDanceTypeId.HasValue && dto.PrimaryDanceTypeId.Value == dtId
                }).ToList();

            context.EventDanceTypes.AddRange(relationsToAdd);

            foreach (var existingRelation in eventEntity.EventDanceTypes)
            {
                existingRelation.IsPrimary = dto.PrimaryDanceTypeId.HasValue && existingRelation.DanceTypeId == dto.PrimaryDanceTypeId.Value;
            }

            await context.SaveChangesAsync();

            var updatedDanceTypes = await context.EventDanceTypes
                .Where(edt => edt.EventId == eventId)
                .Include(edt => edt.DanceType)
                .Select(edt => new DanceTypeDto
                {
                    DanceTypeId = edt.DanceType.DanceTypeId,
                    Name = edt.DanceType.Name,
                    Description = edt.DanceType.Description,
                    IsPrimary = edt.IsPrimary
                }).ToListAsync();

            return new EventDanceTypesResponseDto
            {
                EventId = eventId,
                DanceTypes = updatedDanceTypes
            };
        }

        public async Task<string> GeneratePurchaseQRCodeAsync(int eventId, int userId, int eventPurchaseId)
        {
            // Validar la compra
            var purchase = await context.EventPurchases
                .FirstOrDefaultAsync(ep => ep.PurchaseId == eventPurchaseId && ep.EventId == eventId && ep.UserId == userId)
                ?? throw new ArgumentException("Compra no encontrada");

            // Generar un token único
            var token = Guid.NewGuid().ToString();
            var expiresAt = DateTime.UtcNow.AddHours(8);

            // Crear el enlace para verificar el QR
            var request = httpContextAccessor.HttpContext.Request;
            var verificationUrl = $"{request.Scheme}://{request.Host}/api/events/verify-purchase/{token}";

            // Crear el QR con QRCoder (usando bytes directamente)
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(verificationUrl, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20); // 20 = píxeles por módulo

            // Guardar la imagen PNG en disco
            var uploadsPath = Path.Combine(environment.WebRootPath, "Uploads", "qrcodes");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{token}.png";
            var filePath = Path.Combine(uploadsPath, fileName);
            await File.WriteAllBytesAsync(filePath, qrCodeBytes);

            var qrCodeUrl = $"{request.Scheme}://{request.Host}/Uploads/qrcodes/{fileName}";

            // Guardar el token en la base de datos
            var purchaseToken = new PurchaseToken
            {
                EventPurchaseId = purchase.PurchaseId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            context.PurchaseTokens.Add(purchaseToken);
            await context.SaveChangesAsync();

            return qrCodeUrl;

        }


        public async Task<string> RegeneratePurchaseQRCodeAsync(int eventId, int purchaseId, int userId)
        {
            // Validar la compra
            var purchase = await context.EventPurchases
                .Include(ep => ep.Event)
                .FirstOrDefaultAsync(ep => ep.PurchaseId == purchaseId && ep.EventId == eventId && ep.UserId == userId)
                ?? throw new ArgumentException("Compra no encontrada o no pertenece al usuario");

            // Verificar que el evento no haya expirado
            if (purchase.Event.ExpirationDate.HasValue && purchase.Event.ExpirationDate < DateTime.UtcNow)
                throw new InvalidOperationException("El evento ha caducado, no se puede regenerar el código QR");

            // Buscar el token existente
            var existingToken = await context.PurchaseTokens
                .FirstOrDefaultAsync(pt => pt.EventPurchaseId == purchaseId && pt.ExpiresAt > DateTime.UtcNow);
            if (existingToken != null)
            {
                var request = httpContextAccessor.HttpContext.Request;
                return $"{request.Scheme}://{request.Host}/Uploads/qrcodes/{existingToken.Token}.png";
            }

            // Generar un nuevo token y QR
            return await GeneratePurchaseQRCodeAsync(eventId, userId, purchaseId);
        }

       async Task<IEnumerable> IEventService.GetUserPurchaseQRCodesAsync(int userId)
        {
            var qrCodes = await context.PurchaseTokens
              .Include(pt => pt.EventPurchase)
                  .ThenInclude(ep => ep.Event)
              .Where(pt => pt.EventPurchase.UserId == userId && pt.ExpiresAt > DateTime.UtcNow)
              .Select(pt => new UserPurchaseQRCodeDto
              {
                  EventId = pt.EventPurchase.EventId,
                  PurchaseId = pt.EventPurchaseId,
                  EventTitle = pt.EventPurchase.Event.Title,
                  QRCodeUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}/Uploads/qrcodes/{pt.Token}.png",
                  ExpiresAt = pt.ExpiresAt
              })
              .ToListAsync();

            return qrCodes;
        }
    }
}