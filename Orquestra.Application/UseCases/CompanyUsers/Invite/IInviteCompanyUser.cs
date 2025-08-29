namespace Orquestra.Application.UseCases.CompanyUsers.Invite;

public interface IInviteCompanyUser
{
    Task Execute(Guid userIdAuth, Guid companyId, string email, bool isFirstAdministrator);
}