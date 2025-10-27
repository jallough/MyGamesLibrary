using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Server.Games.Entities;
using Server.Games.Repositories;
using Server.Shared;

namespace Server.Games.Services
{
    public class GamesServices(IGamesRepository _gamesRepository, ILogger<GamesServices> _logger, IMapper _mapper, IServiceProvider _serviceProvider, ICachingService cacheservice ) : IGamesServices
    {
        
        private readonly string _cacheKey = "Games";        
        private void CacheGamesDataAsync(List<GamesEntity> games)
        {
            cacheservice.UpdateAsync(_cacheKey, games);
            _logger.LogInformation("Caching games data for future requests");
        }
        public async Task<List<GamesEntity>> GetAllAsync(string orderBy, string filterByCategory, string search, int page, int number)
        {
            var cachedGames = await cacheservice.GetAsync<List<GamesEntity>>(_cacheKey);
            if (cachedGames != null)
            {
                _logger.LogInformation("Returning cached games for Page: {Page}, Number: {Number}", page, number);
                if (!string.IsNullOrWhiteSpace(search))
                {
                    cachedGames = cachedGames.Where(g => g.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(filterByCategory))
                {
                    switch (filterByCategory.ToLower())
                    {
                        case "action":
                            cachedGames = cachedGames.Where(g => g.Genre == Genre.Action).ToList();
                            break;
                        case "adventure":
                            cachedGames = cachedGames.Where(g => g.Genre == Genre.Adventure).ToList();
                            break;
                        case "rpg":
                            cachedGames = cachedGames.Where(g => g.Genre == Genre.RPG).ToList();
                            break;
                        case "strategy":
                            cachedGames = cachedGames.Where(g => g.Genre == Genre.Strategy).ToList();
                            break;
                        case "simulation":
                            cachedGames = cachedGames.Where(g => g.Genre == Genre.Simulation).ToList();
                            break;
                        case "sports":
                            cachedGames = cachedGames.Where(g => g.Genre == Genre.Sports).ToList();
                            break;
                        case "puzzle":
                            cachedGames = cachedGames.Where(g => g.Genre == Genre.Puzzle).ToList();
                            break;
                        case "horror":
                            cachedGames = cachedGames.Where(g => g.Genre == Genre.Horror).ToList();
                            break;
                        case "mmo":
                            cachedGames = cachedGames.Where(g => g.Genre == Genre.MMO).ToList();
                            break;
                        case "indie":
                            cachedGames = cachedGames.Where(g => g.Genre == Genre.Indie).ToList();
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(orderBy))
                {


                    switch (orderBy.ToLower())
                    {
                        case "titleA":
                            cachedGames = cachedGames.OrderBy(g => g.Title).ToList();
                            break;
                        case "titleD":
                            cachedGames = cachedGames.OrderByDescending(g => g.Title).ToList();
                            break;
                        case "genreA":
                            cachedGames = cachedGames.OrderBy(g => g.Genre).ToList();
                            break;
                        case "genreD":
                            cachedGames = cachedGames.OrderByDescending(g => g.Genre).ToList();
                            break;
                        case "publishDateLH":
                            cachedGames = cachedGames.OrderBy(g => g.ReleaseDate).ToList();
                            break;
                        default:
                            cachedGames = cachedGames.OrderByDescending(g => g.ReleaseDate).ToList();
                            break;
                    }
                }
                return cachedGames.Skip(page * number).Take(number).ToList();
            }
            else
            {
                _ = Task.Run(async ()=> {
                    try
                    {
                        List<GamesEntity> allGames;
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var gamesRepo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                            allGames = await gamesRepo.GetAllAsync();
                        }
                        CacheGamesDataAsync(allGames);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to populate cache");
                    }
                });

            }
            _logger.LogInformation("Fetching all games - Page: {Page}, Number: {Number}", page, number);
            return await _gamesRepository.GetAllAsync(orderBy, filterByCategory, search, page, number);
        }

        public async Task<GamesEntity?> GetByIdAsync(long id)
        {
            _logger.LogInformation("Fetching game by ID: {Id}", id);
            return await _gamesRepository.GetByIdAsync(id);
        }
        public async Task AddAsync(GamesEntity game)
        {
            try
            {
                var existingGame = await _gamesRepository.GetByIdAsync(game.Id);
                if (existingGame != null)
                {
                    _logger.LogWarning("Attempted to add a game with an existing ID: {GameId}", game.Id);
                    throw new InvalidOperationException($"A game with ID {game.Id} already exists.");
                }
                _logger.LogInformation("Adding new game: {GameTitle}", game.Title);
                await _gamesRepository.AddAsync(game);
                _ = Task.Run(async () => {
                    try
                    {
                        List<GamesEntity> allGames;
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var gamesRepo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                            allGames = await gamesRepo.GetAllAsync();
                        }
                        CacheGamesDataAsync(allGames);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to populate cache");
                    }
                });
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
                _ = Task.Run(async () => {
                    try
                    {
                        List<GamesEntity> allGames;
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var gamesRepo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                            allGames = await gamesRepo.GetAllAsync();
                        }
                        CacheGamesDataAsync(allGames);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to populate cache");
                    }
                });
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
                _ = Task.Run(async () => {
                    try
                    {
                        List<GamesEntity> allGames;
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var gamesRepo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                            allGames = await gamesRepo.GetAllAsync();
                        }
                        CacheGamesDataAsync(allGames);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to populate cache");
                    }
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting game with ID: {GameId}", id);
                throw;
            }
        }
        public async Task<List<GamesEntity>> GetAllAsync()
        {
            var cachedGames = await cacheservice.GetAsync<List<GamesEntity>>(_cacheKey);

            if (cachedGames!=null)
            {
                _logger.LogInformation("Returning cached games");
                return cachedGames;
            }
            else
            {
                _ = Task.Run(async () => {
                    try
                    {
                        List<GamesEntity> allGames;
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var gamesRepo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                            allGames = await gamesRepo.GetAllAsync();
                        }
                        CacheGamesDataAsync(allGames);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to populate cache");
                    }
                });
            }
            cachedGames = await cacheservice.GetAsync<List<GamesEntity>>(_cacheKey);

            return cachedGames!;
        }

        public async Task<List<GamesUserRelationEntity>> GetAllByUserAsync(string orderBy, string filterByCategory, string filterByStatus, string search, int page, int number, long userId)
        {
            return await _gamesRepository.GetAllByUserAsync(orderBy, filterByCategory, filterByStatus, search, page, number, userId);
        }
    }
}
