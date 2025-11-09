using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Inventories.Shared;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Inventories.Base;

public partial class InventoryBase(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser)
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Validate(InventoryInput input, Guid userIdAuth, bool isCreate)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        #region dados básicos
        if (!IsNameValid(input.Name) || string.IsNullOrEmpty(input.Name))
        {
            throw new ArgumentException("O nome do item de inventário não é válido.");
        }

        if (!IsDescriptionValid(input.Description))
        {
            throw new ArgumentException("A descrição do item de inventário é muito longa.");
        }

        if (!IsQuantityValid(input.Quantity.GetValueOrDefault()))
        {
            throw new ArgumentException("A quantidade deve ser um número igual ou maior que zero.");
        }

        if (!IsUnitPriceValid(input.UnitPrice))
        {
            throw new ArgumentException("O preço unitário deve ser um valor positivo.");
        }

        if (input.ImageFormFile is not null)
        {
            ValidateMaxSizeFile(file: input.ImageFormFile, maxMegabytes: 3);
        }
        #endregion

        #region duplicidade
        if (isCreate)
        {
            bool exists = await _context.Inventories.AsNoTracking().AnyAsync(x => x.CompanyId == input.CompanyId && x.Name.ToLower() == input.Name.ToLower());

            if (exists)
            {
                throw new InvalidOperationException("Já existe um item de inventário com esse nome nesta empresa.");
            }
        }
        #endregion
    }

    #region extras
    protected static bool IsNameValid(string? name) => !string.IsNullOrWhiteSpace(name) && name.Trim().Length is >= 2 and <= 120;
    protected static bool IsDescriptionValid(string? description) => description == null || description.Trim().Length <= 255;
    protected static bool IsQuantityValid(int quantity) => quantity >= 0;
    protected static bool IsUnitPriceValid(decimal? unitPrice) => unitPrice is null or >= 0;
    #endregion
}