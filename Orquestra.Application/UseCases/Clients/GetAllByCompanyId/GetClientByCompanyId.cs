using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Clients.GetAllByCompanyId;

public sealed class GetClientByCompanyId(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetClientByCompanyId
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<List<ClientOutput>?> Execute(Guid userIdAuth, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var result = await _context.Clients.
                     Include(x => x.Company).
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId && x.Status == true).
                     OrderBy(x => x.FullName).
                     ToListAsync();

        var output = result.Adapt<List<ClientOutput>>();

        return output;
    }

    public async Task<(IEnumerable<ClientOutput> output, int count)> Execute(PaginationInput pagination, ClientInput input, Guid userIdAuth, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        var query = _context.Clients.
                    Include(x => x.Company).
                    AsNoTracking().
                    Where(x =>
                        x.CompanyId == companyId &&
                        x.Status == true &&
                        (string.IsNullOrEmpty(input.FullName) || x.FullName.ToLower().Contains(input.FullName.ToLower())) &&
                        (string.IsNullOrEmpty(input.Email) || x.Email!.ToLower().Contains(input.Email.ToLower())) &&
                        (string.IsNullOrEmpty(input.CPF) || x.CPF.ToLower().Contains(input.CPF.ToLower())) &&
                        (string.IsNullOrEmpty(input.Address) || x.Address!.ToLower().Contains(input.Address.ToLower())) &&
                        (!input.DateOfBirth.HasValue || (x.DateOfBirth.GetValueOrDefault().Day == input.DateOfBirth.Value.Day && x.DateOfBirth.GetValueOrDefault().Month == input.DateOfBirth.Value.Month && x.DateOfBirth.GetValueOrDefault().Year == input.DateOfBirth.Value.Year)) &&
                        (string.IsNullOrEmpty(input.Phone) || x.Phone!.ToLower().Contains(input.Phone.ToLower())) &&
                        (string.IsNullOrEmpty(input.Notes) || x.Notes!.ToLower().Contains(input.Notes.ToLower()))
                    ).OrderBy(x => x.FullName);

        (IEnumerable<Client> linq, int count) = await PagedQuery.Execute(query, pagination);
        var output = linq.Adapt<List<ClientOutput>>();

        return (output, count);
    }
}