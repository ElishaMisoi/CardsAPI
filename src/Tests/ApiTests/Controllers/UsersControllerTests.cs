using Application.Users.Commands.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Tests.ApiTests.Controllers
{
    public class UsersControllerTests : EndpointTestsBase
    {
        private readonly string _registerEndpoint;
        private readonly string _generateTokenEndpoint;
        private readonly string _listEndpoint;

        public UsersControllerTests(CustomWebApplicationFactory<Program> factory) : base("/api/Users", factory)
        {
            _registerEndpoint = $"{_endpoint}/register";
            _generateTokenEndpoint = $"{_endpoint}/generateToken";
            _listEndpoint = $"{_endpoint}/list";
        }

        [Fact]
        public async Task Register_ShouldRegisterUserIfAdmin()
        {
            //Arrange
            var payload = new CreateUserDTO
            {
                Email = "test.user@cardsapi.com",
                Password = "TestUserPassCardsApi1!",
                FirstName = "Test",
                LastName = "User",
                Role = Domain.Common.Enums.UserRole.Member
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockAdminToken);

            // Act
            var response = await _client.PostAsJsonAsync(_registerEndpoint, payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
        }

        [Fact]
        public async Task Register_ShouldValidateEmail()
        {
            //Arrange
            var payload = new CreateUserDTO
            {
                Email = "test.user1@cardsapi",
                Password = "TestUserPassCardsApi1!",
                FirstName = "Test",
                LastName = "User",
                Role = Domain.Common.Enums.UserRole.Member
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockAdminToken);

            // Act
            var response = await _client.PostAsJsonAsync(_registerEndpoint, payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            Assert.False(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
        }

        [Fact]
        public async Task Register_ShouldValidatePassword()
        {
            //Arrange
            var payload = new CreateUserDTO
            {
                Email = "test.user2@cardsapi.com",
                Password = "weakpassword",
                FirstName = "Test",
                LastName = "User",
                Role = Domain.Common.Enums.UserRole.Member
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockAdminToken);

            // Act
            var response = await _client.PostAsJsonAsync(_registerEndpoint, payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            Assert.False(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
        }

        [Fact]
        public async Task Register_ShouldNotRegisterUserIfNotAdmin()
        {
            //Arrange
            var payload = new CreateUserDTO
            {
                Email = "test.use3@cardsapi.com",
                Password = "TestUserPassCardsApi1!",
                FirstName = "Test",
                LastName = "User",
                Role = Domain.Common.Enums.UserRole.Member
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockMemberToken);

            // Act
            var response = await _client.PostAsJsonAsync(_registerEndpoint, payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            Assert.False(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
        }

        [Fact]
        public async Task Register_ShouldNotRegisterExistingUser()
        {
            //Arrange
            var payload = new CreateUserDTO
            {
                Email = "test.user4@cardsapi.com",
                Password = "TestUserPassCardsApi1!",
                FirstName = "Test",
                LastName = "User",
                Role = Domain.Common.Enums.UserRole.Member
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockAdminToken);

            // Act
            var response = await _client.PostAsJsonAsync(_registerEndpoint, payload);
            var conflictResponse = await _client.PostAsJsonAsync(_registerEndpoint, payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            conflictResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
            Assert.False(conflictResponse.IsSuccessStatusCode);
            Assert.NotNull(conflictResponse.Content);
        }

        [Fact]
        public async Task GenerateToken_ShouldGenerateToken()
        {
            //Arrange
            var payload = new CreateUserDTO
            {
                Email = "test.user5@cardsapi.com",
                Password = "TestUserPassCardsApi1!",
                FirstName = "Test",
                LastName = "User",
                Role = Domain.Common.Enums.UserRole.Member
            };

            var loginPayload = new LoginUserDTO
            {
                Email = payload.Email,
                Password = payload.Password
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockAdminToken);

            // Act
            var response = await _client.PostAsJsonAsync(_registerEndpoint, payload);
            var tokenResponse = await _client.PostAsJsonAsync(_generateTokenEndpoint, loginPayload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            tokenResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(tokenResponse.IsSuccessStatusCode);
            Assert.NotNull(tokenResponse.Content);
        }

        [Fact]
        public async Task GenerateToken_ShouldValidateUserExists()
        {
            //Arrange
            var payload = new LoginUserDTO
            {
                Email = "non.user@cardsapi.com",
                Password = "NonExistenPassword1!"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_generateTokenEndpoint, payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            Assert.False(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
        }

        [Fact]
        public async Task GenerateToken_ShouldValidatePassword()
        {
            //Arrange
            var payload = new CreateUserDTO
            {
                Email = "test.user6@cardsapi.com",
                Password = "TestUserPassCardsApi1!",
                FirstName = "Test",
                LastName = "User",
                Role = Domain.Common.Enums.UserRole.Member
            };

            var loginPayload = new LoginUserDTO
            {
                Email = payload.Email,
                Password = "WrongPassword"
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockAdminToken);

            // Act
            var response = await _client.PostAsJsonAsync(_registerEndpoint, payload);
            var tokenResponse = await _client.PostAsJsonAsync(_generateTokenEndpoint, loginPayload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            tokenResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            Assert.False(tokenResponse.IsSuccessStatusCode);
            Assert.NotNull(tokenResponse.Content);
        }

        [Fact]
        public async Task List_ShouldListUsers()
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockAdminToken);

            // Act
            var response = await _client.GetAsync(_listEndpoint);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
        }

        [Fact]
        public async Task List_ShouldListUsersWithPagination()
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockAdminToken);

            // Act
            var response = await _client.GetAsync($"{_listEndpoint}?PageIndex=1&PageSize=20");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
        }

        [Fact]
        public async Task List_ShouldNotListUsersIfNotAdmin()
        {
            //Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockMemberToken);

            // Act
            var response = await _client.GetAsync(_listEndpoint);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            Assert.False(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
        }
    }
}
