using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PS.Master.ViewModels.Auth;
using System.Security.Claims;

namespace PS.Master.UI.Shared
{
    public partial class MainLayout
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; }

        private string userName = "";
        protected override async Task OnInitializedAsync()
        {
            var authNState = await AuthenticationState;
            if (authNState.User.Identity.IsAuthenticated)
            {
                var firstNameClaim = authNState.User.FindFirst(x => x.Type == AppClaimTypes.FirstName);
                var lastNameClaim = authNState.User.FindFirst(x => x.Type == AppClaimTypes.LastName);

                if (firstNameClaim != null && lastNameClaim != null)
                    userName = $"{firstNameClaim.Value} {lastNameClaim.Value}";
            }
        }

    }
}
