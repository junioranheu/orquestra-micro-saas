using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orquestra.Application.UseCases.Locations.Cities.Get;
using Orquestra.Application.UseCases.Locations.States.Get;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UtilityController(IGetState getState, IGetCity getCity) : BaseController<UtilityController>
{
    private readonly IGetState _getState = getState;
    private readonly IGetCity _getCity = getCity;

    [ResponseCache(Duration = SystemConsts.OneMonthInSec)]
    [AllowAnonymous] 
    [HttpGet("GetState")]
    public async Task<ActionResult<List<LocationState>?>> GetState()
    {
        var output = await _getState.Execute();
        return output;
    }

    [ResponseCache(Duration = SystemConsts.OneMonthInSec)]
    [AllowAnonymous]
    [HttpGet("GetCity")]
    public async Task<ActionResult<List<LocationCity>?>> GetCity()
    {
        var output = await _getCity.Execute();
        return output;
    }
}