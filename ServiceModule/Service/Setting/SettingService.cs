using DomainModule.Entity;
using DomainModule.RepositoryInterface;
using DomainModule.ServiceInterface;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceModule.Service
{
    public class SettingService : ISettingService
    {

        private readonly ISettingRepository _settingRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IUnitOfWork _unitOfWork;

        public SettingService(ISettingRepository settingRepository,
            IMemoryCache memoryCache,
            IUnitOfWork unitOfWork)
        {
            _settingRepository = settingRepository;
            _memoryCache = memoryCache;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Get(string key, string group, string defaultValue = "")
        {

            var setting = await GetEntity(key, group);
            return setting == null ? defaultValue : setting.SettingValue;
        }

        public async Task Set(string key, string group, string value)
        {
            var setting = await GetEntity(key, group);
            if (setting == null)
            {
                setting = new SettingEntity
                {
                    SettingGroup = group,
                    SettingKey = key,
                    SettingValue = value
                };
                await _settingRepository.InsertAsync(setting);
                var SettingInMemoryCache = _memoryCache.Get(SettingEntity.SettingCacheKey) as List<SettingEntity>;
                SettingInMemoryCache?.Add(setting);
                RemoveOldAndAddNewDataInMemoryCache(SettingInMemoryCache);
            }
            else
            {
                setting.SettingValue = value;
                _settingRepository.Update(setting);
                var SettingInMemoryCache = _memoryCache.Get(SettingEntity.SettingCacheKey) as List<SettingEntity>;

                var OldSettingData = SettingInMemoryCache?.FirstOrDefault(a => a.SettingKey == key && a.SettingGroup == group);
                var OldSettingDataIndex = SettingInMemoryCache.FindIndex(a => a.SettingKey == key && a.SettingGroup == group);

                SettingInMemoryCache?.Remove(OldSettingData);
                SettingInMemoryCache?.Insert(OldSettingDataIndex, setting);

                RemoveOldAndAddNewDataInMemoryCache(SettingInMemoryCache);
            }
            await _unitOfWork.CompleteAsync().ConfigureAwait(false);
        }

        private void RemoveOldAndAddNewDataInMemoryCache(List<SettingEntity> SettingInMemoryCache)
        {
            _memoryCache.Remove(SettingEntity.SettingCacheKey);
            _memoryCache.Set(SettingEntity.SettingCacheKey, SettingInMemoryCache);
        }

        private async Task<SettingEntity?> GetEntity(string key, string group)
        {
            return await _settingRepository.GetByKeyAndGroup(key, group);
        }

    }
}
