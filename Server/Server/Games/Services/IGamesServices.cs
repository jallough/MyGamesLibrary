using Server.Games.Entities;

namespace Server.Games.Services
{
    public interface IGamesServices
    {
        Task<List<GamesEntity>> GetAllAsync(int page, int number);
        Task<GamesEntity?> GetByIdAsync(long id);
        Task AddAsync(GamesEntity game);
        Task UpdateAsync(GamesEntity game);
        Task DeleteAsync(long id);
        Task<List<GamesEntity>> GetAllAsync();
    }
}