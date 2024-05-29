using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.ServiceInterface
{
    public interface ISettingService
    {
        Task<string> Get(string key, string group, string defaultValue = "");
        Task Set(string key, string group, string value);
    }
}
