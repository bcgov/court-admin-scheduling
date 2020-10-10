using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SS.Api.Helpers.Extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.services;

namespace SS.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// This cannot be called from AJAX or SWAGGER. It must be loaded in the browser location, because it brings the user to the SSO page. 
        /// </summary>
        /// <param name="redirectUri">URL to go back to.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        [HttpGet("login")]
        public IActionResult Login(string redirectUri = "/api")
        {
            return Redirect(redirectUri);
        }

        /// <summary>
        /// Logout function, should wipe out all cookies. 
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { host = HttpContext.Request.Host} );
        }

        [HttpGet("requestAccess")]
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        public IActionResult RequestAccess()
        {
            return Ok();
        }

        /// <summary>
        /// Must be logged in to call this. 
        /// </summary>
        /// <returns>access_token and refresh_token for API calls.</returns>
        [HttpGet("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetToken()
        {
            var accessToken = await HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "access_token");
            return Ok(new { access_token = accessToken });
        }

        /// <summary>
        /// Provides Permissions, Roles, UserId, HomeLocationId via from claims to JSON.
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("info")]
        public ActionResult UserInfo()
        {
            var isImpersonated = !User.Identity.IsAuthenticated;
            var roles = HttpContext.User.FindAll(ClaimTypes.Role).SelectToList(r => r.Value);
            var permissions = HttpContext.User.FindAll(CustomClaimTypes.Permission).SelectToList(r => r.Value);
            var userId = HttpContext.User.FindFirst(CustomClaimTypes.UserId)?.Value;
            var homeLocationId = HttpContext.User.FindFirst(CustomClaimTypes.HomeLocationId)?.Value;
            return Ok(new { Permissions = permissions, Roles = roles, UserId = userId, HomeLocationId = homeLocationId, isImpersonated });
        }
    }
}
    