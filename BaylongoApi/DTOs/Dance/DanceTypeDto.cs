namespace BaylongoApi.DTOs.Dance
{
    public class DanceTypeDto
    {
        public int DanceTypeId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string IconUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DanceCategoryDto Category { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public ICollection<DanceTypeLevel> DanceTypeLevels { get; set; }
    }
}
