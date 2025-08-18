using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Infrastructure.Data;
using System.Text.RegularExpressions;

namespace Orquestra.Application.UseCases.Clients.Base;

public partial class ClientBase(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser)
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Validate(ClientInput input, Guid userId, bool isCreate)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId, needAdmin: true);

        bool checkEmail = IsEmailValid(input.Email);

        if (!checkEmail)
        {
            throw new Exception("O e-mail do cliente não é válido. Insira um e-mail válido, por favor.");
        }

        if (isCreate)
        {
            bool anyCPF = await _context.Clients.AsNoTracking().AnyAsync(x => x.CPF == input.CPF && x.CompanyId == input.CompanyId && x.ClientId == input.ClientId && x.Status == true);

            if (anyCPF)
            {
                throw new Exception("Este CPF já está registrado nesta empresa como cliente.");
            }

            bool anyEmail = await _context.Clients.AsNoTracking().AnyAsync(x => x.Email == input.Email && x.CompanyId == input.CompanyId && x.Email == input.Email);

            if (anyEmail)
            {
                throw new Exception("Este e-mail já está registrado nesta empresa como cliente.");
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