namespace PS.Master.UnitTest.UI.ServiceHandlers
{
    public class SampleServiceHandlerTests
    {
        [Fact]
        public async Task GetUsersAsync_WhenCalled_ReturnsListOfSampleUsers()
        {
            var inMemorySettings = new Dictionary<string, string>();
            inMemorySettings.Add("Api:Login", "user/login");
            var configuration = new ConfigurationBuilder()
                                            .AddInMemoryCollection(inMemorySettings)
                                            .Build();

            var ctx = new TestContext();
            var mockHttp = ctx.Services.AddMockHttpClient();
            var httpUrl = "https://localhost";

            List<UserVM> users = new List<UserVM>
            {
                new UserVM { UserId="testuserid" }
            };

            mockHttp.When($"{httpUrl}/*").RespondJson(users);
            var clinet = mockHttp.ToHttpClient();
            clinet.BaseAddress = new Uri(httpUrl + "/");

            SampleServiceHandler sampleServiceHandler = new SampleServiceHandler(configuration, clinet);
            var response = await sampleServiceHandler.GetUsersAsync();

            Assert.Equal(1, response.Count);
        }
    }
}
