using Orquestra.Application.UseCases.Users.GetByEmail;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using System.Text.RegularExpressions;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Users.Base;

public partial class UserBase(IGetUserByEmail getUserByEmail)
{
    private readonly IGetUserByEmail _getUserByEmail = getUserByEmail;

    public async Task Validate(UserInput input, Guid userId, bool isCreate)
    {
        #region email
        bool checkEmail = IsEmailValid(input.Email);

        if (!checkEmail)
        {
            throw new Exception("O e-mail não é válido. Insira um e-mail válido, por favor.");
        }

        (User? checkUserByEmail, string _) = await _getUserByEmail.Execute(input.Email);      

        bool isEditAndSameEmail = !isCreate && input.Email == checkUserByEmail?.Email;

        if (checkUserByEmail is not null && !isEditAndSameEmail)
        {
            throw new Exception("Já existe um usuário com esse e-mail");
        }

        if (!isCreate)
        {
            if (checkUserByEmail is null)
            {
                throw new Exception("O e-mail não existeaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            }
        }

        if (checkUserByEmail is not null && !isCreate && userId != checkUserByEmail?.UserId)
        {
            throw new Exception("Apenas o dono da conta pode alterar suas informações");
        }
        #endregion

        #region name
        bool checkName = IsFullNameValid(input.FullName);

        if (!checkName)
        {
            throw new Exception("O nome não é válido. Insira seu nome completo, por favor.");
        }

        input.FullName = NormalizeToProperName(input.FullName);
        #endregion

        #region password
        bool checkPassword = IsPasswordValid(input.Password);

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

    [GeneratedRegex(@"^(?=.*[A-Za-z]{3,})[A-Za-z ]{3,}$")]
    private static partial Regex RegexName();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex RegexEmail();

    [GeneratedRegex(@"^(?=.*[\d@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
    private static partial Regex RegexPassword();
    #endregion
}