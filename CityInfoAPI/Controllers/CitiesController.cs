using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CityInfo.API.Models;
using CityInfo.API.Services.Interfaces;

namespace CityInfo.API.Controllers
{
    //attribute routing
    [Route("api/cities")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private ICitiesService _cityInfoSvc;
        //!!AK6.1 inject REpository
        public CitiesController(ICitiesService cityInfoSvc)
        {
            _cityInfoSvc = cityInfoSvc;
        }

        #region sync

        //https://localhost:44313/api/cities
        [HttpGet()]
        public ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>> GetCities()
        {
            var cities = _cityInfoSvc.GetCities();
            //using built in helper methods on the controller
            return Ok(cities);
        }

        //note: if we had routing attribute: [HttpGet("{id}/{includePointsOfInterest}")]
        //then request would be: https://localhost:44313/api/cities/1/false
        //but in our case second attr comes from query string.
        //https://localhost:44313/api/cities/1
        //https://localhost:44313/api/cities/1?includePointsOfInterest=true
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CityWithoutPointsOfInterestDto> GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = _cityInfoSvc.GetCity(id, includePointsOfInterest);

            if (city == null)
                return NotFound();
            else
                return Ok(city);
        }

        #endregion

        #region async

        //https://localhost:44313/api/cities/async/pointsofinterest
        //https://cityinfoapi2.azurewebsites.net/api/cities/async/pointsofinterest
        [HttpGet("async/pointsofinterest")]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetAllCityInfoAsync(int cityId, CancellationToken cancellationToken)
        {
            var cities = await _cityInfoSvc.GetAllCityInfoAsync(cancellationToken);
            //using built in helper methods on the controller
            return Ok(cities);
        }

        //https://localhost:44313/api/cities/async
        //https://cityinfoapiappsvc.azurewebsites.net/api/cities/async
        [HttpGet()]
        [Route("async")]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCitiesAsync(CancellationToken cancellationToken)
        {
            var cities = await _cityInfoSvc.GetCitiesAsync(cancellationToken);
            //using built in helper methods on the controller
            return Ok(cities);
        }

        //note: if we had routing attribute: [HttpGet("{id}/{includePointsOfInterest}")]
        //then request would be: https://localhost:44313/api/cities/1/false
        //but in our case second attr comes from query string.
        //https://localhost:44313/api/cities/async/1?includePointsOfInterest=false
        //https://localhost:44313/api/cities/async/1?includePointsOfInterest=true
        //https://cityinfoapiappsvc.azurewebsites.net/api/cities/async/1?includePointsOfInterest=false
        //https://cityinfoapi2.azurewebsites.net/api/cities/async/1?includePointsOfInterest=false
        [HttpGet("async/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CityWithoutPointsOfInterestDto>> GetCityAsync(int id, bool includePointsOfInterest, CancellationToken cancellationToken)
        {
            var city = await _cityInfoSvc.GetCityAsync(id, includePointsOfInterest, cancellationToken);

            if (city == null)
                return NotFound();
            else
                return Ok(city);
        }

        #endregion

    }
}
