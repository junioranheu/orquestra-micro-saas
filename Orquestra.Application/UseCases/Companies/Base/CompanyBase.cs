using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Companies.Base;

public class CompanyBase(Context context)
{
    private readonly Context _context = context;

    public async Task Validate(CompanyInput input)
    {
        bool checkName = await _context.Companies.AnyAsync(x => x.Name == input.Name);

        if (checkName)
        {
            throw new Exception("Já existe uma empresa registrada com esse nome");
        }
    }
}