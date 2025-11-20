using Orquestra.Application.UseCases.Auth.GetRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.Shared;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Auth.Token;
using System.IdentityModel.Tokens.Jwt;

namespace Orquestra.Application.UseCases.Auth.GetMe;

public sealed class GetMeOutput(
        IJwtTokenGenerator jwtTokenGenerator,
        IGetRefreshToken getRefreshToken,
        IGetCurrentMainCompanyUser getCurrentMainCompanyUser
    ) : IGetMeOutput
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IGetRefreshToken _getRefreshToken = getRefreshToken;
    private readonly IGetCurrentMainCompanyUser _getCurrentMainCompanyUser = getCurrentMainCompanyUser;

    public async Task<MeOutput> Execute(bool checkExpirationDate, string? token, Guid userIdAuth, bool isAuth, string nameAuth, string emailAuth, UserRoleEnum[] userRoles, string[] userRolesStr)
    {
        // Current main company;
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await _getCurrentMainCompanyUser.GetCurrentMainCompany(userIdAuth);

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

        return output;
    }
}