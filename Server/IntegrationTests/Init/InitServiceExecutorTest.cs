using FluentAssertions;
using IntegrationTests.Authorized;
using Server.Games.Entities;

namespace IntegrationTests.Init;

[TestCaseOrderer(ordererTypeName: "IntegrationTests.PriorityOrderer", ordererAssemblyName: "IntegrationTests")]
public class InitialisationTest : BaseIntegrationTest
{
    private readonly IntegrationTestWebAppFactory _factory;

    public InitialisationTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact, TestPriority(0)]
    public async Task CheckInitializedData()
    {
        var games = DbContext.Set<GamesEntity>().ToList();
        games.Should().NotBeEmpty();
    }
}
