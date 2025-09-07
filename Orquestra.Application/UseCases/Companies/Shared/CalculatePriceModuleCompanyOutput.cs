using Orquestra.Domain.Enums;
using System.ComponentModel;

namespace Orquestra.Application.UseCases.Companies.Shared;

public sealed class CalculatePriceModuleCompanyOutput
{
    public required ModuleEnum Module { get; set; }
    public required string ModuleStr { get; set; }
    public required bool CompanyAlreadyHasThisModule { get; set; }

    [Description("Valor original do módulo.")]
    public required decimal OriginalPrice { get; set; }

    [Description("Porcentagem de desconto do módulo.")]
    public required decimal DiscountPercentage { get; set; }

    [Description("Valor do módulo após desconto.")]
    public required decimal DiscountedPrice { get; set; }

    [Description("Valor final do módulo proporcional, já com desconto aplicado e ajustado pelos dias restantes da subscrição atual, se houver.")]
    public required decimal ProportionalPrice { get; set; }
}