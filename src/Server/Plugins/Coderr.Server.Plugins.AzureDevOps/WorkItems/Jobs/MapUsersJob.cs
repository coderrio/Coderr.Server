using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Directory;
using Coderr.Server.Abstractions.WorkItems;
using Coderr.Server.Common.AzureDevOps.App.Clients;
using Coderr.Server.Domain.Core.User;
using Griffin.ApplicationServices;
using log4net;
using ISettingsRepository = Coderr.Server.Common.AzureDevOps.App.Connections.ISettingsRepository;

namespace Coderr.Server.Common.AzureDevOps.App.WorkItems.Jobs
{
    /// <summary>
    /// Map active directory users
    /// </summary>
    [ContainerService(RegisterAsSelf = true)]
    class MapUsersJob : IBackgroundJobAsync
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(SynchronizeWorkItemsJob));
        private readonly ISettingsRepository _settingsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDirectoryService _directoryService;
        private readonly IUserMappingRepository _userMappingRepository;
        private static List<string> _missingEmail = new List<string>();
        private static List<string> _missingAccount = new List<string>();

        public MapUsersJob(IUserMappingRepository userMappingRepository, ISettingsRepository settingsRepository, IUserRepository userRepository, IDirectoryService directoryService)
        {
            _userMappingRepository = userMappingRepository;
            _settingsRepository = settingsRepository;
            _userRepository = userRepository;
            _directoryService = directoryService;
        }

        public async Task ExecuteAsync()
        {
            // Complain every new day
            if (DateTime.UtcNow.Hour == 0 && DateTime.UtcNow.Minute < 10)
            {
                _missingAccount.Clear();
                _missingEmail.Clear();
            }

            //TODO: Merge list from all applications and then lookup.
            var settings = await _settingsRepository.GetAll();
            foreach (var setting in settings)
            {
                var settingsClient = new SettingsClient(setting.PersonalAccessToken, setting.Url);
                var client = new WorkItemClient(setting);
                var members = await client.GetMembers();
                foreach (var member in members)
                {
                    var mapping = await _userMappingRepository.GetByExternalId(member.Id);
                    if (mapping != null)
                    {
                        continue;
                    }

                    var email = await settingsClient.GetEmailAddress(member.Descriptor) ??
                                _directoryService.FindEmail(member.UniqueName);
                    if (email == null)
                    {
                        if (!_missingEmail.Contains(member.UniqueName))
                        {
                            _logger.Debug("Could not find email for " + member.Descriptor + "/" + member.UniqueName);
                            _missingEmail.Add(member.UniqueName);
                        }
                        
                        continue;
                    }

                    var account = await _userRepository.FindByEmailAsync(email);
                    if (account == null)
                    {
                        if (!_missingAccount.Contains(email))
                        {
                            _logger.Warn("Email is not registered: " + email + " for any user.");
                            _missingAccount.Add(email);
                        }
                        
                        continue;
                    }

                    var map = new WorkItemUserMapping
                    {
                        AccountId = account.AccountId,
                        ExternalId = member.Id,
                        AdditionalData = new Dictionary<string, string>()
                        {
                            {"UniqueName", member.UniqueName}, {"Descriptor", member.Descriptor}
                        }
                    };
                    await _userMappingRepository.Create(map);
                }
            }
            
        }
    }
}
