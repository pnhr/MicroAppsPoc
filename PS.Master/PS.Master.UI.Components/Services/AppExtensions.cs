using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Services;

namespace PS.Master.UI.Components.Services
{
    public static class AppExtensions
    {
        public static IServiceCollection AddUILibServices(this IServiceCollection services)
        {
            return services.AddMudServices();
        }
    }
}
