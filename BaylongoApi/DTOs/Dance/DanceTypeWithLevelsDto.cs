namespace BaylongoApi.DTOs.Dance
{
    public class DanceTypeWithLevelsDto
    {
        public int DanceTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<DanceLevelDto> AvailableLevels { get; set; }
    }
}
