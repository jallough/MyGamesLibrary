using Microsoft.Extensions.DependencyInjection;
using Server.DBContext;

namespace IntegrationTests.Unauthorized;

public abstract class BaseIntegrationTestUnauthorized: IClassFixture<IntegrationTestWebAppFactoryUnauthorized>, IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly AppDBContext DbContext;

    protected BaseIntegrationTestUnauthorized(IntegrationTestWebAppFactoryUnauthorized factory)
    {
        _scope = factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider
            .GetRequiredService<AppDBContext>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        DbContext?.Dispose();
    }
}
