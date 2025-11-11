using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.DBContext;
using Testcontainers.MsSql;

namespace IntegrationTests.Unauthorized;

public class IntegrationTestWebAppFactoryUnauthorized : WebApplicationFactory<Program>, IAsyncLifetime
{
    private bool started = false;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptorType =
                typeof(DbContextOptions<AppDBContext>);

            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == descriptorType);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDBContext>(options =>
                options.UseSqlServer(_dbContainer.GetConnectionString()));

            var dbContext = CreateDbContext(services);
            dbContext.Database.EnsureCreated();

            builder.UseEnvironment("Development");
        });
    }
        
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("YourStrong!Passw0rd")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithCleanUp(true)
        .Build();
    

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }

    private static AppDBContext CreateDbContext(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
        return dbContext;
    }

    public new HttpClient CreateClient()
    {
        var client = base.CreateClient();
        if (!started)
        {
            Task.Delay(500).Wait();
            started = true;
        }
        return client;

    }
}
