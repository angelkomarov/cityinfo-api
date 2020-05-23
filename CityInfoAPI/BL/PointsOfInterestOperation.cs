using AutoMapper;
using CityInfo.API.DAL.Interfaces;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Util.Interfaces;
using CityInfoAPI.BL.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CityInfoAPI.BL
{
    public class PointsOfInterestOperation : IPointsOfInterestOperation
    {
        private ICityInfoRepository _cityInfoRepository;
        private IMailService _mailService;
        private ILogger<PointsOfInterestOperation> _logger;

        public PointsOfInterestOperation(ICityInfoRepository cityInfoRepository, IMailService mailService, ILogger<PointsOfInterestOperation> logger)
        {
            _cityInfoRepository = cityInfoRepository;
            _mailService = mailService;
            _logger = logger;
        }

        #region sync

        public IEnumerable<PointOfInterestDto> GetPointsOfInterestForCity(int cityId)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                _logger.LogError($"City does not exist: {cityId}");
                return null;
            }
            var pointsOfInterestForCityEntities = _cityInfoRepository.GetPointsOfInterestForCity(cityId);

            //map entity to dto use mapping: <Entities.PointOfInterest, Models.PointOfInterestDto>
            return Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCityEntities);
        }

        public PointOfInterestDto GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                _logger.LogError($"City does not exist: {cityId}");
                return null;
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                _logger.LogError($"Point of Interest {pointOfInterestId} for city {cityId} does not exist");
                return null;
            }

            //map entity to dto use mapping: <Entities.PointOfInterest, Models.PointOfInterestDto>
            return Mapper.Map<PointOfInterestDto>(pointOfInterestEntity);
        }
        public PointOfInterestDto AddPointOfInterestForCity(int cityId, PointOfInterestForCreationDto pointOfInterestDto)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                _logger.LogError($"City does not exist: {cityId}");
                return null;
            }

            //!!AK7.2 map dto to entity use mapping: note mapper maps subset of fields
            //<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>
            var pointOfInterestEntity = Mapper.Map<PointOfInterest>(pointOfInterestDto);
            //Add created entity
            _cityInfoRepository.AddPointOfInterestForCity(cityId, pointOfInterestEntity);
            //save new entity to DB
            if (!_cityInfoRepository.Save())
                throw new ApplicationException($"A problem while saving Point of Int for city Id: {cityId}.");

            //map entity back to dto: to get the new generated id (generated after Save()) 
            //<Entities.PointOfInterest, Models.PointOfInterestDto>
            return Mapper.Map<PointOfInterestDto>(pointOfInterestEntity);
        }
        public bool UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                _logger.LogError($"City does not exist: {cityId}");
                return false;
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                _logger.LogError($"Point of Interest {pointOfInterestId} for city {cityId} does not exist");
                return false;
            }

            //we do full update on PUT request (if some field is missed then it is populated with null!!!)
            //map dto to entity use mapping: <PointOfInterestForUpdateDto, Entities.PointOfInterest>()
            Mapper.Map(pointOfInterestDto, pointOfInterestEntity);

            if (!_cityInfoRepository.Save())
                throw new ApplicationException($"A problem while saving city Id: {cityId}, Point Of Int: {pointOfInterestId} .");

            return true;
        }
        public void PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto)
        {
            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, pointOfInterestId);

            //<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>
            Mapper.Map(pointOfInterestDto, pointOfInterestEntity);
            if (!_cityInfoRepository.Save())
                throw new ApplicationException($"A problem while saving city Id: {cityId}, Point Of Int: {pointOfInterestId} .");
        }

        public bool DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                _logger.LogError($"City does not exist: {cityId}");
                return false;
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                _logger.LogError($"Point of Interest {pointOfInterestId} for city {cityId} does not exist");
                return false;
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            if (!_cityInfoRepository.Save())
                throw new ApplicationException($"A problem while saving city Id: {cityId}, Point Of Int: {pointOfInterestId} .");

            //use custom service
            _mailService.Send("Point of interest deleted.",
                    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return true;
        }

        #endregion

        #region async

        public async Task<IEnumerable<PointOfInterestDto>> GetPointsOfInterestForCityAsync(int cityId, CancellationToken cancellationToken)
        {
            bool cityExist = await _cityInfoRepository.CityExistsAsync(cityId, cancellationToken);
            if (!cityExist)
            {
                _logger.LogError($"City does not exist: {cityId}");
                return null;
            }
            var pointsOfInterestForCityEntities = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId, cancellationToken);

            //map entity to dto use mapping: <Entities.PointOfInterest, Models.PointOfInterestDto>
            return Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCityEntities);
        }
        public async Task<PointOfInterestDto> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId, CancellationToken cancellationToken)
        {
            bool cityExist = await _cityInfoRepository.CityExistsAsync(cityId, cancellationToken);
            if (!cityExist)
            {
                _logger.LogError($"City does not exist: {cityId}");
                return null;
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId, cancellationToken);
            if (pointOfInterestEntity == null)
            {
                _logger.LogError($"Point of Interest {pointOfInterestId} for city {cityId} does not exist");
                return null;
            }

            //map entity to dto use mapping: <Entities.PointOfInterest, Models.PointOfInterestDto>
            return Mapper.Map<PointOfInterestDto>(pointOfInterestEntity);
        }

        public async Task<PointOfInterestDto> AddPointOfInterestForCityAsync(int cityId, PointOfInterestForCreationDto pointOfInterestDto, CancellationToken cancellationToken)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId, cancellationToken))
            {
                _logger.LogError($"City does not exist: {cityId}");
                return null;
            }

            //!!AK7.2 map dto to entity use mapping: note mapper maps subset of fields
            //<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>
            var pointOfInterestEntity = Mapper.Map<PointOfInterest>(pointOfInterestDto);
            //Add created entity
            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, pointOfInterestEntity, cancellationToken);
            //save new entity to DB
            if (!await _cityInfoRepository.SaveAsync(cancellationToken))
                throw new ApplicationException($"A problem while saving Point of Int for city Id: {cityId}.");

            //map entity back to dto: to get the new generated id (generated after Save()) 
            //<Entities.PointOfInterest, Models.PointOfInterestDto>
            return Mapper.Map<PointOfInterestDto>(pointOfInterestEntity);
        }

        public async Task<bool> UpdatePointOfInterestAsync(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto, CancellationToken cancellationToken)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId, cancellationToken))
            {
                _logger.LogError($"City does not exist: {cityId}");
                return false;
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId, cancellationToken);
            if (pointOfInterestEntity == null)
            {
                _logger.LogError($"Point of Interest {pointOfInterestId} for city {cityId} does not exist");
                return false;
            }

            //we do full update on PUT request (if some field is missed then it is populated with null!!!)
            //map dto to entity use mapping: <PointOfInterestForUpdateDto, Entities.PointOfInterest>()
            Mapper.Map(pointOfInterestDto, pointOfInterestEntity);

            if (!await _cityInfoRepository.SaveAsync(cancellationToken))
                throw new ApplicationException($"A problem while saving city Id: {cityId}, Point Of Int: {pointOfInterestId} .");

            return true;
        }

        public async Task PartiallyUpdatePointOfInterestAsync(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto, CancellationToken cancellationToken)
        {
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId, cancellationToken);

            //<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>
            Mapper.Map(pointOfInterestDto, pointOfInterestEntity);
            if (!await _cityInfoRepository.SaveAsync(cancellationToken))
                throw new ApplicationException($"A problem while saving city Id: {cityId}, Point Of Int: {pointOfInterestId} .");
        }

        public async Task<bool> DeletePointOfInterestAsync(int cityId, int pointOfInterestId, CancellationToken cancellationToken)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId, cancellationToken))
            {
                _logger.LogError($"City does not exist: {cityId}");
                return false;
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId, cancellationToken);
            if (pointOfInterestEntity == null)
            {
                _logger.LogError($"Point of Interest {pointOfInterestId} for city {cityId} does not exist");
                return false;
            }

            bool isDetelte = await _cityInfoRepository.DeletePointOfInterestAsync(pointOfInterestEntity, cancellationToken);
            if (!isDetelte)
                throw new ApplicationException($"A problem while deleting city Id: {cityId}, Point Of Int: {pointOfInterestId} .");

            //use custom service
            await _mailService.SendAsync("Point of interest deleted.",
                    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return true;
        }

        #endregion

    }
}
