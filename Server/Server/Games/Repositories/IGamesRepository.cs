using Server.Games.Entities;

namespace Server.Games.Repositories
{
    public interface IGamesRepository
    {
        Task<List<GamesEntity>> GetAllAsync();
        Task<List<GamesEntity>> GetAllAsync(int page, int number);
        Task<GamesEntity?> GetByIdAsync(long id);
        Task AddAsync(GamesEntity game);
        Task UpdateAsync(GamesEntity game);
        Task DeleteAsync(long id);

    }
}