using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Server.Games.Entities;

namespace Server.DBContext
{
    public class DatabaseInitializer(ILogger<DatabaseInitializer> logger, IConfiguration configuration, IServiceProvider services, IOptions<DatabaseInitializer.InitValues> initValues) : BackgroundService
    {
        private readonly InitValues initValues = initValues.Value;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitializeAsync();
        }
        public async Task InitializeAsync()
        {
            try
            {
                using (var scope = services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();

                    if (configuration.GetValue<bool>("Migrate") || initValues.migrate)
                    {

                        await context.Database.MigrateAsync();
                        logger.LogInformation("Base de données créée/migrée avec succès.");
                    }
                    if (configuration.GetValue<bool>("Initialize") || initValues.initialize)
                    {
                        if (context.Games.Any())
                        {
                            logger.LogInformation("La base de données est déjà initialisée.");
                            return;
                        }
                        var config = new ConfigurationBuilder();
                        var conf = config.AddJsonFile("Games.json").Build();
                        var games = conf.GetSection("Games").Get<List<GamesEntity>>().ToList();
                        context.Games.AddRange(games);
                        await context.SaveChangesAsync();

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de l'initialisation de la base de données.");
                throw;
            }
        }

        public class InitValues
        {
            public bool migrate { get; set; }
            public bool initialize { get; set; }
        }
    }


}
