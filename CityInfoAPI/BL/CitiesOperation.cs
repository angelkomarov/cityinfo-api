using AutoMapper;
using CityInfo.API.DAL.Interfaces;
using CityInfo.API.Models;
using CityInfoAPI.BL.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CityInfoAPI.BL
{
    public class CitiesOperation : ICitiesOperation
    {
        private ICityInfoRepository _cityInfoRepository;
        private ILogger<CitiesOperation> _logger;

        public CitiesOperation(ICityInfoRepository cityInfoRepository, ILogger<CitiesOperation> logger)
        {
            _cityInfoRepository = cityInfoRepository;
            _logger = logger;
        }

        #region sync

        public IEnumerable<CityWithoutPointsOfInterestDto> GetCities()
        {
            var cityEntities = _cityInfoRepository.GetCities();
            //map entity to dto use mapping: <CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>
            return Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
        }

        public CityWithoutPointsOfInterestDto GetCity(int cityId, bool includePointsOfInterest)
        {
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

        #endregion

        #region async

        public async Task<IEnumerable<CityDto>> GetAllCityInfoAsync(CancellationToken cancellationToken)
        {
            var cityEntities = await _cityInfoRepository.GetAllCityInfoAsync(cancellationToken);
            //map entity to dto use mapping:CreateMap<Entities.City, Models.CityDto>
            return Mapper.Map<IEnumerable<CityDto>>(cityEntities);
        }

        public async Task<IEnumerable<CityWithoutPointsOfInterestDto>> GetCitiesAsync(CancellationToken cancellationToken)
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync(cancellationToken);
            //map entity to dto use mapping:CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>
            return Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
        }

        public async Task<CityWithoutPointsOfInterestDto> GetCityAsync(int cityId, bool includePointsOfInterest, CancellationToken cancellationToken)
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

        #endregion
    }
}
