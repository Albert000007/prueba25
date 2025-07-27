namespace BaylongoApi.DTOs.Dance
{
    public class DanceLevel
    {
        public int level_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int order { get; set; }
        public bool is_active { get; set; }

        public ICollection<DanceTypeLevel> DanceTypeLevels { get; set; }
    }
}
