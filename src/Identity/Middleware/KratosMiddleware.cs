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
    
    public KratosMiddleware(RequestDelegate next, ILogger<KratosMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    private void Log(HttpContext context, string message)
    {
        _logger.LogInformation($"[{context.TraceIdentifier} {context.Connection.Id} ({context.Connection.RemoteIpAddress})] {message}");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var query = context.Request.Query;
        if (!query.ContainsKey("flow"))
        {
            if (_kratosRedirects.TryGetValue(context.Request.Path, out var redirectUri))
            {
                Log(context, $"Flow not specified! Rerouting request to Kratos.");
                context.Response.Redirect($"{EnvironmentVariables.PublicUrl}{redirectUri}{context.Request.QueryString}");
                return;
            }
        }
        await _next(context);
    }
}
