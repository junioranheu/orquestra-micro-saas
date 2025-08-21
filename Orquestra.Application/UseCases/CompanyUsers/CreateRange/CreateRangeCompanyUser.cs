using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.Base;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyUsers.CreateRange;

public sealed class CreateRangeCompanyUser(
        Context context,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        IEmailService emailService
    ) : CompanyUserBase(context, checkIfUserIsLinkedCompanyUser), ICreateRangeCompanyUser
{
    private readonly Context _context = context;
    private readonly IEmailService _emailService = emailService;

    public async Task<List<CompanyUserOutput>> Execute(Guid userIdAuth, List<CompanyUserInput> input)
    {
        // Validate;
        foreach (var item in input)
        {
            await Validate(input: item, userIdAuth, isCreate: true);
        }

        var companyUsers = input.Adapt<List<CompanyUser>>();

        // Normalize;
        foreach (var item in companyUsers)
        {
            item.VerifyToken = GetRandomString(32, false);
        }

        NormalizeOwnerProps(companyUsers);

        // Save;
        await _context.AddRangeAsync(companyUsers);
        await _context.SaveChangesAsync();

        // Email;
        Guid? companyId = input.FirstOrDefault()?.CompanyId;

        if (companyId is not null && companyId != Guid.Empty)
        {
            Company? company = await _context.Companies.
                               AsNoTracking().
                               Where(x => x.CompanyId == companyId && x.Status == true).
                               FirstOrDefaultAsync();

            foreach (var item in companyUsers)
            {
                await SendEmail(item, company);
            }
        }

        // Output;
        var output = companyUsers.Adapt<List<CompanyUserOutput>>();

        return output;
    }

    #region extras
    private static void NormalizeOwnerProps(List<CompanyUser> companyUsers)
    {
        if (companyUsers.Count != 1)
        {
            return;
        }

        CompanyUser? first = companyUsers.FirstOrDefault();

        if (first?.CompanyUserRole == CompanyUserRoleEnum.Owner)
        {
            first.IsAccountVerified = true;
            first.IsCurrentMainCompanyUser = true;
        }
    }

    private async Task SendEmail(CompanyUser companyUser, Company? company)
    {
        User? user = await _context.Users.
                     AsNoTracking().
                     Where(x => x.UserId == companyUser.UserId && x.Status == true).
                     FirstOrDefaultAsync();

        if (user is null || company is null)
        {
            return;
        }

        (string urlBack, string _) = GetUrls();
        string verifyUrl = $"{urlBack}/CompanyUser/verify/{companyUser.VerifyToken}";

        Dictionary<string, string> values = new()
        {
            { "[NameApp]", SystemConsts.NameApp },
            { "[CompanyName]", company.Name },
            { "[UserName]", GetFirstWord(user.FullName) },
            { "[VerifyUrl]", verifyUrl }
        };

        string bodyHtml = _emailService.RenderTemplate("EmailVerifyCompanyUser.html", values);
        await _emailService.SendEmail(to: user.Email, subject: $"Bem-vindo ao {SystemConsts.NameApp} — Verifique sua conta!", body: bodyHtml);
    }
    #endregion
}