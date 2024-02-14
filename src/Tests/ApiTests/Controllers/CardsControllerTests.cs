using Application.Cards.Commands.DTOs;
using Application.Users.Commands.DTOs;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Tests.ApiTests.Controllers
{
    public class CardsControllerTests : EndpointTestsBase
    {
        private readonly string _createEndpoint;
        private readonly string _getEndpoint;
        private readonly string _listEndpoint;
        private readonly string _updateEndpoint;
        private readonly string _deleteEndpoint;

        public CardsControllerTests(CustomWebApplicationFactory<Program> factory) : base("/api/Cards", factory)
        {
            _createEndpoint = $"{_endpoint}/create";
            _getEndpoint = $"{_endpoint}/get";
            _listEndpoint = $"{_endpoint}/list";
            _updateEndpoint = $"{_endpoint}/update";
            _deleteEndpoint = $"{_endpoint}/delete";
        }

        [Fact]
        public async Task Create_ShouldCreateCard()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

        }

        [Fact]
        public async Task Create_ShouldValidateColorCode()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            Assert.False(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
        }

        [Fact]
        public async Task Create_ShouldCheckIfUserExists()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockMemberToken);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            Assert.False(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
        }

        [Fact]
        public async Task Get_ShouldFetchUserCard()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);

            var content = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(content);
            string cardId = (string)jsonObject["id"]!;

            var getResponse = await _client.GetAsync($"{_getEndpoint}/{cardId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        [Fact]
        public async Task Get_ShouldThrowNotFountIfCardNotExists()
        {
            //Arrange
            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var getResponse = await _client.GetAsync($"{_getEndpoint}/00000000-0000-0000-0000-000000000000");

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            Assert.False(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        [Fact]
        public async Task Get_ShouldThrowAnuthorizedIfNotUserCard()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);

            var content = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(content);
            string cardId = (string)jsonObject["id"]!;

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockMemberToken);
            var getResponse = await _client.GetAsync($"{_getEndpoint}/{cardId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            getResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            Assert.False(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        [Fact]
        public async Task Update_ShouldUpdateUserCard()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var updatedPayload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#FFFFFF",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);

            var content = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(content);
            string cardId = (string)jsonObject["id"]!;

            var updateResponse = await _client.PutAsJsonAsync($"{_updateEndpoint}/{cardId}", updatedPayload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(updateResponse.IsSuccessStatusCode);
            Assert.NotNull(updateResponse.Content);
        }

        [Fact]
        public async Task Update_ShouldValidateCardExists()
        {
            //Arrange
            var updatedPayload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#FFFFFF",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"{_updateEndpoint}/00000000-0000-0000-0000-000000000000", updatedPayload);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            Assert.False(updateResponse.IsSuccessStatusCode);
            Assert.NotNull(updateResponse.Content);
        }

        [Fact]
        public async Task Update_ShouldThrowUnauthorizedIfNotMemberCard()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var updatedPayload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#FFFFFF",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);

            var content = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(content);
            string cardId = (string)jsonObject["id"]!;

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockMemberToken);
            var updateResponse = await _client.PutAsJsonAsync($"{_updateEndpoint}/{cardId}", updatedPayload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            updateResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            Assert.False(updateResponse.IsSuccessStatusCode);
            Assert.NotNull(updateResponse.Content);
        }

        [Fact]
        public async Task Update_AdminShouldUpdateMemberCard()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var updatedPayload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#FFFFFF",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);

            var content = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(content);
            string cardId = (string)jsonObject["id"]!;

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockAdminToken);
            var updateResponse = await _client.PutAsJsonAsync($"{_updateEndpoint}/{cardId}", updatedPayload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(updateResponse.IsSuccessStatusCode);
            Assert.NotNull(updateResponse.Content);
        }

        [Fact]
        public async Task Delete_ShouldDeleteUserCard()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);

            var content = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(content);
            string cardId = (string)jsonObject["id"]!;

            var updateResponse = await _client.DeleteAsync($"{_deleteEndpoint}/{cardId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(updateResponse.IsSuccessStatusCode);
            Assert.NotNull(updateResponse.Content);
        }

        [Fact]
        public async Task Delete_ShouldValidateCardExists()
        {
            //Arrange
            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var updateResponse = await _client.DeleteAsync($"{_deleteEndpoint}/00000000-0000-0000-0000-000000000000");

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            Assert.False(updateResponse.IsSuccessStatusCode);
            Assert.NotNull(updateResponse.Content);
        }

        [Fact]
        public async Task Delete_ShouldThrowUnauthorizedIfNotMemberCard()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);

            var content = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(content);
            string cardId = (string)jsonObject["id"]!;

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockMemberToken);
            var deleteResponse = await _client.DeleteAsync($"{_deleteEndpoint}/{cardId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            Assert.False(deleteResponse.IsSuccessStatusCode);
            Assert.NotNull(deleteResponse.Content);
        }

        [Fact]
        public async Task Delete_AdminShouldDeleteMemberCard()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var updatedPayload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#FFFFFF",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);

            var content = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(content);
            string cardId = (string)jsonObject["id"]!;

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockAdminToken);
            var deleteResponse = await _client.DeleteAsync($"{_deleteEndpoint}/{cardId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(deleteResponse.IsSuccessStatusCode);
            Assert.NotNull(deleteResponse.Content);
        }

        [Fact]
        public async Task List_ShouldListMemberCards()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);
            var getResponse = await _client.GetAsync(_listEndpoint);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        [Fact]
        public async Task List_AdminShouldViewMemberUserCars()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockAdminToken);
            var getResponse = await _client.GetAsync(_listEndpoint);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        [Fact]
        public async Task List_ShouldListMemberCardsWithPagination()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);
            var getResponse = await _client.GetAsync($"{_listEndpoint}?PageIndex=1&PageSize=20");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        [Fact]
        public async Task List_ShouldFilterCardsByName()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);
            var getResponse = await _client.GetAsync($"{_listEndpoint}?Name=test");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        [Fact]
        public async Task List_ShouldFilterCardsByColor()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);
            var getResponse = await _client.GetAsync($"{_listEndpoint}?Color=#000000");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        [Fact]
        public async Task List_ShouldFilterCardsByDate()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);
            var getResponse = await _client.GetAsync($"{_listEndpoint}?FromDate=2024-02-12&ToDate=2025-02-12");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        [Fact]
        public async Task List_ShouldSortCardsByName()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);
            var getResponse = await _client.GetAsync($"{_listEndpoint}?SortBy=Name");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        [Fact]
        public async Task List_ShouldSortCardsByColor()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);
            var getResponse = await _client.GetAsync($"{_listEndpoint}?SortBy=Color");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        [Fact]
        public async Task List_ShouldSortCardsByStatus()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);
            var getResponse = await _client.GetAsync($"{_listEndpoint}?SortBy=Status");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        [Fact]
        public async Task List_ShouldSortCardsByDate()
        {
            //Arrange
            var payload = new CreateCardDTO
            {
                Name = "Test Name",
                Color = "#000000",
                Description = "Test Desctiption"
            };

            var token = await GetRegisteredMemberToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync(_createEndpoint, payload);
            var getResponse = await _client.GetAsync($"{_listEndpoint}?SortBy=Date");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(getResponse.IsSuccessStatusCode);
            Assert.NotNull(getResponse.Content);
        }

        private async Task<string> GetRegisteredMemberToken()
        {
            var payload = new CreateUserDTO
            {
                Email = "test.user7@cardsapi.com",
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

            await _client.PostAsJsonAsync($"/api/Users/register", payload);

            var tokenResponse = await _client.PostAsJsonAsync($"/api/Users/generateToken", loginPayload);
            var token = await tokenResponse.Content.ReadAsStringAsync();

            return token;
        }

        private async Task<string> GetRegisteredAdminToken()
        {
            var payload = new CreateUserDTO
            {
                Email = "test.user8@cardsapi.com",
                Password = "TestUserPassCardsApi1!",
                FirstName = "Test",
                LastName = "User",
                Role = Domain.Common.Enums.UserRole.Admin
            };

            var loginPayload = new LoginUserDTO
            {
                Email = payload.Email,
                Password = payload.Password
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mockAdminToken);

            await _client.PostAsJsonAsync($"/api/Users/register", payload);

            var tokenResponse = await _client.PostAsJsonAsync($"/api/Users/generateToken", loginPayload);
            var token = await tokenResponse.Content.ReadAsStringAsync();

            return token;
        }
    }
}
