using BaylongoApi.DTOs.Events;
using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.Validators
{
    public class ValidateParticipantsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var participants = value as List<EventParticipantDto>;

            if (participants == null)
                return ValidationResult.Success;

            foreach (var participant in participants)
            {
                if (string.IsNullOrWhiteSpace(participant.Name))
                    return new ValidationResult("El nombre del participante es requerido");

                if (participant.Photo != null && participant.Photo.Length > 5 * 1024 * 1024)
                    return new ValidationResult("La foto del participante no puede exceder 5MB");
            }

            return ValidationResult.Success;
        }
    }
}
