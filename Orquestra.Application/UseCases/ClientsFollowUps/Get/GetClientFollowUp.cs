using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.ClientsFollowUps.Get;

public sealed class GetClientFollowUp(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetClientFollowUp
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<(IEnumerable<ClientFollowUpOutput> output, int count)> Execute(Guid userIdAuth, ClientFollowUpInput input)
    {
        Client? client = await _context.Clients.AsNoTracking().Where(x => x.ClientId == input.ClientId && x.Status == true).FirstOrDefaultAsync() ?? throw new ArgumentException(SystemConsts.Warnings.NotFoundClient);

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: client.CompanyId, userId: userIdAuth, needCompanyAdmin: false);

        PaginationInput pagination = new()
        {
            IsSelectAll = true
        };

        var query = _context.ClientsFollowUps.
                    AsNoTracking().
                    Where(x => x.ClientId == input.ClientId && x.Status == true).
                    OrderByDescending(x => x.CreatedDate);

        (IEnumerable<ClientFollowUp> result, int count) = await PagedQuery.Execute(query, pagination);

        var output = result.Adapt<List<ClientFollowUpOutput>>();
        NormalizeImage([.. result], output);

        return (output, count);
    }

    #region extras
    private static void NormalizeImage(List<ClientFollowUp>? result, List<ClientFollowUpOutput>? output)
    {
        if (result is null || result.Count == 0 || output is null || output.Count == 0)
        {
            return;
        }

        Dictionary<Guid, ClientFollowUpOutput>? outputById = output?.ToDictionary(x => x.ClientFollowUpId);

        foreach (var company in result)
        {
            if (
                company.Images is not null && company.Images.Count > 0 &&
                company.ImagesContentType is not null && company.ImagesContentType.Count > 0 &&
                outputById!.TryGetValue(company.ClientFollowUpId, out ClientFollowUpOutput? clientFollowUpOutput)
                )
            {
                int i = 0;

                foreach (var item in company.Images)
                {
                    string? contentTypeRelative = company?.ImagesContentType[i] ?? string.Empty;

                    if (string.IsNullOrEmpty(contentTypeRelative))
                    {
                        return;
                    }

                    string base64 = ConvertBytesToBase64(bytes: item, contentType: contentTypeRelative);
                    clientFollowUpOutput.ImagesBase64.Add(base64);
                    i++;
                }
            }
        }
    }
    #endregion
}