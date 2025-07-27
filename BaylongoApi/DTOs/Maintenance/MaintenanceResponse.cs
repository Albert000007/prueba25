namespace BaylongoApi.DTOs.Maintenance
{
    public class MaintenanceResponse
    {
        public bool IsActive { get; set; }
        public string? Message { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? StartedBy { get; set; }
    }
}
