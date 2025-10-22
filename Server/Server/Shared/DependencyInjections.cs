using Server.DBContext;
using Server.Games.Repositories;
using Server.Games.Services;

namespace Server.Shared
{
    public static class DependencyInjections
    {
        public static IServiceCollection InjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<DatabaseInitializer>();
            services.AddScoped<IDataContext, AppDBContext>();
            services.AddScoped<IGamesServices, GamesServices>();
            services.AddScoped<IGamesRepository, GamesRepository>();
            return services;
        }
    }
}
