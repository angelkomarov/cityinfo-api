using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    //create custom service implementation
    public class CloudMailService : IMailService
    {
        private string _mailTo = CityInfoAPI.Startup.Configuration["mailSettings:mailToAddress"];
        private string _mailFrom = CityInfoAPI.Startup.Configuration["mailSettings:mailFromAddress"];

        public void Send(string subject, string message)
        {
            // send mail - output to debug window
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo}, using CloudMailService.");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }

        public async Task SendAsync(string subject, string message)
        {
            // send mail - output to debug window
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo}, using CloudMailService.");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");

            await Task.Delay(100);
        }

    }
}
