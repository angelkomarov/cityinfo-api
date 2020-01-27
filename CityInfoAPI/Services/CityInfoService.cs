using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
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
        private ILogger<CitiesService> _logger;
        private ICityInfoRepository _cityInfoRepository;

        public CitiesService(ICityInfoRepository cityInfoRepository, ILogger<CitiesService> logger)
        {
            _logger = logger;
            _cityInfoRepository = cityInfoRepository;
        }

        #region sync

        public IEnumerable<CityWithoutPointsOfInterestDto> GetCities()
        {
            try {
                var cityEntities = _cityInfoRepository.GetCities();
                //map entity to dto use mapping: <CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>
                return Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
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
                var cityEntity = _cityInfoRepository.GetCity(cityId, includePointsOfInterest);

                if (cityEntity == null)
                    return null;

                if (includePointsOfInterest)
                {
                    //map entity to dto - use mapping: <CreateMap<Entities.City, Models.CityDto>
                    return Mapper.Map<CityDto>(cityEntity);
                }

                //map entity to dto use mapping: CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>
                return Mapper.Map<CityWithoutPointsOfInterestDto>(cityEntity);
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
                var cityEntities = await _cityInfoRepository.GetAllCityInfoAsync(cancellationToken);
                //map entity to dto use mapping:CreateMap<Entities.City, Models.CityDto>
                return Mapper.Map<IEnumerable<CityDto>>(cityEntities);
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
                var cityEntities = await _cityInfoRepository.GetCitiesAsync(cancellationToken);
                //map entity to dto use mapping:CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>
                return Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
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
                var cityEntity = await _cityInfoRepository.GetCityAsync(cityId, includePointsOfInterest, cancellationToken);

                if (cityEntity == null)
                    return null;

                if (includePointsOfInterest)
                {
                    //map entity to dto - use mapping: CreateMap<Entities.City, Models.CityDto>
                    return Mapper.Map<CityDto>(cityEntity);
                }

                //map entity to dto use mapping: CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>
                return Mapper.Map<CityWithoutPointsOfInterestDto>(cityEntity);
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
