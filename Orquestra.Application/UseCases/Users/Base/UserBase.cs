using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using System.Text.RegularExpressions;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Users.Base;

public partial class UserBase(IGetUser getUser)
{
    private readonly IGetUser _getUser = getUser;

    public async Task Validate(UserInput input, Guid userIdAuth, bool isCreate)
    {                               
        #region email
        input.Email = input.Email?.Trim().ToLowerInvariant();

        bool checkEmail = IsEmailValid(input.Email);

        if (!checkEmail)
        {
            throw new ArgumentException("O e-mail não é válido. Insira um e-mail válido, por favor.");
        }

        if (isCreate)
        {
            (UserOutput? checkUserByEmail, string _) = await _getUser.Execute(new UserInput() { Email = input.Email });

            if (checkUserByEmail is not null)
            {
                throw new InvalidOperationException($"O e-mail {input.Email} já está cadastrado no sistema.");
            }
        }

        if (!isCreate)
        {
            (UserOutput? checkUserById, string _) = await _getUser.Execute(new UserInput() { UserId = userIdAuth });

            if (checkUserById is not null && userIdAuth != checkUserById?.UserId)
            {
                throw new UnauthorizedAccessException("Apenas o proprietário da conta pode alterar suas informações.");
            }
        }
        #endregion

        #region name
        bool checkName = IsFullNameValid(input.FullName ?? string.Empty);

        if (!checkName)
        {
            throw new ArgumentException("O nome não é válido. Insira seu nome completo, por favor.");
        }

        input.FullName = NormalizeToProperName(input.FullName ?? string.Empty);
        #endregion

        #region password
        bool checkPassword = IsPasswordValid(input.Password ?? string.Empty);

        if (!checkPassword)
        {
            throw new ArgumentException("A senha não é válida. Insira uma senha com pelo menos 8 carácteres e um digito ou carácter especial, por favor.");
        }
        #endregion
    }

    #region extras
    // A valid full name must have at least two parts, each with at least 3 letters (case-insensitive);
    private static bool IsFullNameValid(string fullName)
    {
        return RegexName().IsMatch(fullName);
    }

    // Minimum requirements: 8+ characters, at least 1 digit OR 1 special character;
    private static bool IsPasswordValid(string password)
    {
        return RegexPassword().IsMatch(password);
    }

    // Regex;
    [GeneratedRegex(@"^(?i)[A-Za-zÀ-ÿ]{3,}(?:\s+(?:de|da|dos|das))?\s+[A-Za-zÀ-ÿ]{3,}$")]
    private static partial Regex RegexName();

    [GeneratedRegex(@"^(?=.*[\d@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
    private static partial Regex RegexPassword();
    #endregion
}