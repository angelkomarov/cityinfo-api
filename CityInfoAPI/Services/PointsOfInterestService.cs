using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public enum UpdatePoints { NOT_FOUND, OK} 
    public class PointsOfInterestService : IPointsOfInterestService
    {
        private ICityInfoRepository _cityInfoRepository;
        private IMailService _mailService;
        private ILogger<PointsOfInterestService> _logger;

        public PointsOfInterestService(ICityInfoRepository cityInfoRepository, IMailService mailService, ILogger<PointsOfInterestService> logger)
        {
            _cityInfoRepository = cityInfoRepository;
            _mailService = mailService;
            _logger = logger;
        }

        #region sync

        public IEnumerable<PointOfInterestDto> GetPointsOfInterestForCity(int cityId)
        {
            try {
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogError($"City does not exist: {cityId}");
                    return null;
                }
                var pointsOfInterestForCityEntities = _cityInfoRepository.GetPointsOfInterestForCity(cityId);

                //map entity to dto use mapping: <Entities.PointOfInterest, Models.PointOfInterestDto>
                return Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCityEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPointsOfInterestForCity error!!!");
                throw;
            }
        }

        public PointOfInterestDto GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPointOfInterestForCity error!!!");
                throw;
            }
        }
        public PointOfInterestDto AddPointOfInterestForCity(int cityId, PointOfInterestForCreationDto pointOfInterestDto)
        {
            try
            {
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogError($"City does not exist: {cityId}");
                    return null;
                }

                //!!AK7.2 map dto to entity use mapping: note mapper maps subset of fields
                //<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>
                var pointOfInterestEntity = Mapper.Map<Entities.PointOfInterest>(pointOfInterestDto);
                //Add created entity
                _cityInfoRepository.AddPointOfInterestForCity(cityId, pointOfInterestEntity);
                //save new entity to DB
                if (!_cityInfoRepository.Save())
                    throw new ApplicationException($"A problem while saving Point of Int for city Id: {cityId}.");

                //map entity back to dto: to get the new generated id (generated after Save()) 
                //<Entities.PointOfInterest, Models.PointOfInterestDto>
                return Mapper.Map<PointOfInterestDto>(pointOfInterestEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddPointOfInterestForCity error!!!");
                throw;
            }
        }
        public UpdatePoints UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto)
        {
            try
            {
                var result = UpdatePoints.OK;

                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogError($"City does not exist: {cityId}");
                    result = UpdatePoints.NOT_FOUND;
                }

                var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, pointOfInterestId);
                if (pointOfInterestEntity == null)
                {
                    _logger.LogError($"Point of Interest {pointOfInterestId} for city {cityId} does not exist");
                    result = UpdatePoints.NOT_FOUND;
                }

                //we do full update on PUT request (if some field is missed then it is populated with null!!!)
                //map dto to entity use mapping: <PointOfInterestForUpdateDto, Entities.PointOfInterest>()
                Mapper.Map(pointOfInterestDto, pointOfInterestEntity);

                if (!_cityInfoRepository.Save())
                    throw new ApplicationException($"A problem while saving city Id: {cityId}, Point Of Int: {pointOfInterestId} .");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdatePointOfInterest error!!!");
                throw;
            }
        }
        public void PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto)
        {
            try {
                var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, pointOfInterestId);

                //<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>
                Mapper.Map(pointOfInterestDto, pointOfInterestEntity);
                if (!_cityInfoRepository.Save())
                    throw new ApplicationException($"A problem while saving city Id: {cityId}, Point Of Int: {pointOfInterestId} .");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PartiallyUpdatePointOfInterest error!!!");
                throw;
            }
        }

        public bool DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            try {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeletePointOfInterest error!!!");
                throw;
            }
        }

        #endregion

        #region async

        public async Task<IEnumerable<PointOfInterestDto>> GetPointsOfInterestForCityAsync(int cityId, CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPointsOfInterestForCity error!!!");
                throw;
            }
        }
        public async Task<PointOfInterestDto> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId, CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPointOfInterestForCity error!!!");
                throw;
            }
        }

        public async Task<PointOfInterestDto> AddPointOfInterestForCityAsync(int cityId, PointOfInterestForCreationDto pointOfInterestDto, CancellationToken cancellationToken)
        {
            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId, cancellationToken))
                {
                    _logger.LogError($"City does not exist: {cityId}");
                    return null;
                }

                //!!AK7.2 map dto to entity use mapping: note mapper maps subset of fields
                //<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>
                var pointOfInterestEntity = Mapper.Map<Entities.PointOfInterest>(pointOfInterestDto);
                //Add created entity
                await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, pointOfInterestEntity, cancellationToken);
                //save new entity to DB
                if (!await _cityInfoRepository.SaveAsync(cancellationToken))
                    throw new ApplicationException($"A problem while saving Point of Int for city Id: {cityId}.");

                //map entity back to dto: to get the new generated id (generated after Save()) 
                //<Entities.PointOfInterest, Models.PointOfInterestDto>
                return Mapper.Map<PointOfInterestDto>(pointOfInterestEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddPointOfInterestForCity error!!!");
                throw;
            }
        }

        public async Task<UpdatePoints> UpdatePointOfInterestAsync(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto, CancellationToken cancellationToken)
        {
            try
            {
                var result = UpdatePoints.OK;

                if (!await _cityInfoRepository.CityExistsAsync(cityId, cancellationToken))
                {
                    _logger.LogError($"City does not exist: {cityId}");
                    result = UpdatePoints.NOT_FOUND;
                }

                var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId, cancellationToken);
                if (pointOfInterestEntity == null)
                {
                    _logger.LogError($"Point of Interest {pointOfInterestId} for city {cityId} does not exist");
                    result = UpdatePoints.NOT_FOUND;
                }

                //we do full update on PUT request (if some field is missed then it is populated with null!!!)
                //map dto to entity use mapping: <PointOfInterestForUpdateDto, Entities.PointOfInterest>()
                Mapper.Map(pointOfInterestDto, pointOfInterestEntity);

                if (!await _cityInfoRepository.SaveAsync(cancellationToken))
                    throw new ApplicationException($"A problem while saving city Id: {cityId}, Point Of Int: {pointOfInterestId} .");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdatePointOfInterest error!!!");
                throw;
            }
        }

        public async Task PartiallyUpdatePointOfInterestAsync(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto, CancellationToken cancellationToken)
        {
            try
            {
                var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId, cancellationToken);

                //<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>
                Mapper.Map(pointOfInterestDto, pointOfInterestEntity);
                if (!await _cityInfoRepository.SaveAsync(cancellationToken))
                    throw new ApplicationException($"A problem while saving city Id: {cityId}, Point Of Int: {pointOfInterestId} .");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PartiallyUpdatePointOfInterest error!!!");
                throw;
            }
        }

        public async Task<bool> DeletePointOfInterestAsync(int cityId, int pointOfInterestId, CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeletePointOfInterest error!!!");
                throw;
            }
        }

        #endregion
    }
}
