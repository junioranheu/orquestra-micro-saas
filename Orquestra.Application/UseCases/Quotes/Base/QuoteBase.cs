using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Quotes.Shared;

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

        if (input.Items is null || input.Items.Count == 0)
        {
            throw new ArgumentException("Nenhum item foi adicionado ao orçamento.");
        }

        foreach (var item in input.Items)
        {
            if (!IsNameValid(item.Title) || string.IsNullOrEmpty(input.Title))
            {
                throw new ArgumentException("O nome do item não é válido.");
            }

            if (!IsDescriptionValid(item.Description))
            {
                throw new ArgumentException("A descrição do item não é válida.");
            }

            if (!IsQuantityValid(item.Quantity))
            {
                throw new ArgumentException($"{item.Title}: a quantidade deve ser um número igual ou maior que zero.");
            }

            if (!IsUnitPriceValid(item.UnitPrice))
            {
                throw new ArgumentException($"{item.Title}: O preço unitário deve ser um valor positivo.");
            }
        }
    }

    #region extras
    protected static bool IsNameValid(string? name) => !string.IsNullOrWhiteSpace(name) && name.Trim().Length is >= 2 and <= 120;
    protected static bool IsDescriptionValid(string? description) => description == null || description.Trim().Length <= 255;
    protected static bool IsQuantityValid(int quantity) => quantity >= 0;
    protected static bool IsUnitPriceValid(decimal? unitPrice) => unitPrice is null or >= 0;
    #endregion
}