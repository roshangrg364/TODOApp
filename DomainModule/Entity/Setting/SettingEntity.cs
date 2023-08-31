using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.Entity
{
    public class SettingEntity
    {
      
        public const string SettingCacheKey = "SettingKey";
        public int SettingId { get; set; }
        public string SettingKey { get; set; }
        public  string SettingGroup { get; set; }
        public  string SettingValue { get; set; }
    }
}
