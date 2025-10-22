using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Server.Games.Entities;
using Server.Games.Repositories;

namespace Server.Games.Services
{
    public class GamesServices:IGamesServices
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILogger<GamesServices> _logger;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey = "Games";
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;
        public GamesServices(IGamesRepository gamesRepository, ILogger<GamesServices> logger, IMapper mapper, IMemoryCache cache)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
            _cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                .SetSlidingExpiration(TimeSpan.FromDays(7))
                .SetPriority(CacheItemPriority.Normal);
        }
        private async Task CacheGamesDataAsync()
        {
            var games = await _gamesRepository.GetAllAsync();
            _cache.Set(_cacheKey, games, _cacheEntryOptions);
            _logger.LogInformation("Caching games data for future requests");
        }
        public async Task<List<GamesEntity>> GetAllAsync(int page, int number)
        {
            if(_cache.TryGetValue(_cacheKey, out List<GamesEntity>? cachedGames))
            {
                _logger.LogInformation("Returning cached games for Page: {Page}, Number: {Number}", page, number);
                return cachedGames.Skip(page*number).Take(number).ToList();
            }else
            {
                _ = Task.Run(CacheGamesDataAsync);
                
            }
            _logger.LogInformation("Fetching all games - Page: {Page}, Number: {Number}", page, number);
            return await _gamesRepository.GetAllAsync(page, number);
        }

        public async Task<GamesEntity?> GetByIdAsync(long id)
        {
            _logger.LogInformation("Fetching game by ID: {Id}", id);
            return await _gamesRepository.GetByIdAsync(id);
        }
        public async Task AddAsync(GamesEntity game)
        {
            try { 
                var existingGame = await _gamesRepository.GetByIdAsync(game.Id);
                if (existingGame != null)
                {
                    _logger.LogWarning("Attempted to add a game with an existing ID: {GameId}", game.Id);
                    throw new InvalidOperationException($"A game with ID {game.Id} already exists.");
                }
                _logger.LogInformation("Adding new game: {GameTitle}", game.Title);
                await _gamesRepository.AddAsync(game);
                _ = Task.Run(CacheGamesDataAsync);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error checking for existing game with ID: {GameId}", game.Id);
                throw;
            }
            
            
        }
        public async Task UpdateAsync(GamesEntity game)
        {
            try
            {
                var gameInDb = await _gamesRepository.GetByIdAsync(game.Id);
                if (gameInDb == null)
                {
                    _logger.LogWarning("Game ID: {GameId} not found for update", game.Id);
                    throw new KeyNotFoundException($"Game with ID {game.Id} not found.");
                }
                _mapper.Map(game, gameInDb);
                await _gamesRepository.UpdateAsync(gameInDb);
                _ = Task.Run(CacheGamesDataAsync);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating game with ID: {GameId}", game.Id);
                throw;
            }
        }
        public async Task DeleteAsync(long id)
        {
            try
            {
                var gameInDb = await _gamesRepository.GetByIdAsync(id);
                if (gameInDb == null)
                {
                    _logger.LogWarning("Game ID: {GameId} not found for deletion", id);
                    throw new KeyNotFoundException($"Game with ID {id} not found.");
                }

                _logger.LogInformation("Deleting game ID: {GameId}", id);
                await _gamesRepository.DeleteAsync(id);
                _ = Task.Run(CacheGamesDataAsync);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting game with ID: {GameId}", id);
                throw;
            }
        }
        public async Task<List<GamesEntity>> GetAllAsync()
        {
            if(_cache.TryGetValue(_cacheKey, out List<GamesEntity>? cachedGames))
            {
                _logger.LogInformation("Returning cached games");
                return cachedGames;
            }
            else
            {
                await CacheGamesDataAsync();
            }
            _cache.TryGetValue(_cacheKey, out List<GamesEntity>? games);
            return games;
        }
    }
}
