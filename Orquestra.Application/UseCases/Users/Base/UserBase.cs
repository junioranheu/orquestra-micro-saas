using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using static Orquestra.Utils.Fixtures.CommonForBases;
using static Orquestra.Utils.Fixtures.Get;
using static Orquestra.Utils.Fixtures.RegexPatterns;

namespace Orquestra.Application.UseCases.Users.Base;

public partial class UserBase(IGetUser getUser)
{
    private readonly IGetUser _getUser = getUser;

    public async Task Validate(UserInput input, Guid userIdAuth, bool isCreate, bool hasChangedPassword)
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

            if (checkUserById is null || (checkUserById is not null && userIdAuth != checkUserById?.UserId))
            {
                throw new UnauthorizedAccessException("Apenas o proprietário da conta pode alterar suas informações.");
            }
        }
        #endregion

        #region name
        input.FullName = NormalizeToProperName(input.FullName ?? string.Empty);
        bool checkName = IsFullNameValid(input.FullName ?? string.Empty);

        if (!checkName)
        {
            throw new ArgumentException("O nome não é válido. Insira seu nome completo, por favor.");
        }
        #endregion

        #region password
        if (hasChangedPassword)
        {
            bool checkPassword = IsPasswordValid(input.Password ?? string.Empty);

            if (!checkPassword)
            {
                throw new ArgumentException("A senha é inválida. Insira uma senha com pelo menos 8 caracteres e um digito ou carácter especial, por favor.");
            }
        }
        #endregion
    }

    #region extras
    // Pelo menos 8 caracteres, 1 digito OU 1 caracter especial;
    private static bool IsPasswordValid(string password)
    {
        return RegexPassword().IsMatch(password);
    }
    #endregion
}