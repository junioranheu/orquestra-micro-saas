using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.ClientsFollowUps.Base;

public partial class ClientFollowUpBase(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser)
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Validate(ClientFollowUpInput input, Guid userIdAuth)
    {
        Client? client = await _context.Clients.AsNoTracking().Where(x => x.ClientId == input.ClientId && x.Status == true).FirstOrDefaultAsync() ?? throw new ArgumentException(SystemConsts.Warnings.NotFoundClient);

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: client.CompanyId, userId: userIdAuth, needCompanyAdmin: false);

        if (input.ImagesFormFile is not null || input.ImagesFormFile?.Count > 0)
        {
            foreach (var item in input.ImagesFormFile)
            {
                ValidateMaxSizeFile(file: item, maxMegabytes: 3);
            }
        }
    }
}