using Server.Shared;
using Server.Users.Entities;

namespace Server.Games.Entities
{
    public class GamesUserRelationEntity:BaseEntity
    {
        public long UserId { get; set; } 
        public long GameId { get; set; }
        public GamesEntity Game { get; set; } = null!;
        public Status Status { get; set; }
    }

    public enum Status
    {
        playing,
        completed,
        onhold,
        dropped,
        plantoplay
    }
}
