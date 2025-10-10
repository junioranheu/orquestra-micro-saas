using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Infrastructure.Data;
using System.Text.RegularExpressions;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Clients.Base;

public partial class ClientBase(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser)
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Validate(ClientInput input, Guid userIdAuth, bool isCreate)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: true);

        bool checkName = IsFullNameValid(input.FullName ?? string.Empty);

        if (!checkName)
        {
            throw new ArgumentException("O nome não é válido. Insira seu nome completo, por favor.");
        }

        input.Email = input.Email?.Trim().ToLowerInvariant() ?? string.Empty;

        // Segundo o Gongo, deveria ser possível cadastrar um cliente sem um e-mail;
        if (!string.IsNullOrEmpty(input.Email))
        {
            bool checkEmail = IsEmailValid(input.Email);

            if (!checkEmail)
            {
                throw new ArgumentException("O e-mail do cliente não é válido. Insira um e-mail válido, por favor.");
            }

            input.Email = GetNormalizedLowerStr(input.Email);
        }

        bool checkPhone = IsPhoneValid(input.Phone);

        if (!checkPhone)
        {
            throw new ArgumentException("O número de telefone não é válido. Insira um número válido, por favor.");
        }

        if (isCreate)
        {
            bool anyCPF = await _context.Clients.AsNoTracking().AnyAsync(x => x.CPF == input.CPF && x.CompanyId == input.CompanyId);

            if (anyCPF)
            {
                throw new InvalidOperationException($"O CPF {input.CPF} já está registrado nesta empresa como cliente.");
            }

            if (!string.IsNullOrEmpty(input.Email))
            {
                bool anyEmail = await _context.Clients.AsNoTracking().AnyAsync(x => x.Email.ToLower() == input.Email && x.CompanyId == input.CompanyId);

                if (anyEmail)
                {
                    throw new InvalidOperationException($"O e-mail {input.Email} já está registrado nesta empresa como cliente.");
                }
            }
        }
    }

    #region extras
    private static bool IsFullNameValid(string fullName)
    {
        return RegexName().IsMatch(fullName);
    }

    private static bool IsPhoneValid(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return true; // É possível cadastrar sem telefone;
        }

        return RegexPhone().IsMatch(phone);
    }

    // Regex;
    [GeneratedRegex(@"^(?i)[A-Za-zÀ-ÿ]{3,}(?:\s+(?:de|da|dos|das))?\s+[A-Za-zÀ-ÿ]{3,}$")]
    private static partial Regex RegexName();

    [GeneratedRegex(@"^\d{2} ?9?\d{8}$")]
    private static partial Regex RegexPhone();
    #endregion
}