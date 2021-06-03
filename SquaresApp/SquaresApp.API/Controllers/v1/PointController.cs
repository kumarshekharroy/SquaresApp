﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SquaresApp.Common.Constants;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.ExtentionMethods;
using SquaresApp.Common.Helpers;
using SquaresApp.Common.Models;
using SquaresApp.Domain.Models;
using SquaresApp.Infra.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquaresApp.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class PointController : ControllerBase
    {

        private readonly IPointService _pointService; 
        public PointController(IPointService pointService)
        {
            _pointService = pointService; 
        }


        /// <summary>
        /// Add a new point.
        /// </summary>
        /// <remarks>
        /// Provide unique coordinates for successful addition.
        /// </remarks>
        /// <param name="pointDTO">It is an object consists of X and Y fields. </param> 
        /// <returns>Returns HttpResponse with added point's detail on successful addition and error message when failed. </returns> 
        /// <response code="200">Successfully added</response>
        /// <response code="400">Validation failure</response> 
        [HttpPost("")]
        [ProducesResponseType(typeof(Response<GetPointDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] PointDTO pointDTO)
        {
            if (pointDTO is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = "Invalid point payload." });
            }

            var userId = User.GetUserId();

            var result = await _pointService.AddPointAsync(userId, pointDTO);


            if (string.IsNullOrWhiteSpace(result.errorMessage))
            {
                return StatusCode(StatusCodes.Status200OK, new Response<GetPointDTO> { IsSuccess = true, Message = "Successfully added.", Data = result.getPointDTO });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = result.errorMessage });

        }


        /// <summary>
        /// Import new points.
        /// </summary>
        /// <remarks>
        /// Provide unique coordinates for successful addition.
        /// </remarks>
        /// <param name="pointDTOs">It is a list of object consists of X and Y fields. </param> 
        /// <returns>Returns HttpResponse with added points list on successful addition and error message when failed. </returns> 
        /// <response code="200">Successfully added</response>
        /// <response code="400">Validation failure</response> 
        [HttpPost("Import")]
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
        /// Get all points.
        /// </summary>
        /// <remarks>
        /// Provide unique coordinates for successful addition.
        /// </remarks>
        /// <param name="pointDTOs">It is a list of object consists of X and Y fields. </param> 
        /// <returns>Returns HttpResponse with added points list on successful addition and error message when failed. </returns> 
        /// <response code="200">Successfully added</response>
        /// <response code="400">Validation failure</response> 
        [HttpGet("")]
        [ProducesResponseType(typeof(Response<IEnumerable<GetPointDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        { 
            var userId = User.GetUserId();

            var getPointDTOs = await _pointService.GetAllPointsAsync(userId);

            return StatusCode(StatusCodes.Status200OK, new Response<IEnumerable<GetPointDTO>> { IsSuccess = true, Message = "Successfully fetched.", Data = getPointDTOs });
        }


        /// <summary>
        /// Delete a new point.
        /// </summary>
        /// <remarks>
        /// Provide a valid point id for successful deletion.
        /// </remarks>
        /// <param name="pointId">It is id of an existing point. </param> 
        /// <returns>Returns HttpResponse with success response on successful addition and error response when failed. </returns> 
        /// <response code="200">Successfully added</response>
        /// <response code="400">Validation failure</response> 
        [HttpDelete("{pointId}")]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] long pointId)
        {
            if (pointId <= 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = "Invalid point id." });
            }

            var userId = User.GetUserId();

            var result = await _pointService.DeletePointAsync(userId, pointId);


            if (string.IsNullOrWhiteSpace(result.errorMessage))
            {
                return StatusCode(StatusCodes.Status200OK, new Response<string> { IsSuccess = true, Message = "Successfully deleted." });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = result.errorMessage });

        }

    }
}