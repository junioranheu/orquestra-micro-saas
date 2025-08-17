using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using System.Text.RegularExpressions;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Users.Base;

public partial class UserBase(IGetUser getUser)
{
    private readonly IGetUser _getUser = getUser;

    public async Task Validate(UserInput input, Guid userId, bool isCreate)
    {
        #region email
        bool checkEmail = IsEmailValid(input.Email ?? string.Empty);

        if (!checkEmail)
        {
            throw new Exception("O e-mail não é válido. Insira um e-mail válido, por favor.");
        }

        if (isCreate)
        {
            (User? checkUserByEmail, string _) = await _getUser.Execute(new UserInput() { Email = input.Email });

            if (checkUserByEmail is not null)
            {
                throw new Exception($"O e-mail {input.Email} já está cadastrado no sistema.");
            }
        }

        if (!isCreate)
        {
            (User? checkUserById, string _) = await _getUser.Execute(new UserInput() { UserId = userId });

            if (checkUserById is not null && userId != checkUserById?.UserId)
            {
                throw new Exception("Apenas o dono da conta pode alterar suas informações.");
            }
        }
        #endregion

        #region name
        bool checkName = IsFullNameValid(input.FullName ?? string.Empty);

        if (!checkName)
        {
            throw new Exception("O nome não é válido. Insira seu nome completo, por favor.");
        }

        input.FullName = NormalizeToProperName(input.FullName ?? string.Empty);
        #endregion

        #region password
        bool checkPassword = IsPasswordValid(input.Password ?? string.Empty);

        if (!checkPassword)
        {
            throw new Exception("A senha não é válida. Insira uma senha com pelo menos 8 carácteres e um digito ou carácter especial, por favor.");
        }
        #endregion
    }

    #region extras
    // A valid full name must have at least two parts, each with at least 3 letters (case-insensitive);
    private static bool IsFullNameValid(string fullName)
    {
        return RegexName().IsMatch(fullName);
    }

    // Simple email validation regex;
    private static bool IsEmailValid(string email)
    {
        return RegexEmail().IsMatch(email);
    }

    // Minimum requirements: 8+ characters, at least 1 digit OR 1 special character;
    private static bool IsPasswordValid(string password)
    {
        return RegexPassword().IsMatch(password);
    }

    // Regex;
    [GeneratedRegex(@"^(?=.*[A-Za-z]{3,})[A-Za-z ]{3,}$")]
    private static partial Regex RegexName();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex RegexEmail();

    [GeneratedRegex(@"^(?=.*[\d@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
    private static partial Regex RegexPassword();
    #endregion
}