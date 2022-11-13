namespace PS.Master.UnitTest.UI.Auth
{
    public class AppAccessTokenServiceTests
    {
        [Fact]
        public async Task GetAccessTokenAsync_WhenCalled_ReturnsToken()
        {
            using var ctx = new TestContext();
            ctx.JSInterop.Setup<string>("localStorage.getItem", It.IsAny<string>()).SetResult("testtoken");

            AppAccessTokenService appAccessTokenService = new AppAccessTokenService(ctx.JSInterop.JSRuntime);

            string token = await appAccessTokenService.GetAccessTokenAsync(It.IsAny<string>());

            Assert.Equal("testtoken", token);
        }
        [Fact]
        public async Task SetAccessTokenAsync_WhenCalled_SavesTokenAndReturnNothing()
        {
            using var ctx = new TestContext();
            var ctxSetup = ctx.JSInterop.SetupVoid("localStorage.setItem", "testtoken", "testtokenvalue").SetVoidResult();

            AppAccessTokenService appAccessTokenService = new AppAccessTokenService(ctx.JSInterop.JSRuntime);

            await appAccessTokenService.SetAccessTokenAsync("testtoken", "testtokenvalue");

            ctxSetup.VerifyInvoke("localStorage.setItem");
        }

        [Fact]
        public async Task RemoveAccessTokenAsyncc_WhenCalled_RemovesTokenAndReturnNothing()
        {
            using var ctx = new TestContext();
            var ctxSetup = ctx.JSInterop.SetupVoid("localStorage.removeItem", "testtoken").SetVoidResult();

            AppAccessTokenService appAccessTokenService = new AppAccessTokenService(ctx.JSInterop.JSRuntime);

            await appAccessTokenService.RemoveAccessTokenAsync("testtoken");

            ctxSetup.VerifyInvoke("localStorage.removeItem");
        }
    }
}
