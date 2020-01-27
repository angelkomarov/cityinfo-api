using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    //return child resources - define proper route: api/cities/{cityId}/pointsofinterest
    [Route("api/cities")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private IPointsOfInterestService _pointsOfInterestSvc;

        //inject the framework logger and IMailService to controllerconstructor
        //!!AK6.1 inject Repository
        public PointsOfInterestController(IPointsOfInterestService pointsOfInterestSvc)
        {
            //inject custom services
            _pointsOfInterestSvc = pointsOfInterestSvc;
        }

        #region sync

        //https://localhost:44313/api/cities/1/pointsofinterest
        //https://cityinfoapiappsvc.azurewebsites.net/api/cities/1/pointsofinterest
        [HttpGet("{cityId}/pointsofinterest")]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var pointsOfInterestForCity = _pointsOfInterestSvc.GetPointsOfInterestForCity(cityId);

            if (pointsOfInterestForCity == null)
                return NotFound();
            else
                return Ok(pointsOfInterestForCity);
        }

        //https://localhost:44313/api/cities/1/pointsofinterest/1
        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterestLink")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var pointOfInterest = _pointsOfInterestSvc.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterest == null)
                return NotFound();
            else
                return Ok(pointOfInterest);
        }

        /*
        https://localhost:44313/api/cities/1/pointsofinterest
        POST request headers:
        Content-Type:application/json
        Accept: application/xml

        {
            "name": "Pere Lachaise",
            "description": "Famous cemetery where Jim Morrison and Oscar Wilde are buried."

         }
        */
        //get cityid from query string + deserialize from  body pointOfInterest
        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest) 
        {
            //1.validate
            //if deserialization fails then return 404
            if (pointOfInterest == null)
                return BadRequest();
            //do custom check and set modelstate error
            if (pointOfInterest.Description == pointOfInterest.Name)
                ModelState.AddModelError("title", "The provided description should be different from the name.");
            //check modelstate and return 404 with error message set - custom or data annotation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //2.save
            //if City resource does not exist then return 404
            var pointOfInterestResult = _pointsOfInterestSvc.AddPointOfInterestForCity(cityId, pointOfInterest);
            if (pointOfInterestResult == null)
                return NotFound();
            //3.create response
            //use this helper method returns 201 and location header with link to the newly resource
            //Also returns GetPointOfInterestLink and pass 2 params. 
            //Also pass newly created object in the response body
            return CreatedAtRoute("GetPointOfInterestLink",
                new { cityId = cityId, id = pointOfInterestResult.Id }, pointOfInterestResult);
        }

        /*
         * https://localhost:44313/api/cities/1/pointsofinterest/1
        PUT request headers:
        Content-Type:application/json
        Accept: application/xml

        {
            "name": "New York City- updated",
            "description": "The one with that big park."

         }
        */
        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            //1.validate
            if (pointOfInterest == null)
                return BadRequest();
            if (pointOfInterest.Description == pointOfInterest.Name)
                ModelState.AddModelError("title", "The provided description should be different from the name.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //2.save
            if (_pointsOfInterestSvc.UpdatePointOfInterest(cityId, id, pointOfInterest) == UpdatePoints.NOT_FOUND)
                return NotFound();

            return NoContent();
        }

        /*
        https://localhost:44313/api/cities/1/pointsofinterest/1
        PATCH request headers:
        Content-Type:application/json
        Accept: application/json

        Body:
        Req1 - OK
        [{ "op": "replace", "path": "/name", "value": "Town Hall" }]

        Req2 - Error
        [{ "op": "replace", "path": "/name", "value": "Town Hall" }, { "op": "remove",  "path": "/name" }]

        Response - Error
        { "Name": ["You should provide a name value."] }
        */
        //Same as full update but FromBody param specifies JsonPatchDocument<T>
        //where T is our DTO object that contains the partial data for update
        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patch)
        {
            //1.validate
            if (patch == null)
                return BadRequest();

            var pointOfInterestDto = _pointsOfInterestSvc.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestDto == null)
                return NotFound();

            //convert the DTO to partial DTO (same as the input param)
            //<Models.PointOfInterestDto, Models.PointOfInterestForUpdateDto>
            var piToPatch = Mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestDto);
            //Note - here is the JSON convertion of partial update!!!
            patch.ApplyTo(piToPatch, ModelState);
            //validation on imputed model: JsonPatchDocument - 
            //so it passes any anotation attr on PointOfInterestForUpdateDto - e.g. "Required"
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (piToPatch.Description == piToPatch.Name)
                ModelState.AddModelError("title", "The provided description should be different from the name.");
            //another validation on any anotation attr of PointOfInterestForUpdateDto 
            TryValidateModel(piToPatch);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //2.save
            _pointsOfInterestSvc.PartiallyUpdatePointOfInterest(cityId, id, piToPatch);

            //204 - no content
            return NoContent();
        }

        /*
         https://localhost:44313/api/cities/1/pointsofinterest/14
          DELETE
         */
        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!_pointsOfInterestSvc.DeletePointOfInterest(cityId, id))
                return NotFound();

            //Return 204 no content.
            return NoContent();
        }

        #endregion

        #region async

        //https://localhost:44313/api/cities/async/1/pointsofinterest
        //https://cityinfoapi2.azurewebsites.net/api/cities/async/1/pointsofinterest
        [HttpGet("async/{cityId}/pointsofinterest")]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterestAsync(int cityId, CancellationToken cancellationToken)
        {
            var pointsOfInterestForCity = await _pointsOfInterestSvc.GetPointsOfInterestForCityAsync(cityId, cancellationToken);

            if (pointsOfInterestForCity == null)
                return NotFound();
            else
                return Ok(pointsOfInterestForCity);
        }

        //https://localhost:44313/api/cities/async/1/pointsofinterest/1
        //https://cityinfoapi2.azurewebsites.net/api/cities/async/1/pointsofinterest/1
        [HttpGet("async/{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterestLinkAsync")]
        public async Task<IActionResult> GetPointOfInterestAsync(int cityId, int id, CancellationToken cancellationToken)
        {
            var pointOfInterest = await _pointsOfInterestSvc.GetPointOfInterestForCityAsync(cityId, id, cancellationToken);

            if (pointOfInterest == null)
                return NotFound();
            else
                return Ok(pointOfInterest);
        }


        /*
        https://localhost:44313/api/cities/async/1/pointsofinterest
        https://cityinfoapi2.azurewebsites.net/api/cities/async/1/pointsofinterest
        POST request headers:
        Content-Type:application/json
        Accept: application/xml

        {
            "name": "Pere Lachaise",
            "description": "Famous cemetery where Jim Morrison and Oscar Wilde are buried."

         }
        */
        //get cityid from query string + deserialize from  body pointOfInterest
        [HttpPost("async/{cityId}/pointsofinterest")]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterestAsync(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest,
            CancellationToken cancellationToken)
        {
            //1.validate
            //if deserialization fails then return 404
            if (pointOfInterest == null)
                return BadRequest();
            //do custom check and set modelstate error
            if (pointOfInterest.Description == pointOfInterest.Name)
                ModelState.AddModelError("title", "The provided description should be different from the name.");
            //check modelstate and return 404 with error message set - custom or data annotation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //2.save
            //if City resource does not exist then return 404
            var pointOfInterestResult = await _pointsOfInterestSvc.AddPointOfInterestForCityAsync(cityId, pointOfInterest, cancellationToken);
            if (pointOfInterestResult == null)
                return NotFound();
            //3.create response
            //use this helper method returns 201 and location header with link to the newly resource
            //Also returns GetPointOfInterestLink and pass 2 params. 
            //Also pass newly created object in the response body
            return CreatedAtRoute("GetPointOfInterestLink",
                new { cityId = cityId, id = pointOfInterestResult.Id }, pointOfInterestResult);
        }

        /*
         * https://localhost:44313/api/cities/async/1/pointsofinterest/1
        PUT request headers:
        Content-Type:application/json
        Accept: application/xml

        {
            "name": "New York City- updated",
            "description": "The one with that big park."

         }
        */
        [HttpPut("async/{cityId}/pointsofinterest/{id}")]
        public async Task<IActionResult> UpdatePointOfInterestAsync(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest, CancellationToken cancellationToken)
        {
            //1.validate
            if (pointOfInterest == null)
                return BadRequest();
            if (pointOfInterest.Description == pointOfInterest.Name)
                ModelState.AddModelError("title", "The provided description should be different from the name.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //2.save
            if (await _pointsOfInterestSvc.UpdatePointOfInterestAsync(cityId, id, pointOfInterest, cancellationToken) == UpdatePoints.NOT_FOUND)
                return NotFound();

            return NoContent();
        }

        /*
        https://localhost:44313/api/cities/async/1/pointsofinterest/1
        PATCH request headers:
        Content-Type:application/json
        Accept: application/json

        Body:
        Req1 - OK
        [{ "op": "replace", "path": "/name", "value": "Town Hall" }]

        Req2 - Error
        [{ "op": "replace", "path": "/name", "value": "Town Hall" }, { "op": "remove",  "path": "/name" }]

        Response - Error
        { "Name": ["You should provide a name value."] }
        */
        //Same as full update but FromBody param specifies JsonPatchDocument<T>
        //where T is our DTO object that contains the partial data for update
        [HttpPatch("async/{cityId}/pointsofinterest/{id}")]
        public async Task<IActionResult> PartiallyUpdatePointOfInterestAsync(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patch, CancellationToken cancellationToken)
        {
            //1.validate
            if (patch == null)
                return BadRequest();

            var pointOfInterestDto = await _pointsOfInterestSvc.GetPointOfInterestForCityAsync(cityId, id, cancellationToken);
            if (pointOfInterestDto == null)
                return NotFound();

            //convert the DTO to partial DTO (same as the input param)
            //<Models.PointOfInterestDto, Models.PointOfInterestForUpdateDto>
            var piToPatch = Mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestDto);
            //Note - here is the JSON convertion of partial update!!!
            patch.ApplyTo(piToPatch, ModelState);
            //validation on imputed model: JsonPatchDocument - 
            //so it passes any anotation attr on PointOfInterestForUpdateDto - e.g. "Required"
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (piToPatch.Description == piToPatch.Name)
                ModelState.AddModelError("title", "The provided description should be different from the name.");
            //another validation on any anotation attr of PointOfInterestForUpdateDto 
            TryValidateModel(piToPatch);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //2.save
            await _pointsOfInterestSvc.PartiallyUpdatePointOfInterestAsync(cityId, id, piToPatch, cancellationToken);

            //204 - no content
            return NoContent();
        }

        /*
         https://localhost:44313/api/cities/async/1/pointsofinterest/14
          DELETE
         */
        [HttpDelete("async/{cityId}/pointsofinterest/{id}")]
        public async Task<IActionResult> DeletePointOfInterestAsync(int cityId, int id, CancellationToken cancellationToken)
        {
            if (!await _pointsOfInterestSvc.DeletePointOfInterestAsync(cityId, id, cancellationToken))
                return NotFound();

            //Return 204 no content.
            return NoContent();
        }

        #endregion
    }
}
