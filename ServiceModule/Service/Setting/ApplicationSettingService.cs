using DomainModule.Dto;
using DomainModule.RepositoryInterface;
using DomainModule.ServiceInterface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceModule.Service
{
    public class ApplicationSettingService : IApplicationSettingService
    {
        private readonly ISettingService _settingService;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public ApplicationSettingService(ISettingService settingService,
            IUnitOfWork unitOfWork)
        {
            _settingService = settingService;
            _unitOfWork = unitOfWork;
        }
        //EmailGroup and Key
        public const string EmailGroup = "Email_Group";
        public const string EmailUserName = "Email_UserName";
        public const string EmailPassword = "Email_Password";
        public const string HostServer = "Host_Server";
        public const string Port = "Port";
        public const string DefaultSender = "Default_Sender";


        public async Task<string?> GetDefaultSender()
        {
            var defaultSender = await _settingService.Get(DefaultSender, EmailGroup).ConfigureAwait(false);
            return defaultSender;
        }

       

        public async Task<string?> GetEmailPassword()
        {
            var emailPassword = await _settingService.Get(EmailPassword, EmailGroup).ConfigureAwait(false);
            return emailPassword;
        }

        public async Task<string?> GetEmailUserName()
        {
            var emailUserName = await _settingService.Get(EmailUserName, EmailGroup).ConfigureAwait(false);
            return emailUserName;
        }

        public async Task<string?> GetHostServer()
        {
            var defaultServer = await _settingService.Get(HostServer, EmailGroup).ConfigureAwait(false);
            return defaultServer;
        }

        public async Task<string?> GetPort()
        {
            var emailPort = await _settingService.Get(HostServer, EmailGroup).ConfigureAwait(false);
            return emailPort;
        }

        public async Task<EmailConfigurationDto> GetEmailConfigurationDetails()
        {
            var returnModel = new EmailConfigurationDto
            {
                HostServer = await GetHostServer().ConfigureAwait(false),
                Username = await GetEmailUserName().ConfigureAwait(false),
                Password = await GetEmailPassword().ConfigureAwait(false),
                Port = await GetPort().ConfigureAwait(false),
                DefaultSender = await GetDefaultSender().ConfigureAwait(false),
            };
            return returnModel;
        }

        public  async Task SetDefaultSender(string value)
        {
            await _settingService.Set(DefaultSender, EmailGroup, value).ConfigureAwait(false);
        }

       

        public async Task SetEmailPassword(string value)
        {
            await _settingService.Set(EmailPassword, EmailGroup, value).ConfigureAwait(false);
        }

        public async Task SetEmailUserName(string value)
        {
            await _settingService.Set(EmailUserName, EmailGroup, value).ConfigureAwait(false);

        }

        public async Task SetHostServer(string value)
        {
            await _settingService.Set(HostServer, EmailGroup, value).ConfigureAwait(false);

        }

        public async Task SetPort(string value)
        {
            await _settingService.Set(Port, EmailGroup, value).ConfigureAwait(false);

        }

        public async Task SetEmailConfigurationDetails(EmailConfigurationDto model)
        {
            using (var tx = await _unitOfWork.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    await SetEmailUserName(model.Username).ConfigureAwait(false);
                    await SetEmailPassword(model.Password).ConfigureAwait(false);
                    await SetHostServer(model.HostServer).ConfigureAwait(false);
                    await SetPort(model.Port).ConfigureAwait(false);
                    await SetDefaultSender(model.DefaultSender).ConfigureAwait(false);
                    await _unitOfWork.CompleteAsync().ConfigureAwait(false);
                    await tx.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
            }
            
        }
    }
}
