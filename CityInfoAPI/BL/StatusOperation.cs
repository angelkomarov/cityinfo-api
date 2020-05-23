using CityInfo.API.DAL.Interfaces;
using CityInfoAPI.BL.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CityInfoAPI.BL
{
    public class StatusOperation: IStatusOperation
    {
        private ICityInfoRepository _cityInfoRepository;
        private ILogger<StatusOperation> _logger;

        public StatusOperation(ICityInfoRepository cityInfoRepository, ILogger<StatusOperation> logger)
        {
            _cityInfoRepository = cityInfoRepository;
            _logger = logger;
        }

        public async Task<string> GetStatusAsync(CancellationToken cancellationToken)
        {
            string result = "Unknown Error";

            if (await _cityInfoRepository.HealthCheckAsync(cancellationToken))
                result = "Health Check OK";

            return result;
        }

        public Task<string> GetVersion()
        {
            return Task.FromResult(Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
    }
}
