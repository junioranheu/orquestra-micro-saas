using Microsoft.AspNetCore.Mvc;

namespace Orquestra.Infrastructure.Services.GenericCache;

public interface IGenericCacheService
{
    void Clear(string[] keys);
    Task<T?> Get<T>(string key);
    Task<T?> GetObjectFromActionResult<T>(Func<Task<ActionResult>> fetchFunction, string key) where T : class;
    Task<T?> GetOrAdd<T>(Func<Task<T>> fetchFunction, string key, TimeSpan expiration);
    Task<T?> GetOrAddSimple<T>(Func<Task<T>> fetchFunction, string key, TimeSpan expiration);
    Task<T?> GetOrAddWithQueue<T>(Func<Task<T>> fetchFunction, string key, TimeSpan expiration);
}