using Orquestra.Application.UseCases.Auth.GetRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.Shared;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Application.UseCases.CompanyUsers.GetModule;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Auth.Token;
using System.IdentityModel.Tokens.Jwt;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Auth.GetMe;

public sealed class GetMeOutput(
        IJwtTokenGenerator jwtTokenGenerator,
        IGetRefreshToken getRefreshToken,
        IGetCurrentMainCompanyUser getCurrentMainCompanyUser,
        IGetModuleCompanyUser getModuleCompanyUser
    ) : IGetMeOutput
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IGetRefreshToken _getRefreshToken = getRefreshToken;
    private readonly IGetCurrentMainCompanyUser _getCurrentMainCompanyUser = getCurrentMainCompanyUser;
    private readonly IGetModuleCompanyUser _getModuleCompanyUser = getModuleCompanyUser;

    public async Task<MeOutput> Execute(bool checkExpirationDate, string? token, Guid userIdAuth, bool isAuth, string nameAuth, string emailAuth, UserRoleEnum[] userRoles, string[] userRolesStr)
    {
        // Current main company;
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await _getCurrentMainCompanyUser.Execute(userIdAuth);

        DateTime tokenExpirationDate = DateTime.MinValue;
        DateTime refreshTokenExpirationDate = DateTime.MinValue;

        if (checkExpirationDate)
        {
            // Token;
            JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            (_, _, tokenExpirationDate) = _jwtTokenGenerator.IsTokenExpiringSoonOrHasAlreadyExpired(jwtToken);

            // Refresh token;
            RefreshToken? refreshToken = await _getRefreshToken.GetLatestNotRevokedToken(userIdAuth);
            refreshTokenExpirationDate = refreshToken?.ExpiredDate ?? DateTime.MinValue;
        }

        // Output;
        MeOutput output = new()
        {
            IsAuth = isAuth,
            UserId = userIdAuth,
            UserName = nameAuth,
            Email = emailAuth,
            Roles = userRoles,
            RolesStr = userRolesStr,
            CurrentMainCompany = currentMainCompany,
            TokenExpirationDate = tokenExpirationDate,
            RefreshTokenExpirationDate = refreshTokenExpirationDate,
            IsUserAdmOfCurrentMainCompany = isUserAdm
        };

        // Módulos;
        if (currentMainCompany is not null && output.CurrentMainCompany is not null)
        {
            if (isUserAdm)
            {
                var moduleEnum = GetEnumListWithDescriptions<ModuleEnum>();

                output.CurrentMainCompany.UserModules = [.. moduleEnum.Select(x => x.Value)];
                output.CurrentMainCompany.UserModulesStr = [.. moduleEnum.Select(x => x.Description)];
            }
            else
            {
                (ModuleEnum[] modules, List<string> modulesStr) = await _getModuleCompanyUser.Execute(userIdAuth, companyId: currentMainCompany.CompanyId);

                output.CurrentMainCompany.UserModules = modules;
                output.CurrentMainCompany.UserModulesStr = modulesStr;
            }
        }

        return output;
    }
}