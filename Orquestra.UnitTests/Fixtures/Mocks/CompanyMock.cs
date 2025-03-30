using AutoMapper;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Fixtures.Mocks;

public static class CompanyMock
{
    public static CompanyInput Create()
    {
        var input = new CompanyInput
        {
            CompanyId = Guid.NewGuid(),
            Name = GetRandomString(GetRandomNumber(5, 15), false),
            Email = $"{GetRandomString(GetRandomNumber(5, 15), false)}@gmail.com",
            Phone = GetRandomString(GetRandomNumber(5, 15), false),
            Type = CompanyTypeEnum.ClinicaOdontologia
        };

        return input;
    }

    public static List<CompanyInput> CreateList(int j)
    {
        List<CompanyInput> list = [];

        for (int i = 0; i < j; i++)
        {
            list.Add(Create());
        }

        return list;
    }

    public static Company Create(IMapper _map)
    {
        Company output = _map.Map<Company>(Create());
        return output;
    }

    public static List<Company> CreateList(IMapper _map, int j)
    {
        List<Company> output = _map.Map<List<Company>>(CreateList(j));
        return output;
    }
}