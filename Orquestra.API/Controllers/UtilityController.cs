using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.Application.UseCases.Locations.Cities.Get;
using Orquestra.Application.UseCases.Locations.States.Get;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Registry;
using System.Reflection;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UtilityController(IGetState getState, IGetCity getCity) : BaseController<UtilityController>
{
    private readonly IGetState _getState = getState;
    private readonly IGetCity _getCity = getCity;

    [ResponseCache(Duration = SystemConsts.HalfDayInSec)]
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

    [ResponseCache(Duration = SystemConsts.OneHourInSec)]
    [AllowAnonymous]
    [HttpGet("GetModuleEnum")]
    public ActionResult GetModuleEnum()
    {
        var output = Enum.GetValues<ModuleEnum>().
            Select(x => new
            {
                Id = (int)x,
                Module = x,
                Name = x.ToString(),
                Description = GetEnumDesc(x),
                Price = ModuleHelper.GetOriginalPrice(x),
                Discount = ModuleHelper.GetDiscount(x)
            }).ToList();

        return Ok(output);
    }

    [AllowAnonymous]
    [HttpGet("GetEnum")]
    public ActionResult GetEnum(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) { 
            return BadRequest("Nome do enum inválido.");
        }

        if (!EnumRegistry.TryGetEnum(name, out Type? enumType))
        {
            string validEnums = string.Join(", ", EnumRegistry.GetEnumNames());
            return BadRequest($"O enum '{name}' não foi encontrado. Enums disponíveis: {validEnums}.");
        }

        List<DropdownOptionOutput<int>> values = [.. Enum.GetValues(enumType!).
            Cast<Enum>().
            Select(x => new DropdownOptionOutput<int>
            {
                Value = Convert.ToInt32(x),
                Label = GetEnumDesc(x)
            }).
            OrderBy(x => x.Label)];

        return Ok(values);
    }
}