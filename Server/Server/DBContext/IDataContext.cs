using Microsoft.EntityFrameworkCore;
using Server.Games.Entities;
using Server.Users.Entities;

namespace Server.DBContext
{
    public interface IDataContext
    {
        public DbSet<GamesEntity> Games { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<GamesUserRelationEntity> GamesUserRelations { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}