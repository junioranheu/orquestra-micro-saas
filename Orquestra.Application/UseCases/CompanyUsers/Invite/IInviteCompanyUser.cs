using Orquestra.Application.UseCases.CompanyUsers.Shared;

namespace Orquestra.Application.UseCases.CompanyUsers.Invite;

public interface IInviteCompanyUser
{
    Task<CompanyUserOutput> Execute(Guid userIdAuth, string email);
}