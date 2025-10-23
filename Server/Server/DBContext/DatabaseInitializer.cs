using Microsoft.EntityFrameworkCore;
using Server.Games.Entities;
using Server.Users.Entities;
using Server.Users.Services;
using System.Text.Json;

namespace Server.DBContext
{
    public class DatabaseInitializer(AppDBContext context, ILogger<DatabaseInitializer> logger, IConfiguration configuration,IUsersService usersService)
    {
        public async Task InitializeAsync()
        {
            try
            {
                if(configuration.GetValue<bool>("Migrate"))
                {
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Base de données créée/migrée avec succès.");
                }
                if (configuration.GetValue<bool>("Initialize"))
                {
                    var config = new ConfigurationBuilder();
                    var conf =config.AddJsonFile("Games.json").Build();
                    var games = conf.GetSection("Games").Get<List<GamesEntity>>().ToList();
                    context.Games.AddRange(games);
                    await context.SaveChangesAsync();
                    var users = conf.GetSection("Users").Get<List<UserEntity>>().First();
                    //usersService.AddUser(users);
                    
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de l'initialisation de la base de données.");
                throw;
            }
        }
    }
}
