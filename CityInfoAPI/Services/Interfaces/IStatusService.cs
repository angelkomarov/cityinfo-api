using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CityInfo.API.Services.Interfaces
{
    public interface IStatusService
    {
        Task<string> GetStatusAsync(CancellationToken cancellationToken);
        Task<string> GetVersion();
    }
}
