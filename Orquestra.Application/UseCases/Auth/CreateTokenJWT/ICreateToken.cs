using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Auth.Shared;
using Orquestra.Application.UseCases.Users.Shared;

namespace Orquestra.Application.UseCases.Auth.CreateTokenJWT;

public interface ICreateToken
{
    Task<(UserOutput output, string token, CookieOptions cookieOptions)> Execute(AuthInput input);
}