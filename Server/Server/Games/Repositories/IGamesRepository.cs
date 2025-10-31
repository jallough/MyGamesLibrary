using Server.Games.Entities;

namespace Server.Games.Repositories
{
    public interface IGamesRepository
    {
        Task<List<GamesEntity>> GetAllAsync();
        Task<List<GamesEntity>> GetAllAsync(string orderBy, string filterByCategory, string search, int page, int number);
        Task<GamesEntity?> GetByIdAsync(long id);
        Task AddAsync(GamesEntity game);
        Task UpdateAsync(GamesEntity game);
        Task DeleteAsync(long id);
        Task<List<GamesUserRelationEntity>> GetAllByUserAsync(string orderBy, string filterByCategory, string filterByStatus, string search, int page, int number, long userId);
        Task AddRelation(GamesUserRelationEntity game);
        Task UpdateRelation(GamesUserRelationEntity game);
        Task DeleteRelation(long id);
        Task<GamesUserRelationEntity> GetRelation(GamesUserRelationEntity game);
    }
}