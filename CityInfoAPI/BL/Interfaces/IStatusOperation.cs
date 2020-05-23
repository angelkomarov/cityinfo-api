using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CityInfoAPI.BL.Interfaces
{
    public interface IStatusOperation
    {
        Task<string> GetStatusAsync(CancellationToken cancellationToken);
        Task<string> GetVersion();
    }
}
