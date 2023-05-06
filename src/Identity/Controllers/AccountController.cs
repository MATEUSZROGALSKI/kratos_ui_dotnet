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

    private async Task<IActionResult> FlowCheckAsync<TFlow>(string redirectUri, Func<string, string, Task<TFlow>> getFlowAsync)
    {
        var query = HttpContext.Request.Query;
        if (!query.ContainsKey("flow"))
        {
            return Redirect($"{redirectUri}{HttpContext.Request.QueryString}");
        }
        var cookie = HttpContext.Request.Headers.Cookie;
        var flowId = HttpContext.Request.Query["flow"];
        var flow = await getFlowAsync(flowId!, cookie!);
        return View(flow);
    }

    [HttpGet("sign-out")]
    public new async Task<IActionResult> SignOut()
    {
        var cookie = HttpContext.Request.Headers.Cookie;
        var flow = await _kratosService.CreateBrowserLogoutFlowAsync(cookie);
        return Redirect($"{flow.LogoutUrl}{HttpContext.Request.QueryString}");  
    }

    [HttpGet("settings")]
    public async Task<IActionResult> Settings()
    {
        try
        {
            return await FlowCheckAsync(
                "http://127.0.0.1:4433/self-service/settings/browser",
                async (flow, cookie) => await _kratosService.GetSettingsFlowAsync(flow, cookie));
        }
        catch (ApiException ex)
        {
            if (ex.ErrorCode == 403 || ex.ErrorCode == 401)
            {
                return Redirect("/account/sign-in?aal=aal2&refresh=true");
            }

            return View("Error", new ErrorViewModel
            {
                Error = ex.Message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }

    [HttpGet("verification")]
    public async Task<IActionResult> Verification()
    {
        try
        {
            return await FlowCheckAsync(
                "http://127.0.0.1:4433/self-service/verification/browser",
                async (flow, cookie) => await _kratosService.GetVerificationFlowAsync(flow, cookie));
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
    public async Task<IActionResult> SignIn()
    {
        return await FlowCheckAsync(
            "http://127.0.0.1:4433/self-service/login/browser",
            async (flow, cookie) => await _kratosService.GetLoginFlowAsync(flow, cookie));
    }

    [HttpGet("sign-up")]
    public async Task<IActionResult> SignUp()
    {
        return await FlowCheckAsync(
            "http://127.0.0.1:4433/self-service/registration/browser",
            async (flow, cookie) => await _kratosService.GetRegistrationFlowAsync(flow, cookie));
    }
}