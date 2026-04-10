using Garagem75.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]

public class DashboardController : Controller

{
    private readonly DashboardApiService _api;

    public DashboardController(DashboardApiService api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        var vm = await _api.Get();

        if (vm == null)
            return View(new DashboardDto());

        return View(vm);
    }
}