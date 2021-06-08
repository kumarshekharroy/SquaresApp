using AutoMapper;
using SquaresApp.Application.IServices;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.ExtentionMethods;
using SquaresApp.Data.IRepositories;
using SquaresApp.Data.Models;
using System.Threading.Tasks;

namespace SquaresApp.Application.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// create / add  a new user
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        public async Task<(GetUserDTO getUserDTO, string errorMessage)> AddUserAsync(UserDTO userDTO)
        {

            var errorMessage = ValidateUserDTO(userDTO);

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                return (getUserDTO: default, errorMessage);
            }

            var user = _mapper.Map<User>(userDTO);

            user.Password = user.Password.ToSHA256();

            var result = await _userRepository.AddUserAsync(user);

            var getUserDTO = string.IsNullOrWhiteSpace(result.errorMessage) ? _mapper.Map<GetUserDTO>(result.user) : default;

            return (getUserDTO: getUserDTO, errorMessage: result.errorMessage);
        }

        /// <summary>
        /// get user detail by username and password
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        public async Task<(GetUserDTO getUserDTO, string errorMessage)> GetUserAsync(UserDTO userDTO)
        {
            var errorMessage = ValidateUserDTO(userDTO);

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                return (getUserDTO: default, errorMessage);
            }

            userDTO.Password = userDTO.Password.ToSHA256();

            var result = await _userRepository.GetUserAsync(userDTO.Username, userDTO.Password);

            var getUserDTO = string.IsNullOrWhiteSpace(result.errorMessage) ? _mapper.Map<GetUserDTO>(result.user) : default;

            return (getUserDTO: getUserDTO, errorMessage: result.errorMessage);

        }


        private static string ValidateUserDTO(UserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.Username))
            {
                return "Invalid username.";
            }

            if (string.IsNullOrWhiteSpace(userDTO.Password))
            {
                return "Invalid password.";
            }

            return string.Empty;
        }
    }

}
