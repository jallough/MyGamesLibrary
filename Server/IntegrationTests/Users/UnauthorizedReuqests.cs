using FluentAssertions;
using IntegrationTests.Unauthorized;
using Server.Models;
using Server.Users.Entities;
using System.Text;
using System.Text.Json;

namespace IntegrationTests.Users
{
    [TestCaseOrderer("IntegrationTests.PriorityOrderer", "IntegrationTests")]
    public class UnauthorizedReuqests : BaseIntegrationTestUnauthorized
    {
        private readonly IntegrationTestWebAppFactoryUnauthorized _factory;
        private readonly HttpClient _client;
        public UnauthorizedReuqests(IntegrationTestWebAppFactoryUnauthorized factory) : base(factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact, TestPriority(0)]
        public async Task AddUsers_Unauthorized_ShouldReturn200()
        {
            //Arrange
            UserEntity userEntity = new UserEntity
            {
                Username = "testuser",
                Email = "test@t.t",
                PasswordHash = "hashedpassword"
                };
            var serializedUser = new StringContent( JsonSerializer.Serialize(userEntity),Encoding.UTF8, "application/json");
            // Act
            var response = await _client.PostAsync("api/Users/register",serializedUser);
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact, TestPriority(1)]
        public async Task AddUsers_Unauthorized_ShouldReturn405()
        {
            //Arrange
            UserEntity userEntity = new UserEntity
            {
                Username = "testuser",
                Email = "test@t.t",
                PasswordHash = "hashedpassword",
                Role = UserRole.User
            };
            var serializedUser = new StringContent(JsonSerializer.Serialize(userEntity), Encoding.UTF8, "application/json");
            // Act
            var response = await _client.PostAsync("api/Users/register", serializedUser);
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        }
        [Fact, TestPriority(1)]
        public async Task Login_Unauthorized_ShouldReturn200()
        {
            //Arrange
            UserEntity userEntity = new UserEntity
            {
                Username = "testuser",
                PasswordHash = "hashedpassword"
            };
            var serializedUser = new StringContent(JsonSerializer.Serialize(userEntity), Encoding.UTF8, "application/json");
            // Act
            var response = await _client.PostAsync("api/Users/login", serializedUser);
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact, TestPriority(1)]
        public async Task Login_Unauthorized_ShouldReturn401()
        {
            //Arrange
            UserEntity userEntity = new UserEntity
            {
                Username = "testuser",
                PasswordHash = "wrongpassword"
            };
            var serializedUser = new StringContent(JsonSerializer.Serialize(userEntity), Encoding.UTF8, "application/json");
            // Act
            var response = await _client.PostAsync("api/Users/login", serializedUser);
            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
