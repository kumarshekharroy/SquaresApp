using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SquaresApp.Application.IServices;
using SquaresApp.Common.Constants;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.Helpers;
using SquaresApp.Common.Models;
using System.Threading.Tasks;

namespace SquaresApp.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, AppSettings appSettings, IMapper mapper)
        {
            _userService = userService;
            _appSettings = appSettings;
            _mapper = mapper;
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <remarks>
        /// Provide unique username for successful registration.
        /// </remarks>
        /// <param name="userDTO">It is an object consists of username and password fields. </param> 
        /// <returns>Returns HttpResponse with registered user's detail on successful registration and error message when failed. </returns> 
        /// <response code="200">Registered</response>
        /// <response code="400">Validation failure</response> 
        [HttpPost("Register")]
        [ProducesResponseType(typeof(Response<GetUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {

            if (userDTO is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = "Invalid registration payload." });
            }


            var result = await _userService.AddUserAsync(userDTO);


            if (string.IsNullOrWhiteSpace(result.errorMessage))
            {
                return StatusCode(StatusCodes.Status200OK, new Response<GetUserDTO> { IsSuccess = true, Message = "Registration successful.", Data = result.getUserDTO });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = result.errorMessage });

        }

        /// <summary>
        /// Get JWT token
        /// </summary>
        /// <remarks>
        /// This endpoint returns a JWT token on successful authentication which can be used to access other secure endpoints.
        /// </remarks>
        /// <param name="userDTO">It is an object consists of username,password. </param> 
        /// <returns>Returns HttpResponse with a valid JWT on successful authentication and error message when failed. </returns> 
        /// <response code="200">Authentication Success</response>
        /// <response code="400">Validation failure</response> 
        /// <response code="500">Unexpected error</response> 
        [HttpPost("Login")]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {

            if (userDTO is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = "Invalid authentication payload." });
            }

            var result = await _userService.GetUserAsync(userDTO);

            if (!string.IsNullOrWhiteSpace(result.errorMessage))
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response<string> { Message = result.errorMessage });
            }

            if (_appSettings is null || _appSettings.JWTConfig is null || string.IsNullOrWhiteSpace(_appSettings.JWTConfig.Secret))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response<string> { Message = ConstantValues.UnexpectedErrorMessage });
            }

            var jwtToken = JWTHelper.GenerateJWTToken(result.getUserDTO, _appSettings.JWTConfig);

            return StatusCode(StatusCodes.Status200OK, new Response<string> { IsSuccess = true, Message = "Authentication successful.", Data = jwtToken });

        }



    }
}
