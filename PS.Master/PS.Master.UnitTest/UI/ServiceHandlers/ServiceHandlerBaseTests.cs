namespace PS.Master.UnitTest.UI.ServiceHandlers
{
    public class ServiceHandlerBaseTests : ServiceHandlerBase
    {
        public ServiceHandlerBaseTests() : base(ConstructorHelper.GetTestConfigFile(), ConstructorHelper.GetTestHttpClient())
        {
        }

        [Fact]
        public async Task GetServiceUri_WhenCalledWithNullAsSecondParamter_ReturnsEncodedServiceUriWithoutQueryStrings()
        {
            string uri = GetServiceUri("Api:Login", null);
            Assert.Equal("user/login", uri);
        }

        [Fact]
        public async Task GetServiceUri_WhenCalledWithOneQueryString_ReturnsEncodedServiceUri()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("userid", "corp\\e999999");
            string uri = GetServiceUri("Api:Login", keyValuePairs);
            Assert.Equal("user/login?userid=corp%5Ce999999", uri);
        }
        [Fact]
        public async Task GetServiceUri_WhenCalledWithMoreThanOneQueryString_ReturnsEncodedServiceUriWithAllQueryStrinsSeperatedByAndSymbol()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("userid", "corp\\e999999");
            keyValuePairs.Add("firstname", "test firstname");
            string uri = GetServiceUri("Api:Login", keyValuePairs);
            Assert.Equal("user/login?userid=corp%5Ce999999&firstname=test+firstname", uri);
        }
        [Fact]
        public async Task ReadApiResponseAsync_WhenCalledWithSomeErrorStatusCode_ReturnsDefaultOfTheGivenDatatype()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.StatusCode = System.Net.HttpStatusCode.NotImplemented;
            string result = await ReadApiResponseAsync<string>(httpResponseMessage);
            Assert.Null(result);
        }

        [Fact]
        public async Task Post_WhenCalledWithViewModel_ReturnsResponseOfGivenType()
        {
            UserVM user = new UserVM();
            var response = await Post<UserVM, UserVM>(user, "Api:Post");
            Assert.Equal("test post response", response.FirstName);
        }

        [Fact]
        public async Task Post_WhenCalledWithViewModelAsNull_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAnyAsync<ArgumentNullException>(() => Post<UserVM, UserVM>(null, "Api:Post"));
        }

        [Fact]
        public async Task Post_WhenCalledWithStringValueAsParamter_ReturnsResponseOfGivenType()
        {
            var response = await Post<UserVM>("this_is_test_string", "Api:Post");
            Assert.Equal("test post response", response.FirstName);
        }
    }
}
