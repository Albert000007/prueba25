namespace BaylongoApi.DTOs.Maintenance
{
    public class MaintenanceRecord
    {
        public int MaintenanceId { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Message { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
