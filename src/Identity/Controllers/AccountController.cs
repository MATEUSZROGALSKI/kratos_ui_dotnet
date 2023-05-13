using Identity.Common.Authentication.Attributes;
using Identity.Models;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Ory.Kratos.Client.Api;
using Ory.Kratos.Client.Client;

using System.Diagnostics;

namespace Identity.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IFrontendApi _kratosService;

    public AccountController(IFrontendApi kratosService, ILogger<AccountController> logger)
    {
        _logger = logger;
        _kratosService = kratosService;
    }

    private void Log(string message)
    {
        var context = HttpContext;
        _logger.LogInformation($"[{context.TraceIdentifier} {context.Connection.Id} ({context.Connection.RemoteIpAddress})] {message}");
    }


    [HttpGet("sign-out"), KratosAuthorized]
    public new async Task<IActionResult> SignOut()
    {
        try
        {
            var cookie = HttpContext.Request.Headers.Cookie;
            var flow = await _kratosService.CreateBrowserLogoutFlowAsync(cookie);
            Log(JsonConvert.SerializeObject(flow));
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
            var flow = await _kratosService.GetSettingsFlowAsync(flowId, cookie: cookie);
            Log(JsonConvert.SerializeObject(flow));
            return View(flow);
        }
        catch (ApiException ex)
        {
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
            var flow = await _kratosService.GetVerificationFlowAsync(flowId, cookie: cookie);
            Log(JsonConvert.SerializeObject(flow));
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
            var flow = await _kratosService.GetLoginFlowAsync(flowId, cookie:cookie);
            Log(JsonConvert.SerializeObject(flow));
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
            var flow = await _kratosService.GetRegistrationFlowAsync(flowId, cookie: cookie);
            Log(JsonConvert.SerializeObject(flow));
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