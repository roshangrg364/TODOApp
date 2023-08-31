using DomainModule.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModule.ServiceInterface
{
    public interface ApplicationSettingServiceInterface
    {
        //Email
        Task<string?> GetEmailUserName();
        Task SetEmailUserName(string apiEndPoint);
        Task<string?> GetEmailPassword();
        Task SetEmailPassword(string apiEndPoint);
        Task<string?> GetHostServer();
        Task SetHostServer(string value);
        Task<string?> GetPort();
        Task SetPort(string value);
        Task<string?> GetDefaultSender();
        Task SetDefaultSender(string value);
        Task<EmailConfigurationDto> GetEmailConfigurationDetails();
         Task SetEmailConfigurationDetails(EmailConfigurationDto model);

    }
}
