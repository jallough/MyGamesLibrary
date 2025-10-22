using Server.Shared;

namespace Server.Games.Entities
{
    public class GamesEntity:BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public Genre Genre { get; set; } 
        public DateOnly ReleaseDate { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

    }

    public enum Genre
    {
        Action,
        Adventure,
        RPG,
        Strategy,
        Simulation,
        Sports,
        Puzzle,
        Horror,
        MMO,
        Indie
    }
    
}
