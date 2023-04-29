using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Identity.Models;
using Ory.Kratos.Client.Api;

namespace Identity.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IFrontendApiAsync _kratosService;

    public HomeController(IFrontendApiAsync kratosService, ILogger<HomeController> logger)
    {
        _logger = logger;
        _kratosService = kratosService;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("/error")]
    public async Task<IActionResult> Error()
    {
        var query = HttpContext.Request.Query;
        if (!query.ContainsKey("id"))
        {
            _logger.LogError($"[{Activity.Current?.Id ?? HttpContext.TraceIdentifier}]");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        var error = await _kratosService.GetFlowErrorAsync(query["id"]);
        return View(new ErrorViewModel { Error = error.Error?.ToString() ?? "Unknown error", RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
