namespace PS.Master.UnitTest.UI.Lib.Components.Layout
{
    public class AppHeaderTests
    {
        [Fact]
        public void AppHeader_WhenRenderedWithOutAuthentication_DisplaysTextAsNotAuthenticated()
        {
            var ctx = new TestContext();
            var cut = ctx.RenderComponent<AppHeader>(parm => parm.Add(p => p.IsAuthenticated, false));

            var text = cut.Find("div > span.text-primary").TextContent;

            Assert.Equal("Not Authenticated", text);

        }
        [Fact]
        public void AppHeader_WhenRenderedWithAuthentication_DisplaysTextAsHelloSpaceUserName()
        {
            var ctx = new TestContext();
            var cut = ctx.RenderComponent<AppHeader>(parm => parm.Add(p => p.IsAuthenticated, true)
                                                                    .Add(p => p.UserName, "Test User"));

            var text = cut.Find("div > span.text-primary").TextContent;

            Assert.Equal("Hello Test User!", text);
        }
    }
}
