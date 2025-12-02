using Orquestra.Application.UseCases.Clients.GetAllByCompanyId;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Services.GenericCache;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Clients.GetFilter;

public sealed class GetFilterClient(IGenericCacheService cache, IGetAllClientByCompanyId getAllClientByCompanyId) : IGetFilterClient
{
    private readonly IGenericCacheService _cache = cache;
    private readonly IGetAllClientByCompanyId _getAllClientByCompanyId = getAllClientByCompanyId;

    public async Task<ClientFilterOutput?> Execute(Guid userIdAuth, Guid companyId)
    {
        string key = $"{SystemConsts.Cache.CacheKey_FiltersClient}{companyId}";
        List<ClientOutput> cachedLinq = await _cache.GetOrAddWithQueue(fetchFunction: () => _getAllClientByCompanyId.Execute(userIdAuth, companyId), key, expiration: TimeSpan.FromHours(24)) ?? [];

        ClientFilterOutput normalizedData = ConvertToDropdownOptions(cachedLinq);

        return normalizedData;
    }

    #region extras
    private static ClientFilterOutput ConvertToDropdownOptions(List<ClientOutput> cachedLinq)
    {
        var finalLinq = cachedLinq.Select(x => new
        {
            x.FullName,
            x.Email
        }).ToList();

        ClientFilterOutputStringify distinctValues = new()
        {
            FullNames = CleanDistinctOrdered(finalLinq, x => x.FullName),
            Emails = CleanDistinctOrdered(finalLinq, x => x.Email)
        };

        ClientFilterOutput output = new()
        {
            FullNames = distinctValues?.FullNames.Select((x, index) => new DropdownOptionOutput<int> { Label = x ?? string.Empty, Value = index + 1 }).ToList(),
            Emails = distinctValues?.Emails.Select((x, index) => new DropdownOptionOutput<int> { Label = x ?? string.Empty, Value = index + 1 }).ToList()
        };

        return output;
    }
    #endregion
}