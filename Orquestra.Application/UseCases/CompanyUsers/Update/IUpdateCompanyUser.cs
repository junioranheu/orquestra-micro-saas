using Orquestra.Application.UseCases.CompanyUsers.Shared;

namespace Orquestra.Application.UseCases.CompanyUsers.Update;

public interface IUpdateCompanyUser
{
    Task Execute(Guid userIdAuth, CompanyUserInput input);
}