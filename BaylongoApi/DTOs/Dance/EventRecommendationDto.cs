using BaylongoApi.DTOs.Events;

namespace BaylongoApi.DTOs.Dance
{
    public class EventRecommendationDto
    {
        public EventResponseDto Event { get; set; }
        public double MatchScore { get; set; } // 0-100%
        public List<string> MatchingDances { get; set; }
    }
}
