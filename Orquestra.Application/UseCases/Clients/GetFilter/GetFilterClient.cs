using Orquestra.Application.UseCases.Clients.GetAllByCompanyId;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Services.GenericCache;
using static Orquestra.Application.UseCases.Shared.DropDownOption;

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
        ClientFilterOutputStringify distinctValues = new()
        {
            FullNames = CleanDistinctOrdered(cachedLinq, x => x.FullName),
            Emails = CleanDistinctOrdered(cachedLinq, x => x.Email)
        };

        ClientFilterOutput output = new()
        {
            FullNames = MapToDropdown(distinctValues?.FullNames),
            Emails = MapToDropdown(distinctValues?.Emails)
        };

        return output;
    }
    #endregion
}