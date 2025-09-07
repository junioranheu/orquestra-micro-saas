using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.Application.UseCases.Locations.Cities.Get;
using Orquestra.Application.UseCases.Locations.States.Get;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using System.ComponentModel;
using System.Reflection;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UtilityController(IGetState getState, IGetCity getCity) : BaseController<UtilityController>
{
    private readonly IGetState _getState = getState;
    private readonly IGetCity _getCity = getCity;

    [ResponseCache(Duration = SystemConsts.OneHourInSec)]
    [AllowAnonymous]
    [HttpGet("GetBuildVersion")]
    public ActionResult GetBuildVersion()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string version = assembly.GetName().Version?.ToString() ?? "-";

        var build = new
        {
            BuildVersion = version,
            AssemblyName = assembly.GetName().Name,
            Configuration =
#if DEBUG
            "Debug"
#else
            "Release"
#endif
        };

        return Ok(build);
    }

    [AllowAnonymous]
    [HttpGet("GetModulesInfo")]
    public ActionResult GetModulesInfo()
    {
        var modules = Enum.GetValues<ModuleEnum>().
            Select(x =>
            {
                string description = x.GetType().GetField(x.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;

                decimal originalPrice = ModuleHelper.GetOriginalPrice(x);
                decimal price = ModuleHelper.GetPrice(x);
                decimal discountPercent = ModuleHelper.GetDiscount(x);

                return new
                {
                    Name = x.ToString(),
                    Description = description,
                    OriginalPrice = originalPrice,
                    DiscountPercent = discountPercent,
                    Price = price,
                };
            });

        return Ok(modules);
    }

    [ResponseCache(Duration = SystemConsts.OneMonthInSec)]
    [AllowAnonymous]
    [HttpGet("GetState")]
    public async Task<ActionResult> GetState()
    {
        List<LocationState>? output = await _getState.Execute();

        return Ok(output);
    }

    [ResponseCache(Duration = SystemConsts.OneMonthInSec)]
    [AllowAnonymous]
    [HttpGet("GetCity")]
    public async Task<ActionResult> GetCity()
    {
        List<LocationCity>? output = await _getCity.Execute();

        return Ok(output);
    }
}