using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace net5backendWithIdentityAndAD.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("/error")]
        public async Task<IActionResult> Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (context.Error is MsalUiRequiredException ex)
            {
                var appBaseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                var azureLogInUrl = $"{appBaseUrl}/MicrosoftIdentity/Account/SignIn";
                await Request.HttpContext.SignOutAsync();
                return Redirect(azureLogInUrl);
            }

            return Problem();
        }
    }
}