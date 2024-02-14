using Domain.Common.Enums;

namespace Tests
{
    public class EndpointTestsBase : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private protected readonly HttpClient _client;
        private protected readonly string _endpoint;

        private protected readonly string _mockAdminToken;
        private protected readonly string _mockMemberToken;

        public EndpointTestsBase(string endpoint, CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _endpoint = endpoint;

            _mockAdminToken = MockJwtToken.MockToken(UserRole.Admin);
            _mockMemberToken = MockJwtToken.MockToken(UserRole.Member);
        }
    }
}
