using System.Security.Claims;

namespace PS.Master.UnitTest.UI.Auth
{
    public class AppAuthenticationStateProviderTests
    {
        [Fact]
        public async Task GetAuthenticationStateAsync_WhenLoginIsValid_ReturnAuthenticaitonStateObjectWithUserIdClaim()
        {
            var userServiceHandlerMock = new Mock<IUserServiceHandler>();
            var accessTokenServiceMock = new Mock<IAccessTokenService>();

            userServiceHandlerMock.Setup(x => x.LoginAsync()).ReturnsAsync(new AuthenticationResponse { Token = "testtoken" });
            userServiceHandlerMock.Setup(x => x.GetLoginUserDetailsAsync()).ReturnsAsync(new UserVM { UserId = "testuserid", FirstName = "TestFirstName" });
            accessTokenServiceMock.Setup(x => x.SetAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            AppAuthenticationStateProvider obj = new AppAuthenticationStateProvider(userServiceHandlerMock.Object, accessTokenServiceMock.Object);
            var response = await obj.GetAuthenticationStateAsync();
            var userIdClaim = response.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);
            Assert.Equal("testuserid", userIdClaim.Value);
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_WhenLoginIsNotValid_ReturnEmptyAuthenticaitonStateObject()
        {
            var userServiceHandlerMock = new Mock<IUserServiceHandler>();
            var accessTokenServiceMock = new Mock<IAccessTokenService>();

            userServiceHandlerMock.Setup(x => x.LoginAsync()).ReturnsAsync(default(AuthenticationResponse));
            userServiceHandlerMock.Setup(x => x.GetLoginUserDetailsAsync()).ReturnsAsync(default(UserVM));
            accessTokenServiceMock.Setup(x => x.SetAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            AppAuthenticationStateProvider obj = new AppAuthenticationStateProvider(userServiceHandlerMock.Object, accessTokenServiceMock.Object);
            var response = await obj.GetAuthenticationStateAsync();
            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetAuthenticationStateAsync_WhenLoginIsValidButUserDetailsNotFound_ReturnEmptyAuthenticaitonStateObject()
        {
            var userServiceHandlerMock = new Mock<IUserServiceHandler>();
            var accessTokenServiceMock = new Mock<IAccessTokenService>();

            userServiceHandlerMock.Setup(x => x.LoginAsync()).ReturnsAsync(new AuthenticationResponse { Token = "testtoken" });
            userServiceHandlerMock.Setup(x => x.GetLoginUserDetailsAsync()).ReturnsAsync(default(UserVM));
            accessTokenServiceMock.Setup(x => x.SetAccessTokenAsync(It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            AppAuthenticationStateProvider obj = new AppAuthenticationStateProvider(userServiceHandlerMock.Object, accessTokenServiceMock.Object);
            var response = await obj.GetAuthenticationStateAsync();
            Assert.NotNull(response);
        }
    }
}
