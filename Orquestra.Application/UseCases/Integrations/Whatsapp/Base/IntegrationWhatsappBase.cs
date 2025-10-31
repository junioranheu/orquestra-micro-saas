using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;
using static Orquestra.Utils.Fixtures.RegexPatterns;

namespace Orquestra.Application.UseCases.Integrations.WhatsApp.Base;

public record IntegrationWhatsAppBaseDependencies(
    Context Context,
    ICheckIfUserIsLinkedCompanyUser CheckIfUserIsLinkedCompanyUser
);

public partial class IntegrationWhatsAppBase()
{
    public static bool Validate(Company company, bool mustThrow)
    {
        if (company is null)
        {
            if (mustThrow)
            {
                throw new InvalidOperationException(SystemConsts.Warnings.NotLinkedOrDontHaveCompany);
            }

            return false;
        }

        if (company.PlanType == PlanTypeEnum.Free)
        {
            if (mustThrow)
            {
                throw new InvalidOperationException($"O plano <b>{GetEnumDesc(PlanTypeEnum.Free).ToLowerInvariant()}</b> não tem acesso às integrações do WhatsApp.");
            }

            return false;
        }

        if (company.CompanySituation == CompanySituationEnum.PendingPayment)
        {
            if (mustThrow)
            {
                throw new InvalidOperationException($"Sua empresa está na situação de <b>{GetEnumDesc(CompanySituationEnum.PendingPayment).ToLowerInvariant()}</b>, portanto não pode prosseguir com esta ação.");
            }

            return false;
        }

        if (!IsPhoneValid(company.Phone))
        {
            if (mustThrow)
            {
                throw new ArgumentException("O número de telefone não é válido. Insira um número válido, por favor.");
            }

            return false;
        }

        return true;
    }

    #region extras
    private static bool IsPhoneValid(string? phone)
    {
        return RegexPhone().IsMatch(phone ?? string.Empty);
    }
    #endregion
}