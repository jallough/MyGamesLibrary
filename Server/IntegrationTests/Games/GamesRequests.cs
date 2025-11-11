using FluentAssertions;
using IntegrationTests.Authorized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Games
{
    public class GamesRequests : BaseIntegrationTest
    {
        private readonly IntegrationTestWebAppFactory _factory;
        private readonly HttpClient _client;
        public GamesRequests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetGames_Authorized_ShouldReturn200()
        {
            // Act
            var response = await _client.GetAsync("api/Games/user/Filtered");
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetGameById_Authorized_ShouldReturn200()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/games/1");
            // Act
            var response = await _client.SendAsync(request);
            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
