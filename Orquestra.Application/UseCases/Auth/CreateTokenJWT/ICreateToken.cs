using Orquestra.Application.UseCases.Auth.Shared;

namespace Orquestra.Application.UseCases.Auth.CreateTokenJWT;

public interface ICreateToken
{
    Task<string> Execute(AuthInput input);
}