using IntegrationTests.Unauthorized;


namespace IntegrationTests.Games;

[TestCaseOrderer("IntegrationTests.PriorityOrderer", "IntegrationTests")]
public class GamesUnauthorizedReuqests : BaseIntegrationTestUnauthorized
{
    private readonly IntegrationTestWebAppFactoryUnauthorized _factory;
    private readonly HttpClient _client;
    public GamesUnauthorizedReuqests(IntegrationTestWebAppFactoryUnauthorized factory) : base(factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact,TestPriority(0)]
    public async Task GetAllGames_Unauthorized_ShouldReturn204()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/games");
        // Act
        var response = await _client.SendAsync(request);
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }
    [Fact, TestPriority(1)]
    public async Task GetAllGames_Unauthorized_ShouldReturn200()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/games");
        // Act
        var response = await _client.SendAsync(request);
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
    [Fact]
    public async Task GetGameById_Unauthorized_ShouldReturn401()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/games/1");
        // Act
        var response = await _client.SendAsync(request);
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task GetGames_Unauthorized_ShouldReturn401()
    {
        // Act
        var response = await _client.GetAsync("api/Games/user/Filtered");
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
