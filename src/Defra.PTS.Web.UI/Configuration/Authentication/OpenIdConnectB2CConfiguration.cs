using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Defra.PTS.Web.Application.Features.Users.Commands;
using Defra.PTS.Web.UI.Constants;
using Defra.PTS.Web.UI.Extensions;
using Defra.PTS.Web.UI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Caching;
using System.Security.Claims;
using static Defra.PTS.Web.UI.Constants.WebAppConstants;

namespace Defra.PTS.Web.UI.Configuration.Authentication
{
    [ExcludeFromCodeCoverage]
    public class OpenIdConnectB2CConfiguration
    {

        public string CallbackPath { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string Nonce { get; set; }
        public string Instance { get; set; }
        public string SignedOutCallbackPath { get; set; }
        public string Id { get; } = "oidc";
        public string MetadataAddress { get; set; }
        public string SignUpSignInPolicyId { get; set; }

        public Task HandleRedirectToIdentityProvider(RedirectContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var adB2cSection = configuration.GetSection("AzureAdB2C").Get<OpenIdConnectB2CConfiguration>();

            string serviceId = configuration.GetValue<string>("PTSB2C:PtsB2CTenantServiceId");

            context.ProtocolMessage.RedirectUri = adB2cSection.CallbackPath;
            context.ProtocolMessage.PostLogoutRedirectUri = adB2cSection.SignedOutCallbackPath;
            context.ProtocolMessage.SetParameter("serviceId", serviceId);
            // If the request has come from one of the API endpoints then we dont want the user signing in from here, we 401 instead
            if (context.Request.Path.StartsWithSegments("/api", StringComparison.InvariantCultureIgnoreCase) ||
                (context.Request.Path.StartsWithSegments("/login", StringComparison.InvariantCultureIgnoreCase) && context.Request.QueryString.Value.Contains("ReturnUrl=%2FApi", StringComparison.InvariantCultureIgnoreCase)))
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.HandleResponse();
            }

            return Task.CompletedTask;
        }

        public virtual async Task HandleRemoteSignOut(RemoteSignOutContext ctx)
        {
            var hostHelper = ctx.HttpContext.RequestServices.GetRequiredService<IHostHelper>();
            ctx.HttpContext.Request.Host = new HostString(hostHelper.HostUrl().Replace("https://", ""));
            await Task.WhenAll(
                ctx.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme),
                ctx.HttpContext.SignOutAsync(Id)
            ).ConfigureAwait(false);
            ctx.Response.Redirect("/Signed-out");
            ctx.HandleResponse();
        }

        public async Task HandleTicketReceived(TicketReceivedContext context)
        {
            var metadata = context.Properties;
            var accessTokenValue = metadata.GetTokenValue(Pages.User.AccessTokenKey);
            MemoryCache.Default[Pages.User.AccessTokenKey] = accessTokenValue;

            if (context is { Principal.Identity: ClaimsIdentity identity })
            {
                var mediator = context.HttpContext.RequestServices.GetRequiredService<IMediator>();

                var userInfo = identity.ToUserInfo();
                var response = await mediator.Send(new AddUserRequest(userInfo));
                if (response.IsSuccess)
                {
                    identity.AddClaim(new Claim(WebAppConstants.IdentityKeys.PTSUserId, response.UserId.ToString()));
                }
            }

            await Task.CompletedTask;
        }

    }

}
