namespace BaylongoApi.DTOs.Organizations.Enum
{
    public enum InvitationPurpose
    {
        OrganizationManagement = 1,  // Puede gestionar toda la organización
        EventManagement = 5,         // Solo puede gestionar eventos
        TicketValidation = 6,         // Solo puede validar tickets
    }
}
