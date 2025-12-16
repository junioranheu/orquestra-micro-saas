using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Quotes.Shared;
using static Orquestra.Utils.Fixtures.CommonForBases;

namespace Orquestra.Application.UseCases.Quotes.Base;

public partial class QuoteBase(ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser)
{
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Validate(QuoteInput input, Guid userIdAuth)
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

        IsDateValid(input, x => x.ValidUntil, (x, value) => x.ValidUntil = value);

        if (input.Items is null || input.Items.Count == 0)
        {
            throw new ArgumentException("Nenhum item foi adicionado ao orçamento.");
        }

        foreach (var item in input.Items)
        {
            if (!IsNameValid(item.Title) || string.IsNullOrEmpty(item.Title))
            {
                throw new ArgumentException($"O nome de um dos itens não é válido: <b>{item.Title}</b>.");
            }

            if (!IsQuantityValid(item.Quantity))
            {
                throw new ArgumentException($"<b>{item.Title}</b>: a quantidade deve ser um número igual ou maior que zero.");
            }

            if (!IsUnitPriceValid(item.UnitPrice))
            {
                throw new ArgumentException($"<b>{item.Title}</b>: o preço unitário deve ser um valor positivo.");
            }
        }
    }
}