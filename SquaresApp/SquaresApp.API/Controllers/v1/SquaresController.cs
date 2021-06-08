﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.ExtentionMethods;
using SquaresApp.Common.Models;
using SquaresApp.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquaresApp.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class SquaresController : ControllerBase
    {
        private readonly ISquaresService _squaresService; 
        public SquaresController(ISquaresService squaresService)
        { 
            _squaresService = squaresService; 
        }

        /// <summary>
        /// Get all squares.
        /// </summary>
        /// <remarks>
        /// Return A,B,C,D coordinates of all the identified squares.
        /// </remarks> 
        /// <response code="200">Success</response> 
        [HttpGet("")]
        [ProducesResponseType(typeof(Response<IEnumerable<SquareDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var userId = User.GetUserId(); 

            var squareDTOs = await _squaresService.GetAllSquares(userId);

            return StatusCode(StatusCodes.Status200OK, new Response<IEnumerable<SquareDTO>> { IsSuccess = true, Message = "Successfully identified.", Data = squareDTOs });
        }
    }
}