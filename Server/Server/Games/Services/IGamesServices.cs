using Server.Games.Entities;

namespace Server.Games.Services
{
    public interface IGamesServices
    {
        Task<List<GamesEntity>> GetAllAsync(string orderBy, string filterByCategory, string search, int page, int number);
        Task<List<GamesUserRelationEntity>> GetAllByUserAsync(string orderBy, string filterByCategory, string filterByStatus, string search, int page, int number, long userId);

        Task<GamesEntity?> GetByIdAsync(long id);
        Task AddAsync(GamesEntity game);
        Task UpdateAsync(GamesEntity game);
        Task DeleteAsync(long id);
        Task<List<GamesEntity>> GetAllAsync();
        Task AddGameByUserAsync(GamesUserRelationEntity game);
        Task UpdateGameByUserAsync(GamesUserRelationEntity game);
        Task DeleteGameByUserAsync(long id);
    }
}