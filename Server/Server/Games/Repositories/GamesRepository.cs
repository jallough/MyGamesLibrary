using Microsoft.EntityFrameworkCore;
using Server.DBContext;
using Server.Games.Entities;

namespace Server.Games.Repositories
{
    public class GamesRepository(IDataContext _context) : IGamesRepository
    {
        private IQueryable<GamesEntity> GetAll()
        {
            return _context.Games.OrderByDescending(g => g.ReleaseDate);
        }
        public async Task<List<GamesEntity>> GetAllAsync()
        {
            return await GetAll().ToListAsync();
        }
        public async Task<List<GamesEntity>> GetAllAsync(string orderBy, string filterByCategory, string search, int page, int number)
        {
            var query = GetAll();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(g => g.Title.ToLower().Contains(search.ToLower()));
            }
            switch (filterByCategory)
            {
                case "action":
                    query = query.Where(g => g.Genre == Genre.Action);
                    break;
                case "adventure":
                    query = query.Where(g => g.Genre == Genre.Adventure);
                    break;
                case "rpg":
                    query = query.Where(g => g.Genre == Genre.RPG);
                    break;
                case "strategy":
                    query = query.Where(g => g.Genre == Genre.Strategy);
                    break;
                case "simulation":
                    query = query.Where(g => g.Genre == Genre.Simulation);
                    break;
                case "sports":
                    query = query.Where(g => g.Genre == Genre.Sports);
                    break;
                case "puzzle":
                    query = query.Where(g => g.Genre == Genre.Puzzle);
                    break;
                case "horror":
                    query = query.Where(g => g.Genre == Genre.Horror);
                    break;
                case "mmo":
                    query = query.Where(g => g.Genre == Genre.MMO);
                    break;
                case "indie":
                    query = query.Where(g => g.Genre == Genre.Indie);
                    break;
                default:
                    break;
            }
            switch (orderBy)
            {
                case "titleA":
                    query = query.OrderBy(g => g.Title);
                    break;
                case "titleD":
                    query = query.OrderByDescending(g => g.Title);
                    break;
                case "genreA":
                    query = query.OrderBy(g => g.Genre);
                    break;
                case "genreD":
                    query = query.OrderByDescending(g => g.Genre);
                    break;
                case "publishDateLH":
                    query = query.OrderBy(g => g.ReleaseDate);
                    break;
                default:
                    query = query.OrderByDescending(g => g.ReleaseDate);
                    break;

            }
            return await query.Skip(page * number).Take(number).ToListAsync();
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

        public async Task<List<GamesUserRelationEntity>> GetAllByUserAsync(string orderBy, string filterByCategory, string filterByStatus, string search, int page, int number, long userId)
        {
            var query = _context.GamesUserRelations
                .Include(rel => rel.Game)
                .Where(rel => rel.UserId == userId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(g => g.Game.Title.ToLower().Contains(search.ToLower()));
            }
            switch (filterByCategory)
            {
                case "action":
                    query = query.Where(g => g.Game.Genre == Genre.Action);
                    break;
                case "adventure":
                    query = query.Where(g => g.Game.Genre == Genre.Adventure);
                    break;
                case "rpg":
                    query = query.Where(g => g.Game.Genre == Genre.RPG);
                    break;
                case "strategy":
                    query = query.Where(g => g.Game.Genre == Genre.Strategy);
                    break;
                case "simulation":
                    query = query.Where(g => g.Game.Genre == Genre.Simulation);
                    break;
                case "sports":
                    query = query.Where(g => g.Game.Genre == Genre.Sports);
                    break;
                case "puzzle":
                    query = query.Where(g => g.Game.Genre == Genre.Puzzle);
                    break;
                case "horror":
                    query = query.Where(g => g.Game.Genre == Genre.Horror);
                    break;
                case "mmo":
                    query = query.Where(g => g.Game.Genre == Genre.MMO);
                    break;
                case "indie":
                    query = query.Where(g => g.Game.Genre == Genre.Indie);
                    break;
                default:
                    break;
            }
            switch(filterByStatus)
            {
                case "playing":
                    query = query.Where(g => g.Status == Status.playing);
                    break;
                case "completed":
                    query = query.Where(g => g.Status == Status.completed);
                    break;
                case "onHold":
                    query = query.Where(g => g.Status == Status.onhold);
                    break;
                case "dropped":
                    query = query.Where(g => g.Status == Status.dropped);
                    break;
                case "planToPlay":
                    query = query.Where(g => g.Status == Status.plantoplay);
                    break;
                default:
                    break;
            }
            switch (orderBy)
            {
                case "titleA":
                    query = query.OrderBy(g => g.Game.Title);
                    break;
                case "titleD":
                    query = query.OrderByDescending(g => g.Game.Title);
                    break;
                case "genreA":
                    query = query.OrderBy(g => g.Game.Genre);
                    break;
                case "genreD":
                    query = query.OrderByDescending(g => g.Game.Genre);
                    break;
                case "publishDateLH":
                    query = query.OrderBy(g => g.Game.ReleaseDate);
                    break;
                default:
                    query = query.OrderByDescending(g => g.Game.ReleaseDate);
                    break;

            }
            return await query.Skip(page * number).Take(number).ToListAsync();
        }

        public async Task AddRelation(GamesUserRelationEntity game)
        {
            _context.GamesUserRelations.Add(game);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRelation(GamesUserRelationEntity game)
        {
            _context.GamesUserRelations.Update(game);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteRelation(long id)
        {
            var relation = await _context.GamesUserRelations.FindAsync(id);
            if (relation == null)
            {
                throw new KeyNotFoundException();
            }
            _context.GamesUserRelations.Remove(relation);
            await _context.SaveChangesAsync();
        }

        public Task<GamesUserRelationEntity> GetRelation(GamesUserRelationEntity game)
        {
            return _context.GamesUserRelations.Where(g => g.GameId == game.GameId && g.UserId == game.UserId).FirstAsync();
        }
    }
}
