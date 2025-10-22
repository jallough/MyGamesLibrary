using Microsoft.EntityFrameworkCore;
using Server.DBContext;
using Server.Games.Entities;

namespace Server.Games.Repositories
{
    public class GamesRepository: IGamesRepository
    {
        private readonly IDataContext _context;
        public GamesRepository(IDataContext context) {
            _context = context;
        }

        private IQueryable<GamesEntity> GetAll()
        {
            return _context.Games.OrderBy(g=>g.ReleaseDate);
        }
        public async Task<List<GamesEntity>> GetAllAsync()
        {
            return await GetAll().ToListAsync();
        }
        public async Task<List<GamesEntity>> GetAllAsync(int page, int number)
        {
            return await GetAll().Skip(page*number).Take(number).ToListAsync();
        }
        public async Task<GamesEntity?> GetByIdAsync(long id)
        {
            return await _context.Games.FindAsync(id);
        }
        public async Task AddAsync(GamesEntity game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(GamesEntity game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(long id)
        {
            var game = await _context.Games.FindAsync(id);
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }


    }
}
