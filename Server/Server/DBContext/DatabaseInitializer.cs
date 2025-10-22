using Microsoft.EntityFrameworkCore;

namespace Server.DBContext
{
    public class DatabaseInitializer(AppDBContext context, ILogger<DatabaseInitializer> logger, IConfiguration configuration)
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
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de l'initialisation de la base de données.");
                throw;
            }
        }
    }
}
