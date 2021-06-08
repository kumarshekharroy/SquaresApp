using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SquaresApp.Application.IServices;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.ExtentionMethods;
using SquaresApp.Common.Models;
using System.Collections.Generic;
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
        /// Return A,B,C,D vertices of all the identified squares.
        /// </remarks> 
        /// <response code="200">Success</response> 
        /// <response code="401">Unauthorized request</response> 
        /// <response code="500">Unexpected error</response> 
        [HttpGet("")]
        [ProducesResponseType(typeof(Response<IEnumerable<SquareDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            var userId = User.GetUserId();

            var squareDTOs = await _squaresService.GetAllSquares(userId);

            return StatusCode(StatusCodes.Status200OK, new Response<IEnumerable<SquareDTO>> { IsSuccess = true, Message = "Successfully identified.", Data = squareDTOs });
        }
    }
}
