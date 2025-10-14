using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Companies.CalculatePrice;

public sealed class CalculatePriceModuleCompany(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : ICalculatePriceModuleCompany
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<List<CalculatePriceModuleCompanyOutput>> Execute(Guid userIdAuth, Guid companyId, ModuleEnum[]? modules)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        Company? company = await _context.Companies.AsNoTracking().
                           Where(x => x.CompanyId == companyId).
                           FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundCompany);

        // Se por param não vier nenhum módulo, busque todos;
        if (modules is null || modules.Length == 0)
        {
            modules = Enum.GetValues<ModuleEnum>();
        }

        List<CalculatePriceModuleCompanyOutput> output = [];

        DateTime? planStartDate = company.PlanStartDate;
        DateTime? planEndDate = company.PlanEndDate;
        int diffDays = (planEndDate - planStartDate)?.Days ?? 0;
        int daysInMonth = 0;

        if (diffDays > 0 && (planStartDate is not null))
        {
            daysInMonth = DateTime.DaysInMonth(planStartDate.Value.Year, planStartDate.Value.Month);
        }

        foreach (var item in modules)
        {
            decimal originalPrice = ModuleHelper.GetOriginalPrice(item);
            decimal discountPercent = ModuleHelper.GetDiscount(item);
            decimal discountedPrice = ModuleHelper.GetPrice(item);
            decimal proportionalPrice = discountedPrice;

            if (diffDays > 0 && (planStartDate is not null))
            {
                proportionalPrice = decimal.Round((originalPrice * diffDays / daysInMonth), 2, MidpointRounding.AwayFromZero);
            }

            CalculatePriceModuleCompanyOutput calculate = new()
            {
                Module = item,
                ModuleStr = GetEnumDesc(item),
                CompanyAlreadyHasThisModule = company.Modules is not null && company.Modules.Length != 0 && company.Modules.Any(x => x == item),
                OriginalPrice = originalPrice,
                DiscountPercentage = discountPercent,
                DiscountedPrice = discountedPrice,
                ProportionalPrice = proportionalPrice
            };

            output.Add(calculate);
        }

         return output;
    }
}