using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.ClientsFollowUps.Base;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.ClientsFollowUps.Update;

public sealed class UpdateClientFollowUp(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : ClientFollowUpBase(context, checkIfUserIsLinkedCompanyUser), IUpdateClientFollowUp
{
    private readonly Context _context = context;

    public async Task Execute(Guid userIdAuth, ClientFollowUpInput input)
    {
        ClientFollowUp? clientFollowUp = await _context.ClientsFollowUps.
            // AsNoTracking(). // Propositalmente sem AsNoTracking;
            Where(x => x.ClientFollowUpId == input.ClientFollowUpId).
            FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundData);

        await Validate(input, userIdAuth, isCreate: false);
        await Update(input, clientFollowUp);
    }

    #region extras
    private async Task Update(ClientFollowUpInput input, ClientFollowUp clientFollowUp)
    {
        clientFollowUp.Observation = input.Observation;
        clientFollowUp.ClientFollowUpStatus = input.ClientFollowUpStatus;

        if (input.ImagesFormFile is not null || input.ImagesFormFile?.Count > 0)
        {
            foreach (var item in input.ImagesFormFile)
            {
                using MemoryStream ms = new();
                await item.CopyToAsync(ms);
                byte[] array = ms.ToArray();
                clientFollowUp.Images?.Add(array);
                clientFollowUp.ImagesContentType?.Add(item.ContentType);
            }
        }
        else
        {
            clientFollowUp.Images = [];
            clientFollowUp.ImagesContentType = null;
        }

        _context.Update(clientFollowUp);
        await _context.SaveChangesAsync();
    }
    #endregion
}