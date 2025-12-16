using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.ServiceOrders.Shared;
using static Orquestra.Utils.Fixtures.Get;

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

        IsExecutionDateValid(input);
    }

    #region extras
    protected static bool IsNameValid(string? name) => !string.IsNullOrWhiteSpace(name) && name.Trim().Length is >= 2 and <= 120;
    protected static bool IsDescriptionValid(string? description) => description == null || description.Trim().Length <= 255;

    protected static void IsExecutionDateValid(ServiceOrderInput input)
    {
        // Workaround;
        if (input?.ExecutionDate?.Date == DateTime.MinValue || input?.ExecutionDate?.Date == new DateTime(2001, 1, 1))
        {
            input.ExecutionDate = null;
        }

        if (input?.ExecutionDate is null || input?.ExecutionDate?.Date == DateTime.MinValue || input?.ExecutionDate?.Date == new DateTime(2001, 1, 1))
        {
            // throw new ArgumentException("A data de validade é obrigatória.");
            return;
        }

        // Normalizar a data do input (que vem do front-end), para ficar UTC;
        input!.ExecutionDate = ConvertToBrasiliaTime(input.ExecutionDate.GetValueOrDefault());

        // Comparar apenas com a data;
        DateTime date;

        try
        {
            date = input.ExecutionDate.Value.Date;
        }
        catch
        {
            throw new ArgumentException("A data de validade é inválida.");
        }

        if (date.Date < ConvertToBrasiliaTime(GetDate()).Date)
        {
            throw new ArgumentException("A data de validade não pode ser anterior a hoje.");
        }
    }
    #endregion
}