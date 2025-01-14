using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using System.Text.RegularExpressions;

namespace Orquestra.Application.UseCases.Companies.Base;

public partial class CompanyBase(Context context)
{
    private readonly Context _context = context;

    public async Task Validate(CompanyInput input)
    {
        #region name
        bool checkName = await _context.Companies.AsNoTracking().AnyAsync(x => x.Name == input.Name);

        if (checkName)
        {
            throw new Exception("Já existe uma empresa registrada com esse nome");
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
        return Enum.IsDefined(typeof(CompanyTypeEnum), type);
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

    private static bool IsZipCodeValid(string zipCode)
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
    private static bool IsLogoUrlValid(string logoUrl)
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
        return Enum.IsDefined(typeof(PlanTypeEnum), planType);
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