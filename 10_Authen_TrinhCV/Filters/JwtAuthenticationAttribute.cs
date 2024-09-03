using _10_Authen_TrinhCV.Services;
using _10_Authen_TrinhCV.ServicesCommon;
using System.Net;
using System.Security.Claims;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace _10_Authen_TrinhCV.Filters
{
    public class JwtAuthenticationAttribute : AuthorizationFilterAttribute
    {
        private readonly ISecurityService _securityService;
        private readonly IUserServices _userServices;
        private readonly IRoleServices _roleServices;
        private string ROLE;

        public JwtAuthenticationAttribute(string role)
        {
            ROLE = role;
            //_securityService = DI.Kernel.Get<ISecurityService>();
            _userServices = DependencyResolver.Current.GetService<IUserServices>();
            //_userServices = DI.Kernel.Get<IUserServices>();
            //_roleServices = DI.Kernel.Get<IRoleServices>();
            _roleServices = DependencyResolver.Current.GetService<IRoleServices>();
            _securityService = DependencyResolver.Current.GetService<ISecurityService>();
        }

        public override async void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string authenticationToken = "";
            try
            {
                // Get the auth token from the http custom header
                authenticationToken = Convert.ToString(actionContext.Request.Headers.GetValues("ATC-Bearer").FirstOrDefault());
            }
            catch
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "ATC-Bearer header missing");
                return;
            }

            if (string.IsNullOrEmpty(authenticationToken))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "ATC-Bearer token missing");
                return;
            }

            // Validate the token and extract its claims into token info
            string errMsg = "";
            var tokenInfo = _securityService.ValidateToken(authenticationToken, out errMsg);
            if (tokenInfo == null)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Token validation error");
                return;
            }

            //add claims before
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, tokenInfo.Username),
                new Claim("Email", tokenInfo.Email),
                new Claim("Id", tokenInfo.Id),
                new Claim("Username", tokenInfo.Username),
                new Claim("Role", tokenInfo.Role),
            };

            var role = await _roleServices.GetRolenameAsync(tokenInfo.Username);
            if (role == string.Empty && role != ROLE)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to locate user roles");
                return;
            }
            DateTime.TryParse(tokenInfo.ValidTo, out DateTime tokenValidTo);
            if (tokenValidTo > DateTime.UtcNow)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "User session timeout.");
                return;
            }

            var identity = new ClaimsIdentity(claims, "Jwt");
            actionContext.RequestContext.Principal = new ClaimsPrincipal(identity);

            base.OnAuthorization(actionContext);
        }
    }
}
