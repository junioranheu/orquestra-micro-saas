using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.RegexPatterns;

namespace Orquestra.Application.UseCases.Integrations.WhatsApp.Base;

public record IntegrationWhatsAppBaseDependencies(
    Context Context,
    ICheckIfUserIsLinkedCompanyUser CheckIfUserIsLinkedCompanyUser,
    IGetCurrentMainCompanyUser GetCurrentMainCompanyUser
);

public partial class IntegrationWhatsAppBase(IntegrationWhatsAppBaseDependencies deps)
{
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = deps.CheckIfUserIsLinkedCompanyUser;
    private readonly IGetCurrentMainCompanyUser _getCurrentMainCompanyUser = deps.GetCurrentMainCompanyUser;

    public async Task Validate(Guid userIdAuth)
    {
        (CompanyOutput? currentMainCompany, bool _) = await _getCurrentMainCompanyUser.Execute(userId: userIdAuth);

        if (currentMainCompany is null)
        {
            throw new InvalidOperationException(SystemConsts.Warnings.NotLinkedOrDontHaveCompany);
        }
        //else
        //{
        //    input.CompanyName = currentMainCompany.Name;
        //}

        //if (currentMainCompany.PlanType == PlanTypeEnum.Free)
        //{
        //    throw new InvalidOperationException($"O plano <b>{GetEnumDesc(PlanTypeEnum.Free).ToLowerInvariant()}</b> não tem acesso às integrações do WhatsApp.");
        //}

        //if (currentMainCompany.CompanySituation == CompanySituationEnum.PendingPayment)
        //{
        //    throw new InvalidOperationException($"Sua empresa está na situação de <b>{GetEnumDesc(CompanySituationEnum.PendingPayment).ToLowerInvariant()}</b>, portanto não pode prosseguir com esta ação.");
        //}

        //if (!IsPhoneValid(input.Phone))
        //{
        //    throw new ArgumentException("O número de telefone não é válido. Insira um número válido, por favor.");
        //}

        //if (string.IsNullOrEmpty(input.Message))
        //{
        //    throw new ArgumentException("O conteúdo da mensagem está vazio. Insira texto válido, por favor.");
        //}

        //if (!IsFullNameValid(input.FromFullName))
        //{
        //    throw new ArgumentException("O nome não é válido. Insira um nome completo, por favor.");
        //}

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: currentMainCompany.CompanyId, userId: userIdAuth, needCompanyAdmin: false);
    }

    #region extras
    private static bool IsFullNameValid(string fullName)
    {
        return RegexName().IsMatch(fullName);
    }

    private static bool IsPhoneValid(string? phone)
    {
        return RegexPhone().IsMatch(phone ?? string.Empty);
    }
    #endregion
}