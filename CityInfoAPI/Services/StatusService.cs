using CityInfoAPI.BL.Interfaces;
using CityInfo.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public class StatusService : IStatusService
    {
        private IStatusOperation _statusOperation;
        private ILogger<StatusService> _logger;

        public StatusService(IStatusOperation statusOperation, ILogger<StatusService> logger)
        {
            _statusOperation = statusOperation;
            _logger = logger;
        }

        public async Task<string> GetStatusAsync(CancellationToken cancellationToken)
        {
            string result = string.Empty;

            try
            {
                result =  await _statusOperation.GetStatusAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetStatus error!!!");
                result = "Error:" + ex.Message;
            }

            return result;
        }

        public async Task<string> GetVersion()
        {
            return await _statusOperation.GetVersion();
        }

    }
}
