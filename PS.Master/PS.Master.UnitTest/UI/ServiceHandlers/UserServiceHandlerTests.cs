namespace PS.Master.UnitTest.UI.ServiceHandlers
{
    public class UserServiceHandlerTests
    {
        [Fact]
        public async Task GetLoginUserDetailsAsync_WhenCalledWithValidToken_ReturnsUserObject()
        {
            var inMemorySettings = new Dictionary<string, string>();
            inMemorySettings.Add("Api:LoginUserDetails", "user/LoginUserDetails");
            inMemorySettings.Add("Api:IsTokenExpired", "user/IsTokenExpired");
            var configuration = new ConfigurationBuilder()
                                            .AddInMemoryCollection(inMemorySettings)
                                            .Build();

            var ctx = new TestContext();
            var mockHttp = ctx.Services.AddMockHttpClient();
            var httpUrl = "https://localhost";

            mockHttp.When($"{httpUrl}/user/LoginUserDetails").RespondJson(new UserVM { UserId = "testuserid" });
            mockHttp.When($"{httpUrl}/user/IsTokenExpired").RespondJson(false);
            var clinet = mockHttp.ToHttpClient();
            clinet.BaseAddress = new Uri(httpUrl + "/");

            var accessTokenServiceMock = new Mock<IAccessTokenService>();

            UserServiceHandler userServiceHandler = new UserServiceHandler(accessTokenServiceMock.Object, configuration, clinet);

            var result = await userServiceHandler.GetLoginUserDetailsAsync();

            Assert.NotNull(result);
            Assert.Equal("testuserid", result.UserId);
        }

        [Fact]
        public async Task GetLoginUserDetailsAsync_WhenCalledWithExpiredToken_ReturnsNull()
        {
            var inMemorySettings = new Dictionary<string, string>();
            inMemorySettings.Add("Api:LoginUserDetails", "user/LoginUserDetails");
            inMemorySettings.Add("Api:IsTokenExpired", "user/IsTokenExpired");
            var configuration = new ConfigurationBuilder()
                                            .AddInMemoryCollection(inMemorySettings)
                                            .Build();

            var ctx = new TestContext();
            var mockHttp = ctx.Services.AddMockHttpClient();
            var httpUrl = "https://localhost";

            mockHttp.When($"{httpUrl}/user/LoginUserDetails").RespondJson(new UserVM { UserId = "testuserid" });
            mockHttp.When($"{httpUrl}/user/IsTokenExpired").RespondJson(true);
            var clinet = mockHttp.ToHttpClient();
            clinet.BaseAddress = new Uri(httpUrl + "/");

            var accessTokenServiceMock = new Mock<IAccessTokenService>();

            UserServiceHandler userServiceHandler = new UserServiceHandler(accessTokenServiceMock.Object, configuration, clinet);

            var result = await userServiceHandler.GetLoginUserDetailsAsync();

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WhenCalled_ReturnsAuthenticationResponse()
        {
            var inMemorySettings = new Dictionary<string, string>();
            inMemorySettings.Add("Api:Login", "user/Login");
            var configuration = new ConfigurationBuilder()
                                            .AddInMemoryCollection(inMemorySettings)
                                            .Build();

            var ctx = new TestContext();
            var mockHttp = ctx.Services.AddMockHttpClient();
            var httpUrl = "https://localhost";

            mockHttp.When($"{httpUrl}/user/Login").RespondJson(new AuthenticationResponse { Token = "testtoken" });

            var clinet = mockHttp.ToHttpClient();
            clinet.BaseAddress = new Uri(httpUrl + "/");

            var accessTokenServiceMock = new Mock<IAccessTokenService>();

            UserServiceHandler userServiceHandler = new UserServiceHandler(accessTokenServiceMock.Object, configuration, clinet);

            var result = await userServiceHandler.LoginAsync();

            Assert.NotNull(result);
            Assert.Equal("testtoken", result.Token);
        }
    }
}
