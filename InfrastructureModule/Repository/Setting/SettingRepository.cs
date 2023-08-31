using DomainModule.Entity;
using DomainModule.RepositoryInterface;
using InfrastructureModule.Context;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureModule.Repository
{
    public class SettingRepository:BaseRepository<SettingEntity>,SettingRepositoryInterface
    {
        private readonly IMemoryCache _memoryCache;
        public SettingRepository(AppDbContext context, IMemoryCache memoryCache) :base(context)
        {
            _memoryCache = memoryCache;
        }

        public async Task<SettingEntity?> GetByKeyAndGroup(string key, string group)
        {

            IList<SettingEntity> Settings = new List<SettingEntity>();
            if (!_memoryCache.TryGetValue(SettingEntity.SettingCacheKey, out Settings))
            {
                _memoryCache.Set(SettingEntity.SettingCacheKey, await GetAllAsync().ConfigureAwait(false));
            }

            Settings = _memoryCache.Get(SettingEntity.SettingCacheKey) as List<SettingEntity>;

            return Settings.SingleOrDefault(a => a.SettingKey == key && a.SettingGroup == group);
        }
    }
}
