using AutoMapper;
using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.Shared;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Auth.Token;
using static Orquestra.Utils.Fixtures.Encrypt;

namespace Orquestra.Application.UseCases.Auth.CreateTokenJWT;

public sealed class CreateToken(
    IMapper map,
    IJwtTokenGenerator jwtTokenGenerator,
    ICreateRefreshToken createRefreshToken,
    IGetUser getUser) : ICreateToken
{
    private readonly IMapper _map = map;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly ICreateRefreshToken _createRefreshToken = createRefreshToken;
    private readonly IGetUser _getUser = getUser;

    public async Task<UserOutput> Execute(AuthInput input)
    {
        (User? user, string passwordEncrypted) = await _getUser.Execute(new UserInput() { Email = input.Email });
        var output = _map.Map<UserOutput>(user) ?? throw new Exception("Usuário não encontrado.");

        if (!CheckPassword(password: input.Password, encryptedPassword: passwordEncrypted))
        {
            throw new Exception("E-mail ou senha incorretos.");
        }

        if (!output.Status)
        {
            throw new Exception("Usuário desativado.");
        }

        (string token, RefreshToken refreshToken) = _jwtTokenGenerator.GenerateToken(userId: output.UserId, name: output.FullName, email: output.Email, role: output.Role);

        // Atualizar token no output;
        output.Token = token;

        // Revogar todos os refresh tokens antigos, caso existam;
        await _createRefreshToken.Update(userId: output.UserId, mustCheckForValidRefreshTokens: false);

        // Salvar o refresh token no banco;
        await _createRefreshToken.Save(refreshToken);

        return output;
    }
}