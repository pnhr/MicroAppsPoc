using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.UnitTest.UI.Components
{
    public class SampleTests
    {
        [Fact]
        public async Task Sample_WhenComponentRendered()
        {
            var sampleServiceHandlerMock = new Mock<ISampleServiceHandler>();
            var ctx = new TestContext();
            var authCtx = ctx.AddTestAuthorization();
            authCtx.SetAuthorized("CORP\\e999999");
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            ctx.Services.AddSingleton<ISampleServiceHandler>(sampleServiceHandlerMock.Object);

            sampleServiceHandlerMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(new List<UserVM> { new UserVM { UserId = "CORP\\e777777" } });

            var cut = ctx.RenderComponent<Sample>();

            var tbElement = cut.Find("table > tbody > tr > td:nth-child(1)");

            Assert.Equal("CORP\\e777777", tbElement.TextContent);
        }

        [Fact]
        public async Task Sample_WhenComponentRenderedAndUserObjectIsNull()
        {
            var sampleServiceHandlerMock = new Mock<ISampleServiceHandler>();
            var ctx = new TestContext();
            var authCtx = ctx.AddTestAuthorization();
            authCtx.SetAuthorized("CORP\\e999999");
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            ctx.Services.AddSingleton<ISampleServiceHandler>(sampleServiceHandlerMock.Object);

            sampleServiceHandlerMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(default(List<UserVM>));

            var cut = ctx.RenderComponent<Sample>();

            var tbElement = cut.Find("p > em");

            Assert.Equal("Loading...", tbElement.TextContent);
        }
    }
}
