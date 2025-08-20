using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using System.Text.RegularExpressions;

namespace Orquestra.Application.UseCases.Companies.Base;

public partial class CompanyBase(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser)
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Validate(CompanyInput input, Guid userId, bool isCreate)
    {
        string warn = $"Caso você não concorde que já exista uma empresa com esta informação, entre em contato pelo e-mail {SystemConsts.Email}.";

        #region basic
        if (!isCreate)
        {
            await _checkIfUserIsLinkedCompanyUser.Execute(companyId: input.CompanyId, userId, needCompanyAdmin: true);
        }

        bool checkName = IsNameValid(input.Name);

        if (!checkName)
        {
            throw new Exception("O nome da empresa não é válido.");
        }

        bool checkNameAlreadyExist = await _context.Companies.AsNoTracking().AnyAsync(x => x.Name == input.Name && x.Status == true);

        if (checkNameAlreadyExist)
        {
            throw new Exception($"Já existe uma empresa registrada com esse nome. {warn}");
        }

        bool checkEmailAlreadyExist = await _context.Companies.AsNoTracking().AnyAsync(x => x.Email == input.Email && x.Status == true);

        if (checkEmailAlreadyExist)
        {
            throw new Exception($"Já existe uma empresa registrada com esse e-mail. {warn}");
        }

        bool checkEmail = IsEmailValid(input.Email);

        if (!checkEmail)
        {
            throw new Exception("O e-mail da empresa não é válido. Insira um e-mail válido, por favor.");
        }

        bool checkPhone = IsPhoneValid(input.Phone);

        if (!checkPhone)
        {
            throw new Exception("O número de telefone não é válido. Insira um número válido, por favor.");
        }

        bool checkType = IsTypeValid(input.Type);

        if (!checkType)
        {
            throw new Exception("O tipo da empresa não é válido. Insira um tipo válido, por favor.");
        }
        #endregion

        #region location
        bool checkAddress = IsStreetAddressValid(input.Address);

        if (!checkAddress)
        {
            throw new Exception("O endereço não é válido. Insira um endereço válido, por favor.");
        }

        bool checkCity = IsCityValid(input.City);

        if (!checkCity)
        {
            throw new Exception("A cidade não é válida. Insira uma cidade válida, por favor.");
        }

        bool checkState = IsStateValid(input.State);

        if (!checkState)
        {
            throw new Exception("O estado não é válido. Insira um estado válido, por favor.");
        }

        bool checkZipCode = IsZipCodeValid(input.ZipCode);

        if (!checkZipCode)
        {
            throw new Exception("O CEP não é válido. Insira um CEP válido, por favor.");
        }

        bool checkCountry = IsCountryValid(input.Country);

        if (!checkCountry)
        {
            throw new Exception("O país não é válido. Insira um país válido, por favor.");
        }
        #endregion

        #region customization
        bool checkLogoUrl = IsLogoUrlValid(input.LogoUrl);

        if (!checkLogoUrl)
        {
            throw new Exception("A logo não é válida. Insira uma logo válida, por favor.");
        }
        #endregion

        #region subscription
        bool checkPlanType = IsPlanTypeValid(input.PlanType);

        if (!checkPlanType)
        {
            throw new Exception("O tipo de plano não é válido. Insira um plano válido, por favor.");
        }
        #endregion

        #region status
        if (!isCreate)
        {
            if (!input.IsAccountVerified)
            {
                throw new Exception(SystemConsts.Warn_NeedToVerifyCompany);
            }
        }
        #endregion
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
    private static bool IsStreetAddressValid(string streetAddress)
    {
        return !string.IsNullOrWhiteSpace(streetAddress);
    }

    private static bool IsCityValid(string city)
    {
        return !string.IsNullOrWhiteSpace(city);
    }

    private static bool IsStateValid(string state)
    {
        return !string.IsNullOrWhiteSpace(state);
    }

    private static bool IsZipCodeValid(string? zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return true;
        }

        return RegexZipCode().IsMatch(zipCode);
    }

    private static bool IsCountryValid(string country)
    {
        return !string.IsNullOrWhiteSpace(country);
    }

    // Customization;
    private static bool IsLogoUrlValid(string? logoUrl)
    {
        if (string.IsNullOrWhiteSpace(logoUrl))
        {
            return true;
        }

        return RegexLogoUrl().IsMatch(logoUrl);
    }

    // Subscription;
    private static bool IsPlanTypeValid(PlanTypeEnum planType)
    {
        return Enum.IsDefined(planType);
    }

    // Regex;
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex RegexEmail();

    [GeneratedRegex(@"^\d{2} ?9?\d{8}$")]
    private static partial Regex RegexPhone();

    [GeneratedRegex(@"^https?:\/\/[^\s]+$")]
    private static partial Regex RegexLogoUrl();

    [GeneratedRegex(@"^\d{8}$")]
    private static partial Regex RegexZipCode();
    #endregion
}