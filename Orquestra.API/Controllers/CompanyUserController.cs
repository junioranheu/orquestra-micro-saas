using Microsoft.AspNetCore.Mvc;

namespace Orquestra.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyUserController : BaseController<CompanyUserController>
{
    public IActionResult Index()
    {
        return View();
    }
}