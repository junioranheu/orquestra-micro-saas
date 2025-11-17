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

    [ResponseCache(Duration = SystemConsts.Time.HalfDay)]
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

    [ResponseCache(Duration = SystemConsts.Time.OneWeek)]
    [AllowAnonymous]
    [HttpGet("GetControllers")]
    public IActionResult GetControllers()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        var controllers = assembly.GetTypes().
            Where(x => x.IsClass && !x.IsAbstract && typeof(ControllerBase).IsAssignableFrom(x)).
            Select(controller => new
            {
                Controller = controller.Name.Replace("Controller", string.Empty),
                Actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(x => !x.IsSpecialName).Select(x => x.Name).ToList()
            }).
            OrderBy(x => x.Controller).ToList();

        return Ok(controllers);
    }

    [ResponseCache(Duration = SystemConsts.Time.OneYear)]
    [AllowAnonymous]
    [HttpGet("GetState")]
    public async Task<ActionResult> GetState()
    {
        List<LocationState>? output = await _getState.Execute();

        return Ok(output);
    }

    [ResponseCache(Duration = SystemConsts.Time.OneYear)]
    [AllowAnonymous]
    [HttpGet("GetCity")]
    public async Task<ActionResult> GetCity()
    {
        List<LocationCity>? output = await _getCity.Execute();

        return Ok(output);
    }

    [ResponseCache(Duration = SystemConsts.Time.OneYear)]
    [AllowAnonymous]
    [HttpGet("GetCountry")]
    public ActionResult GetCountry()
    {
        List<string> output = GetCountries();

        return Ok(output);
    }

    [AllowAnonymous]
    [HttpGet("GetEnum")]
    public ActionResult GetEnum(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
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

    [ResponseCache(Duration = SystemConsts.Time.HalfDay)]
    [AllowAnonymous]
    [HttpGet("GetPlanType")]
    public ActionResult GetPlanType()
    {
        var planTypeEnum = GetEnumListWithDescriptions<PlanTypeEnum>();

        var plans = planTypeEnum.Select(x =>
        {
            (decimal price, int schedulingLimit, string description, string[] perks, int durationDays) = PlanTypeHelper.GetValues(x.Value);

            return new
            {
                PlanType = x.Value,
                PlanTypeName = x.Name,
                PlanTypeDescription = x.Description,
                Price = price,
                SchedulingLimit = schedulingLimit,
                Description = description,
                Perks = perks,
                DurationDays = durationDays
            };
        }).ToArray();

        var output = new
        {
            SystemConsts.Time.PlanDurationDays,
            SystemConsts.Time.PlanDurationDaysFree,
            Plans = plans
        };

        return Ok(output);
    }

    [AllowAnonymous]
    [HttpGet("GetTestServer")]
    public ActionResult GetTestServer()
    {
        return Ok(true);
    }
}