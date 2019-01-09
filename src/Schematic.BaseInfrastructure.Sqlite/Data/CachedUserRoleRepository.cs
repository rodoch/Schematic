using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Schematic.Core;
using Schematic.Identity;

namespace Schematic.BaseInfrastructure.Sqlite
{
    public class CachedUserRoleRepository : UserRoleRepository, IUserRoleRepository<UserRole>
    {
        private readonly IMemoryCache _cache;

        public CachedUserRoleRepository(
            IOptionsMonitor<SchematicSettings> settings,
            IMemoryCache cache) : base(settings)
        {
            _cache = cache;
        }

        public new async Task<List<UserRole>> ListAsync()
        {
            string cacheKey = CacheKeys.UserRoleListCacheKey;

            if (!_cache.TryGetValue(cacheKey, out List<UserRole> cacheEntry))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                cacheEntry = await base.ListAsync();
                
                _cache.Set(cacheKey, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}