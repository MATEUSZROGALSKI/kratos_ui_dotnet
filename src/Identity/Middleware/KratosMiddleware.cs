using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Ory.Kratos.Client.Api;
using Ory.Kratos.Client.Client;

namespace Identity.Middleware;

internal sealed class KratosMiddleware
{
    private readonly static IDictionary<string, string> _kratosRedirects = new Dictionary<string, string>()
    {
        ["/account/sign-in"] = "/self-service/login/browser",
        ["/account/sign-up"] = "/self-service/registration/browser",
        ["/account/settings"] = "/self-service/settings/browser",
        ["/account/verification"] = "/self-service/verification/browser"
    };

    private readonly ILogger<KratosMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly IFrontendApiAsync _kratosFrontendService;
    
    public KratosMiddleware(RequestDelegate next, IFrontendApiAsync kratosFrontendService, ILogger<KratosMiddleware> logger)
    {
        _logger = logger;
        _kratosFrontendService = kratosFrontendService;
        _next = next;
    }

    private void Log(HttpContext context, string message)
    {
        _logger.LogInformation($"[{context.TraceIdentifier} {context.Connection.Id} ({context.Connection.RemoteIpAddress})] {message}");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var session = await _kratosFrontendService.ToSessionAsync(cookie: context.Request.Headers.Cookie);
            Log(context, $"SESSION: {JsonConvert.SerializeObject(session)}");
            context.Items["session"] = session;
        }
        catch (ApiException ex)
        {
            //Log(context, ex.Message);

            Log(context, $"ApiException: {JsonConvert.SerializeObject(ex)}");
            if (ex.ErrorCode == 403)
            {
                context.Response.Redirect($"{DockerValues.PublicUrl}/self-service/login/browser?aal=aal2", false, false);
                return;
            }
        }

        var query = context.Request.Query;
        if (!query.ContainsKey("flow"))
        {
            Log(context, $"Flow not specified! Rerouting request to Kratos.");
            var redirectUri = _kratosRedirects[context.Request.Path];
            context.Response.Redirect($"{DockerValues.PublicUrl}{redirectUri}{context.Request.QueryString}");
            return;
        }
        await _next(context);
    }
}
