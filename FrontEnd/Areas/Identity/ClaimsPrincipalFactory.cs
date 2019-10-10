using System.Security.Claims;
using System.Threading.Tasks;
using FrontEnd.Data;
using FrontEnd.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

[assembly: HostingStartup(typeof(FrontEnd.Areas.Identity.IdentityHostingStartup))]
namespace FrontEnd.Areas.Identity
{
    public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
    {
        private readonly IApiClient _apiClient;

        public ClaimsPrincipalFactory(IApiClient apiClient, UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
            _apiClient = apiClient;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (user.IsAdmin)
            {
                identity.MakeAdmin();
            }

            return identity;
        }
    }
}