using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SquaresApp.Application.IServices;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.ExtentionMethods;
using SquaresApp.Common.Models;
using SquaresApp.Common.SwaggerUtils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SquaresApp.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class PointsController : ControllerBase
    {

        private readonly IPointService _pointService;
        public PointsController(IPointService pointService)
        {
            _pointService = pointService;
        }

        /// <summary>
        /// Add new points to an existing list.
        /// </summary>
        /// <remarks>
        /// Return list of points along with their database Id on successful addition.
        /// </remarks>
        /// <param name="pointDTOs">It is a list of points. A point consists of X and Y fields. </param> 
        /// <returns>Returns success response with list of added points on successful addition and error response with error message when failed. </returns> 
        /// <response code="200">Successfully added</response>
        /// <response code="400">Validation failure</response> 
        [HttpPost("")]
        [ProducesResponseType(typeof(Response<IEnumerable<GetPointDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Import([FromBody] IEnumerable<PointDTO> pointDTOs)
        {

            if (pointDTOs is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = "Invalid payload." });
            }

            var userId = User.GetUserId();

            var result = await _pointService.AddAllPointsAsync(userId, pointDTOs);


            if (string.IsNullOrWhiteSpace(result.errorMessage))
            {
                return StatusCode(StatusCodes.Status200OK, new Response<IEnumerable<GetPointDTO>> { IsSuccess = true, Message = "Successfully imported.", Data = result.getPointDTOs });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = result.errorMessage });

        }

        /// <summary>
        /// Import new points from CSV file and add them to an existing list.
        /// </summary>
        /// <remarks>
        /// Return list of points along with their database Id on successful addition.
        /// </remarks> 
        /// <returns>Returns success response with list of added points on successful addition and error response with error message when failed. </returns> 
        /// <response code="200">Successfully added</response>
        /// <response code="400">Validation failure</response> 
        [HttpPost("Import")]
        [ProducesResponseType(typeof(Response<IEnumerable<GetPointDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [FileUploadOperation.FileContentType]
        public async Task<IActionResult> ImportFromCSV()
        {
            var pointDTOs = new List<PointDTO>();

            var file = Request?.Form?.Files[0];
            if (file?.Length > 0)
            {
                using var reader = new StreamReader(file.OpenReadStream());
                
                while (reader.Peek() >= 0)
                {
                    var csv = reader.ReadLine().Replace("\"", string.Empty).Split(",");

                    if (csv.Length == 2 && int.TryParse(csv[0], out var x) && int.TryParse(csv[1], out var y))
                    {
                        pointDTOs.Add(new PointDTO { X = x, Y = y });
                    }
                }

            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = "Invalid file." });
            }

            var userId = User.GetUserId();

            var result = await _pointService.AddAllPointsAsync(userId, pointDTOs);

            if (string.IsNullOrWhiteSpace(result.errorMessage))
            {
                return StatusCode(StatusCodes.Status200OK, new Response<IEnumerable<GetPointDTO>> { IsSuccess = true, Message = "Successfully imported.", Data = result.getPointDTOs });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = result.errorMessage });

        }

        /// <summary>
        /// Get all existing points.
        /// </summary>
        /// <remarks>
        /// Return list of all existing points.
        /// </remarks> 
        /// <response code="200">Success</response> 
        [HttpGet("")]
        [ProducesResponseType(typeof(Response<IEnumerable<GetPointDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var userId = User.GetUserId();

            var getPointDTOs = await _pointService.GetAllPointsAsync(userId);

            return StatusCode(StatusCodes.Status200OK, new Response<IEnumerable<GetPointDTO>> { IsSuccess = true, Message = "Successfully fetched.", Data = getPointDTOs });
        }

        /// <summary>
        /// Delete an existing point.
        /// </summary>
        /// <remarks>
        /// Provide a valid point id for successful deletion.
        /// </remarks>
        /// <param name="PointId">It is database id of an existing point. </param> 
        /// <returns>Returns success response on successful deletion and error response with error message when failed.</returns> 
        /// <response code="200">Successfully added</response>
        /// <response code="400">Validation failure</response> 
        [HttpDelete("{PointId}")]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] long PointId)
        {
            if (PointId <= 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = "Invalid point id." });
            }

            var userId = User.GetUserId();

            var result = await _pointService.DeletePointAsync(userId, PointId);


            if (string.IsNullOrWhiteSpace(result.errorMessage))
            {
                return StatusCode(StatusCodes.Status200OK, new Response<string> { IsSuccess = true, Message = "Successfully deleted." });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = result.errorMessage });

        }

    }
}
