using Identity.Models;

using Microsoft.AspNetCore.Mvc;

using Ory.Kratos.Client.Api;
using Ory.Kratos.Client.Client;

using System.Diagnostics;

namespace Identity.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IFrontendApiAsync _kratosService;

    public AccountController(IFrontendApiAsync kratosService, ILogger<AccountController> logger)
    {
        _logger = logger;
        _kratosService = kratosService;
    }

    [HttpGet("sign-out")]
    public new async Task<IActionResult> SignOut()
    {
        try
        {
            var cookie = HttpContext.Request.Headers.Cookie;
            var flow = await _kratosService.CreateBrowserLogoutFlowAsync(cookie);
            return Redirect($"{flow.LogoutUrl}{HttpContext.Request.QueryString}");
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel
            {
                Error = ex.Message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }

    [HttpGet("settings")]
    public async Task<IActionResult> Settings([FromQuery(Name = "flow")] string flowId)
    {
        try
        {
            var cookie = HttpContext.Request.Headers.Cookie;
            var flow = await _kratosService.GetSettingsFlowAsync(flowId, cookie);
            return View(flow);
        }
        catch (ApiException ex)
        {
            if (ex.ErrorCode == 403 || ex.ErrorCode == 401)
            {
                return Redirect($"{DockerValues.PublicUrl}/self-service/login/browser?aal=aal2");
            }

            return View("Error", new ErrorViewModel
            {
                Error = ex.Message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }

    [HttpGet("verification")]
    public async Task<IActionResult> Verification([FromQuery(Name = "flow")] string flowId)
    {
        try
        {
            var cookie = HttpContext.Request.Headers.Cookie;
            var flow = await _kratosService.GetVerificationFlowAsync(flowId, cookie);
            return View(flow);
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel
            {
                Error = ex.Message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }

    [HttpGet("sign-in")]
    public async Task<IActionResult> SignIn([FromQuery(Name = "flow")] string flowId)
    {
        try
        {
            var cookie = HttpContext.Request.Headers.Cookie;
            var flow = await _kratosService.GetLoginFlowAsync(flowId, cookie);
            return View(flow);
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel
            {
                Error = ex.Message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }

    [HttpGet("sign-up")]
    public async Task<IActionResult> SignUp([FromQuery(Name = "flow")] string flowId)
    {
        try
        {
            var cookie = HttpContext.Request.Headers.Cookie;
            var flow = await _kratosService.GetRegistrationFlowAsync(flowId, cookie);
            return View(flow);
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel
            {
                Error = ex.Message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}