using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Inventories.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Inventories.GetAllByCompanyId;

public sealed class GetAllInventoryByCompanyId(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetAllInventoryByCompanyId
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<(IEnumerable<InventoryOutput> output, int count)> Execute(PaginationInput pagination, Guid userIdAuth, Guid companyId, InventoryInput input)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var query = _context.Inventories.
                    AsNoTracking().
                    Where(x =>
                        x.CompanyId == companyId &&
                        x.Status == true &&
                        (string.IsNullOrEmpty(input.Name) || x.Name.ToLower().Contains(input.Name.ToLower())) &&
                        (string.IsNullOrEmpty(input.Description) || x.Description!.ToLower().Contains(input.Description!.ToLower())) &&
                        (input.Quantity.GetValueOrDefault() == 0 || x.Quantity == input.Quantity)
                    ).
                    OrderByDescending(x => x.LastModificationDate ?? DateTime.MinValue).
                    ThenByDescending(x => x.CreatedDate);

        (IEnumerable<Inventory> result, int count) = await PagedQuery.Execute(query, pagination);

        var output = result.Adapt<List<InventoryOutput>>();
        NormalizeImage([.. result], output);

        return (output, count);
    }

    #region extras
    private static void NormalizeImage(List<Inventory>? result, List<InventoryOutput>? output)
    {
        if (result is null || result.Count == 0 || output is null || output.Count == 0)
        {
            return;
        }

        Dictionary<Guid, InventoryOutput>? outputById = output?.ToDictionary(x => x.InventoryId.GetValueOrDefault());

        foreach (var company in result.Where(x => !string.IsNullOrEmpty(x.ImageContentType)).ToList())
        {
            if (company.Image is not null && company.Image.Length > 0 && outputById!.TryGetValue(company.InventoryId, out InventoryOutput? inventoryOutput))
            {
                inventoryOutput.ImageBase64 = ConvertBytesToBase64(bytes: company.Image, contentType: company.ImageContentType);
            }
        }
    }
    #endregion
}