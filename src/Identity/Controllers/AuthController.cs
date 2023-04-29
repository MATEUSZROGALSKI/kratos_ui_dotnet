using Microsoft.AspNetCore.Mvc;

using Ory.Kratos.Client.Api;

namespace Identity.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly IFrontendApiAsync _kratosService;

    public AuthController(IFrontendApiAsync kratosService, ILogger<AuthController> logger)
    {
        _logger = logger;
        _kratosService = kratosService;
    }

    [HttpGet("sign-in")]
    public async Task<IActionResult> SignIn()
    {
        var query = HttpContext.Request.Query;
        if (!query.ContainsKey("flow"))
        {
            return Redirect($"http://127.0.0.1:4433/self-service/login/browser{HttpContext.Request.QueryString}");
        }
        var cookie = HttpContext.Request.Headers.Cookie;
        var flowId = HttpContext.Request.Query["flow"];
        var flow = await _kratosService.GetLoginFlowAsync(flowId, cookie);
        _logger.LogTrace("Rendering sign-in flow with cookie: {cookie} and flow: {flow}", cookie, flow.ToJson());
        return View(flow);
    }

    [HttpGet("sign-up")]
    public async Task<IActionResult> SignUp()
    {
        var query = HttpContext.Request.Query;
        if (!query.ContainsKey("flow"))
        {
            return Redirect($"http://127.0.0.1:4433/self-service/registration/browser{HttpContext.Request.QueryString}");
        }
        var cookie = HttpContext.Request.Headers.Cookie;
        var flowId = HttpContext.Request.Query["flow"];
        var flow = await _kratosService.GetRegistrationFlowAsync(flowId, cookie);
        _logger.LogTrace("Rendering sign-up flow with cookie: {cookie} and flow: {flow}", cookie, flow.ToJson());
        return View(flow);
    }
}
