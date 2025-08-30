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

        bool checkEmail = IsEmailValid(input.Email);

        if (!checkEmail)
        {
            throw new ArgumentException("O e-mail do cliente não é válido. Insira um e-mail válido, por favor.");
        }

        input.Email = GetNormalizedLowerStr(input.Email);

        if (isCreate)
        {
            bool anyCPF = await _context.Clients.AsNoTracking().AnyAsync(x => 
                             x.CPF == input.CPF && 
                             x.CompanyId == input.CompanyId
                          );

            if (anyCPF)
            {
                throw new InvalidOperationException($"O CPF {input.CPF} já está registrado nesta empresa como cliente.");
            }

            bool anyEmail = await _context.Clients.AsNoTracking().AnyAsync(x =>
                                x.Email.ToLower() == input.Email &&
                                x.CompanyId == input.CompanyId
                            );

            if (anyEmail)
            {
                throw new InvalidOperationException($"O e-mail {input.Email} já está registrado nesta empresa como cliente.");
            }
        }
    }

    private static bool IsEmailValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return RegexEmail().IsMatch(email);
    }

    // Regex;
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex RegexEmail();
}