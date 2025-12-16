using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.ServiceOrders.Shared;
using static Orquestra.Utils.Fixtures.CommonForBases;

namespace Orquestra.Application.UseCases.ServiceOrders.Base;

public partial class ServiceOrderBase(ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser)
{
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Validate(ServiceOrderInput input, Guid userIdAuth)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: false);

        if (!IsNameValid(input.Title) || string.IsNullOrEmpty(input.Title))
        {
            throw new ArgumentException("O nome do orçamento não é válido.");
        }

        if (!IsDescriptionValid(input.Observation))
        {
            throw new ArgumentException("A observação não é válida.");
        }

        IsDateValid(input, x => x.ExecutionDate, (x, value) => x.ExecutionDate = value);
    }
}