using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Application.UseCases.Integrations.Whatsapp.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Integrations.Whatsapp.Base;

public record IntegrationWhatsappBaseDependencies(
    Context Context,
    ICheckIfUserIsLinkedCompanyUser CheckIfUserIsLinkedCompanyUser,
    IGetCurrentMainCompanyUser GetCurrentMainCompanyUser
);

public partial class IntegrationWhatsappBase(IntegrationWhatsappBaseDependencies deps)
{
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = deps.CheckIfUserIsLinkedCompanyUser;
    private readonly IGetCurrentMainCompanyUser _getCurrentMainCompanyUser = deps.GetCurrentMainCompanyUser;

    public async Task Validate(Guid userIdAuth, IntegrationWhatsappMessageInput input)
    {
        (CompanyOutput? currentMainCompany, bool _) = await _getCurrentMainCompanyUser.Execute(userId: userIdAuth);

        if (currentMainCompany is null)
        {
            throw new InvalidOperationException(SystemConsts.Warnings.NotLinkedOrDontHaveCompany);
        }

        if (currentMainCompany.PlanType == PlanTypeEnum.Free)
        {
            throw new InvalidOperationException($"O plano <b>{GetEnumDesc(PlanTypeEnum.Free).ToLowerInvariant}</b> não tem acesso às integrações do WhatsApp.");
        }

        if (currentMainCompany.CompanySituation == CompanySituationEnum.PendingPayment)
        {
            throw new InvalidOperationException($"Sua empresa está na situação de <b>{GetEnumDesc(CompanySituationEnum.PendingPayment).ToLowerInvariant}</b>, portanto não pode prosseguir com esta ação.");
        }

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: currentMainCompany.CompanyId, userId: userIdAuth, needCompanyAdmin: false);
    }
}