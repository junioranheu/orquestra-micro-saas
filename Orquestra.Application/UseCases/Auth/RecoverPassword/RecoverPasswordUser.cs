using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Application.UseCases.Verifications.Get;
using Orquestra.Application.UseCases.Verifications.Update;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Messaging.Publishers;
using Orquestra.Infrastructure.Services.Email.Models;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;
using static Orquestra.Utils.Fixtures.Encrypt;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Auth.RecoverPassword;

public sealed class RecoverPasswordUser(
        Context context,
        IEnvService env,
        IGetVerification getVerification,
        IUpdateVerification updateVerification,
        IGetUser getUser,
        ICreateVerification createVerification,
        IGenericPublisher publisher
    ) : IRecoverPasswordUser
{
    private readonly Context _context = context;
    private readonly IEnvService _env = env;
    private readonly IGetVerification _getVerification = getVerification;
    private readonly IUpdateVerification _updateVerification = updateVerification;
    private readonly IGetUser _getUser = getUser;
    private readonly ICreateVerification _createVerification = createVerification;
    private readonly IGenericPublisher _publisher = publisher;

    private const string WARNING_NO_RECOVER_ANSWER = "Aparentemente sua conta não tem nenhuma resposta de recuperação de conta. Caso necessário, contate o suporte.";

    public async Task SendEmail(string email)
    {
        UserOutput user = await _getUser.Execute(userId: null, email: email, throwIfStatusFalse: true);

        if (user.RecoverPasswordQuestion == 0 || string.IsNullOrEmpty(user.RecoverPasswordAnswer))
        {
            throw new InvalidOperationException(WARNING_NO_RECOVER_ANSWER);
        }

        Verification verification = await SaveVerification(user);
        await SendEmail(verification, user);
    }

    public async Task Verify(string token)
    {
        Verification verification = await _getVerification.Execute(token, verificationType: VerificationTypeEnum.PasswordReset);

        var result = await _context.Users.
                     AsNoTracking().
                     Where(x => x.UserId == verification.EntityId).
                     FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"O token {token} não pertence a nenhum usuário.");

        if (result.RecoverPasswordQuestion == 0 || string.IsNullOrEmpty(result.RecoverPasswordAnswer))
        {
            throw new InvalidOperationException(WARNING_NO_RECOVER_ANSWER);
        }

        // Alterar senha;
        result.Password = result.RecoverPasswordAnswer;

        _context.ChangeTracker.Clear();
        _context.Update(result);
        await _context.SaveChangesAsync();

        // Atualizar status da verificação;
        await _updateVerification.Execute(verificationId: verification.VerificationId);
    }

    #region extras
    private async Task<Verification> SaveVerification(UserOutput user)
    {
        DateTime expirationDate = GetDate().AddMinutes(30);
        Verification verification = await _createVerification.Execute<Company>(entityId: user.UserId, verificationType: VerificationTypeEnum.PasswordReset, expirationDate: expirationDate);

        return verification;
    }

    private async Task SendEmail(Verification verification, UserOutput user)
    {
        EnvOutput env = _env.GetUrls();
        string verifyUrl = $"{env.UrlBackend}/Auth/Verify/RecoverPassword/{verification.Token}";

        Dictionary<string, string> values = new()
        {
            { "[NameApp]", SystemConsts.App.NameApp },
            { "[UserName]", GetFirstWord(user.FullName) },
            { "[VerifyUrl]", verifyUrl }
        };

        string bodyHtml = RenderTemplate(SystemConsts.Templates.EmailResetPassword, values);

        EmailInput input = new()
        {
            To = user.Email,
            Subject = $"Redefina sua senha no {SystemConsts.App.NameApp}",
            Body = bodyHtml
        };

        await _publisher.Publish(SystemConsts.RabbitMQ.EmailQueue, input);
    }
    #endregion
}