using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    //create custom service interface
    public interface IMailService
    {
        void Send(string subject, string message);
        Task SendAsync(string subject, string message);
    }
}
