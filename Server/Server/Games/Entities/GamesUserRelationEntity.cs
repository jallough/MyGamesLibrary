using Server.Shared;
using Server.Users.Entities;

namespace Server.Games.Entities
{
    public class GamesUserRelationEntity:BaseEntity
    {
        public UserEntity User { get; set; } = null!;
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
