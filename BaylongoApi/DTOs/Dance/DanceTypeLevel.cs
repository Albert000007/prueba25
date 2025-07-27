namespace BaylongoApi.DTOs.Dance
{
    public class DanceTypeLevel
    {
        public int dance_type_id { get; set; }
        public DanceTypeDto DanceType { get; set; }

        public int level_id { get; set; }
        public DanceLevel DanceLevel { get; set; }
        public bool is_beginner_friendly { get; set; }
    }
}
