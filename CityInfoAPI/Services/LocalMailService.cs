using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    //create custom service implementation
    public class LocalMailService : IMailService
    {
        private string _mailTo = CityInfoAPI.Startup.Configuration["mailSettings:mailToAddress"];
        private string _mailFrom = CityInfoAPI.Startup.Configuration["mailSettings:mailFromAddress"];
        private string _mailServer = "";
        private ILogger<LocalMailService> _logger;

        public LocalMailService(string mailServer, ILogger<LocalMailService> logger)
        {
            _mailServer = mailServer;
            _logger = logger;
        }

        public void Send(string subject, string message)
        {
            // send mail - output to debug window
            _logger.LogInformation($"Mail from {_mailFrom} to {_mailTo}, using LocalMailService  - server: {_mailServer}.");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Message: {message}");
        }

        public async Task SendAsync(string subject, string message)
        {
            // send mail - output to debug window
            _logger.LogInformation($"Mail from {_mailFrom} to {_mailTo}, using LocalMailService  - server: {_mailServer}.");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Message: {message}");
            await Task.Delay(100);
        }

    }
}
