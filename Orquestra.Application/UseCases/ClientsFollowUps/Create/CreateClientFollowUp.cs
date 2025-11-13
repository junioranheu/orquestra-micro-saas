using Mapster;
using Orquestra.Application.UseCases.ClientsFollowUps.Base;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.ClientsFollowUps.Create;

public sealed class CreateClientFollowUp(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : ClientFollowUpBase(context, checkIfUserIsLinkedCompanyUser), ICreateClientFollowUp
{
    private readonly Context _context = context;

    public async Task Execute(Guid userIdAuth, ClientFollowUpInput input)
    {
        await Validate(input, userIdAuth);
        await Save(input);
    }

    #region extras
    private async Task Save(ClientFollowUpInput input)
    {
        var clientFollowUp = input.Adapt<ClientFollowUp>();

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

        await _context.AddAsync(clientFollowUp);
        await _context.SaveChangesAsync();
    }
    #endregion
}