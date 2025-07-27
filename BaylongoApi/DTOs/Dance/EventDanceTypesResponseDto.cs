namespace BaylongoApi.DTOs.Dance
{
    public class EventDanceTypesResponseDto
    {
        public int EventId { get; set; }
        public List<DanceTypeDto> DanceTypes { get; set; } = new List<DanceTypeDto>();
        public string Message { get; set; }
    }
}
