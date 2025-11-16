using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.ClientsFollowUps.Base;

public partial class ClientFollowUpBase(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser)
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    private readonly int MAX_FILES = 3;
     
    public async Task<Guid> Validate(ClientFollowUpInput input, Guid userIdAuth, bool isCreate)
    {
        if (isCreate)
        {
            if (input.ClientFollowUpStatus != ClientFollowUpStatusEnum.InProgress)
            {
                throw new ArgumentException($"O status de um acompanhamento recém criado deve ser <b>{GetStatusDesc(ClientFollowUpStatusEnum.InProgress).ToLowerInvariant()}</b>.");
            }
        }

        if (!isCreate)
        {
            ClientFollowUp? clientFollowUp = await _context.ClientsFollowUps.AsNoTracking().Where(x => x.ClientFollowUpId == input.ClientFollowUpId && x.Status == true).FirstOrDefaultAsync() ?? throw new ArgumentException(SystemConsts.Warnings.NotFoundData);

            if (clientFollowUp.ClientFollowUpStatus != ClientFollowUpStatusEnum.InProgress)
            {
                throw new ArgumentException($"Apenas acompanhamentos com o status <b>{GetStatusDesc(ClientFollowUpStatusEnum.InProgress).ToLowerInvariant()}</b> podem sem editados.");
            }
        }

        Client? client = await _context.Clients.AsNoTracking().Where(x => x.ClientId == input.ClientId && x.Status == true).FirstOrDefaultAsync() ?? throw new ArgumentException(SystemConsts.Warnings.NotFoundClient);
        Guid companyId = client.CompanyId;

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: companyId, userId: userIdAuth, needCompanyAdmin: false);

        if (input.ImagesFormFile is not null || input.ImagesFormFile?.Count > 0)
        {
            if (input.ImagesFormFile.Count > MAX_FILES)
            {
                throw new ArgumentException($"Você pode subir no máximo {MAX_FILES} anexos por acompanhamento.");
            }

            foreach (var item in input.ImagesFormFile)
            {
                ValidateMaxSizeFile(file: item, maxMegabytes: 3);
            }
        }

        return companyId;
    }

    #region extras
    private static string GetStatusDesc(ClientFollowUpStatusEnum status)
    {
        return GetEnumDesc(status).ToLowerInvariant();
    }
    #endregion
}