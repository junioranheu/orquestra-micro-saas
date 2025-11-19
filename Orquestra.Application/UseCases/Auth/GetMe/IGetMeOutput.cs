using Orquestra.Application.UseCases.Auth.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Auth.GetMe;

public interface IGetMeOutput
{
    Task<MeOutput> Execute(bool checkExpirationDate, string? token, Guid userIdAuth, bool isAuth, string nameAuth, string emailAuth, UserRoleEnum[] userRoles, string[] userRolesStr);
}