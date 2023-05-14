using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Ory.Kratos.Client.Api;
using Ory.Kratos.Client.Client;
using Ory.Kratos.Client.Model;

using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Identity.Common.Authentication;

public sealed class KratosAuthenticationHandler : AuthenticationHandler<KratosAuthenticationOptions>
{
    private readonly string _cookieName;
    private readonly IFrontendApi _kratosApi;

    public KratosAuthenticationHandler(
        IOptionsMonitor<KratosAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IFrontendApi kratosApi
    )
        : base(options, logger, encoder, clock)
    {
        _kratosApi = kratosApi;
        _cookieName = EnvironmentVariables.CookieName;
    }
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            if (Request.Cookies.ContainsKey(_cookieName))
            {
                try
                {
                    var cookie = $"{_cookieName}={Request.Cookies[_cookieName]}";
                    var session = await _kratosApi.ToSessionAsync(cookie: cookie);
                    return ValidateToken(session);
                }
                catch (ApiException ex)
                {
                    if (ex.ErrorCode == 403)
                    {
                        // check session aal
                        // if aal < 3 increment by 1 and redirect to login url with return_to that points to requested url
                        // if aal is the user max aal
                        // redirect to some page saying "unauthorized"
                        Response.Redirect($"{EnvironmentVariables.PublicUrl}/self-service/login/browser?aal=aal2&return_to={Request.Path}");
                        return AuthenticateResult.Fail(ex.Message);
                    }
                }
            }

            return AuthenticateResult.NoResult();
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail(ex.Message);
        }
    }

    private AuthenticateResult ValidateToken(KratosSession session)
    {
        if (session.Identity.State == KratosIdentityState.Inactive)
        {
            return AuthenticateResult.Fail("Session is inactive");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, session.Identity.Id),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new System.Security.Principal.GenericPrincipal(identity, null);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
