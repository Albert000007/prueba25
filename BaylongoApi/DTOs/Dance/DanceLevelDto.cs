namespace BaylongoApi.DTOs.Dance
{
    public class DanceLevelDto
    {
        public int LevelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsBeginnerFriendly { get; set; }
        public int Order { get; set; }
    }
}
