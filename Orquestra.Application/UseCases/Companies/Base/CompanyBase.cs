using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Invite;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Create;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Application.UseCases.Verifications.Create;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Messaging.Publishers;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Email.Models;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;
using static Orquestra.Utils.Fixtures.Get;
using static Orquestra.Utils.Fixtures.RegexPatterns;

namespace Orquestra.Application.UseCases.Companies.Base;

public record CompanyBaseDependencies(
    Context Context,
    IEnvService Env,
    ICreateVerification CreateVerification,
    IInviteCompanyUser InviteCompanyUser,
    IUpdateCurrentMainCompanyUser UpdateCurrentMainCompanyUser,
    IGetUser GetUser,
    IEmailService EmailService,
    ICheckIfUserIsLinkedCompanyUser CheckIfUserIsLinkedCompanyUser,
    ICreateCompanyInvoice CreateCompanyInvoice,
    ICreateIntegrationWhatsApp CreateIntegrationWhatsApp,
    IGenericPublisher Publisher
);

public partial class CompanyBase(CompanyBaseDependencies deps)
{
    private readonly Context _context = deps.Context;
    private readonly IEnvService _env = deps.Env;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = deps.CheckIfUserIsLinkedCompanyUser;
    private readonly ICreateVerification _createVerification = deps.CreateVerification;
    private readonly IEmailService _emailService = deps.EmailService;
    private readonly IGenericPublisher _publisher = deps.Publisher;

    public async Task Validate(CompanyInput input, Guid userIdAuth, bool isCreate, bool mustValidateIfNameAlreadyExist, bool mustValidateIfEmailAlreadyExist)
    {
        string warnAlreadyExist = $"Caso você não concorde que já exista uma empresa com esta informação, entre em contato pelo e-mail {SystemConsts.App.Email}.";

        #region status
        if (!isCreate)
        {
            if (!input.Status)
            {
                throw new InvalidOperationException(SystemConsts.Warnings.NeedToVerifyCompany);
            }
        }
        #endregion

        #region basic
        if (!isCreate)
        {
            await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId: userIdAuth, needCompanyAdmin: true);
        }

        bool checkName = IsNameValid(input.Name);

        if (!checkName)
        {
            throw new ArgumentException("O nome da empresa não é válido.");
        }


        bool checkEmail = IsEmailValid(input.Email);

        if (!checkEmail)
        {
            throw new ArgumentException("O e-mail da empresa não é válido. Insira um e-mail válido, por favor.");
        }

        if (mustValidateIfNameAlreadyExist)
        {
            bool checkNameAlreadyExist = await _context.Companies.AsNoTracking().AnyAsync(x => x.Name.ToLower() == input.Name.ToLower());

            if (checkNameAlreadyExist)
            {
                throw new InvalidOperationException($"Já existe uma empresa registrada com esse nome. {warnAlreadyExist}");
            }
        }

        if (mustValidateIfEmailAlreadyExist)
        {
            bool checkEmailAlreadyExist = await _context.Companies.AsNoTracking().AnyAsync(x => x.Email.ToLower() == input.Email.ToLower());

            if (checkEmailAlreadyExist)
            {
                throw new InvalidOperationException($"Já existe uma empresa registrada com esse e-mail. {warnAlreadyExist}");
            }
        }

        bool checkPhone = IsPhoneValid(input.Phone);

        if (!checkPhone)
        {
            throw new ArgumentException("O número de telefone não é válido. Insira um número válido, por favor.");
        }

        bool checkType = IsTypeValid(input.CompanyType);

        if (!checkType)
        {
            throw new ArgumentException("O tipo da empresa não é válido. Insira um tipo válido, por favor.");
        }
        #endregion

        #region location
        bool checkZipCode = IsZipCodeValid(input.ZipCode);

        if (!checkZipCode)
        {
            throw new ArgumentException("O CEP não é válido. Insira um CEP válido, por favor.");
        }

        bool checkCountry = IsCountryValid(input.Country);

        if (!checkCountry)
        {
            throw new ArgumentException("O país não é válido. Insira um país válido, por favor.");
        }
        #endregion

        #region subscription
        if (input.PlanType == 0)
        {
            throw new ArgumentException("O tipo do plano não é válido. Insira um plano válido, por favor.");
        }
        #endregion

        #region customization
        if (input.LogoFormFile is not null)
        {
            ValidateMaxSizeFile(file: input.LogoFormFile, maxMegabytes: 3);
        }
        #endregion
    }

    public async Task SendEmail(Company company, UserOutput user)
    {
        Verification verification = await _createVerification.Execute<Company>(entityId: company.CompanyId, verificationType: VerificationTypeEnum.Company);

        EnvOutput env = _env.GetUrls();
        string verifyUrl = $"{env.UrlBackend}/Company/Verify/{verification.Token}";

        Dictionary<string, string> values = new()
        {
            { "[NameApp]", SystemConsts.App.NameApp },
            { "[CompanyName]", company.Name },
            { "[UserName]", GetFirstWord(user.FullName) },
            { "[VerifyUrl]", verifyUrl }
        };

        string bodyHtml = _emailService.RenderTemplate(SystemConsts.Templates.EmailVerifyCompany, values); // TO DO: REMOVER ESSA DEPENDENCIA

        EmailInput input = new()
        {
            To = company.Email,
            Subject = $"Bem-vindo ao {SystemConsts.App.NameApp} — Verifique sua empresa!",
            Body = bodyHtml,
            Cc = [user.Email]
        };

        await _publisher.PublishAsync(SystemConsts.RabbitMQ.EmailQueue, input);
    }

    #region extras
    // Basic;
    private static bool IsNameValid(string name)
    {
        return !string.IsNullOrWhiteSpace(name);
    }

    private static bool IsEmailValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return RegexEmail().IsMatch(email);
    }

    private static bool IsPhoneValid(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return false;
        }

        return RegexPhone().IsMatch(phone);
    }

    private static bool IsTypeValid(CompanyTypeEnum type)
    {
        return Enum.IsDefined(type);
    }

    // Location;
    private static bool IsZipCodeValid(string? zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return true;
        }

        return RegexZipCode().IsMatch(zipCode);
    }

    private static bool IsCountryValid(string? country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return true;
        }

        List<string> countries = GetCountries();
        string normalizedInput = NormalizeTextRemoveAccentsAndLower(country);
        bool isValid = countries.Any(c => NormalizeTextRemoveAccentsAndLower(c) == normalizedInput);

        return isValid;
    }
    #endregion
}