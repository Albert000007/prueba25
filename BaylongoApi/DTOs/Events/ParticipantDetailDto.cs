namespace BaylongoApi.DTOs.Events
{
    public class ParticipantDetailDto
    {
        public int ParticipantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public string Type { get; set; }
        public string Role { get; set; }
    }
}
