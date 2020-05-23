using CityInfo.API.Models;
using CityInfo.API.Services.Interfaces;
using CityInfoAPI.BL.Interfaces;
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
    public class PointsOfInterestService : IPointsOfInterestService
    {
        private IPointsOfInterestOperation _pointsOfInterestOperation;
        private ILogger<PointsOfInterestService> _logger;

        public PointsOfInterestService(IPointsOfInterestOperation pointsOfInterestOperation, ILogger<PointsOfInterestService> logger)
        {
            _pointsOfInterestOperation = pointsOfInterestOperation;
            _logger = logger;
        }

        #region sync

        public IEnumerable<PointOfInterestDto> GetPointsOfInterestForCity(int cityId)
        {
            try
            {
                return _pointsOfInterestOperation.GetPointsOfInterestForCity(cityId);
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
                return _pointsOfInterestOperation.GetPointOfInterestForCity(cityId, pointOfInterestId);
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
                return _pointsOfInterestOperation.AddPointOfInterestForCity(cityId, pointOfInterestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddPointOfInterestForCity error!!!");
                throw;
            }
        }
        public bool UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto)
        {
            try
            {
                return _pointsOfInterestOperation.UpdatePointOfInterest(cityId, pointOfInterestId, pointOfInterestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdatePointOfInterest error!!!");
                throw;
            }
        }
        public void PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto)
        {
            try
            {
                _pointsOfInterestOperation.PartiallyUpdatePointOfInterest(cityId, pointOfInterestId, pointOfInterestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PartiallyUpdatePointOfInterest error!!!");
                throw;
            }
        }

        public bool DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            try
            {
                return _pointsOfInterestOperation.DeletePointOfInterest(cityId, pointOfInterestId);
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
                return await _pointsOfInterestOperation.GetPointsOfInterestForCityAsync(cityId, cancellationToken);
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
                return await _pointsOfInterestOperation.GetPointOfInterestForCityAsync(cityId, pointOfInterestId, cancellationToken);
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
                return await _pointsOfInterestOperation.AddPointOfInterestForCityAsync(cityId, pointOfInterestDto, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddPointOfInterestForCity error!!!");
                throw;
            }
        }

        public async Task<bool> UpdatePointOfInterestAsync(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterestDto, CancellationToken cancellationToken)
        {
            try
            {
                return await _pointsOfInterestOperation.UpdatePointOfInterestAsync(cityId, pointOfInterestId, pointOfInterestDto, cancellationToken);
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
                await _pointsOfInterestOperation.PartiallyUpdatePointOfInterestAsync(cityId, pointOfInterestId, pointOfInterestDto, cancellationToken);
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
                return await _pointsOfInterestOperation.DeletePointOfInterestAsync(cityId, pointOfInterestId, cancellationToken);
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
