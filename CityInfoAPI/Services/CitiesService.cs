using CityInfo.API.Models;
using CityInfo.API.Services.Interfaces;
using CityInfoAPI.BL.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    //Services lib does basic error logging and re-trows the exception.
    //It is a server component and can be used from other clients - Winforms, Mobile App, Web app.
    public class CitiesService : ICitiesService
    {
        private ICitiesOperation _citiesOperation;
        private ILogger<CitiesService> _logger;

        public CitiesService(ICitiesOperation citiesOperation, ILogger<CitiesService> logger)
        {
            _logger = logger;
            _citiesOperation = citiesOperation;
        }

        #region sync

        public IEnumerable<CityWithoutPointsOfInterestDto> GetCities()
        {
            try {
                var cityDTOs = _citiesOperation.GetCities();
                return cityDTOs;
            }
            catch (Exception ex)
            {
                //do logging or any other actions...
                _logger.LogError(ex, "GetCities error!!!"); 
                //throw new Exception(errorMsg, ex); // rethrow the exceprion new + the original: ex
                throw; //rethrow the original ex (full fn call stack)
            }
        }

        public CityWithoutPointsOfInterestDto GetCity(int cityId, bool includePointsOfInterest)
        {
            try {
                var cityDTO = _citiesOperation.GetCity(cityId, includePointsOfInterest);
                return cityDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCity error!!!");
                throw; 
            }
        }

        #endregion

        #region async

        public async Task<IEnumerable<CityDto>> GetAllCityInfoAsync(CancellationToken cancellationToken)
        {
            try
            {
                var cityDTOs = await _citiesOperation.GetAllCityInfoAsync(cancellationToken);
                return cityDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCitiesAsync error!!!");
                throw;
            }
        }

        public async Task<IEnumerable<CityWithoutPointsOfInterestDto>> GetCitiesAsync(CancellationToken cancellationToken)
        {
            try {
                var cityDTOs = await _citiesOperation.GetCitiesAsync(cancellationToken);
                return cityDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCitiesAsync error!!!");
                throw; 
            }
        }

        public async Task<CityWithoutPointsOfInterestDto> GetCityAsync(int cityId, bool includePointsOfInterest, CancellationToken cancellationToken)
        {
            try
            {
                var cityDTO = await _citiesOperation.GetCityAsync(cityId, includePointsOfInterest, cancellationToken);
                return cityDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCityAsync error!!!");
                throw;
            }
        }

        #endregion
    }
}
