using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using net5backendWithIdentityAndAD.Models;
using net5backendWithIdentityAndAD.Overrides;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace net5backendWithIdentityAndAD.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        private readonly ITokenAcquisition tokenAcquisition;
        private readonly WebOptions webOptions;

        public TestController(ITokenAcquisition tokenAcquisition, IOptions<WebOptions> webOptionValue)
        {

            this.tokenAcquisition = tokenAcquisition;
            webOptions = webOptionValue.Value;
        }

        [Authorize(Policy = "admin")]
        //[AuthorizeForScopes(Scopes = new[] { "User.Read" })]
        public async Task<IActionResult> IndexAsync()
        {
            try
            {
                //Identity Data
                var currentUser = HttpContext.User;
                ViewData["Me"] = currentUser.Identity.Name;

                //Graph Data            
                GraphServiceClient graphClient = GetGraphServiceClient(new[] { "User.Read" });
                var me = await graphClient.Me.Request().GetAsync();
                ViewData["Me"] = me.DisplayName;
            }
            catch (Exception ex)
            {
                return new ChallengeResult(OpenIdConnectDefaults.AuthenticationScheme);
            }                      

            return View();
        }


        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private GraphServiceClient GetGraphServiceClient(string[] scopes)
        {
            return GraphServiceClientFactory.GetAuthenticatedGraphClient(async () =>
            {
                string result = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                return result;
            }, webOptions.GraphApiUrl);
        }
    }
}