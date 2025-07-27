namespace BaylongoApi.DTOs.Maintenance
{
    public class MaintenanceRequest
    {
        public bool IsActive { get; set; }
        public string? Message { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
