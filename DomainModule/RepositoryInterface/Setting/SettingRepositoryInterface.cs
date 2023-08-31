using DomainModule.BaseRepo;
using DomainModule.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.RepositoryInterface
{
    public interface SettingRepositoryInterface:BaseRepositoryInterface<SettingEntity>
    {
        Task<SettingEntity?> GetByKeyAndGroup(string key, string group);
    }
}
