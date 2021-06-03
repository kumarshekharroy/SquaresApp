﻿using AutoMapper;
using SquaresApp.Common.DTOs;
using SquaresApp.Common.ExtentionMethods;
using SquaresApp.Domain.IRepositories;
using SquaresApp.Domain.Models;
using SquaresApp.Infra.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SquaresApp.Infra.Services
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

            if (string.IsNullOrWhiteSpace(userDTO.Username))
            {
                return (getUserDTO: default, errorMessage: "Invalid username.");
            }

            if (string.IsNullOrWhiteSpace(userDTO.Password))
            {
                return (getUserDTO: default, errorMessage: "Invalid password.");
            }

            var user = _mapper.Map<User>(userDTO);

            user.Password = user.Password.ToSHA256();

            var result = await _userRepository.AddUserAsync(user);

            var getUserDTO = string.IsNullOrWhiteSpace(result.errorMessage) ? _mapper.Map<GetUserDTO>(result.user) : default;

            return (getUserDTO: getUserDTO, errorMessage: result.errorMessage); 
        }

        /// <summary>
        /// check existance of a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<bool> CheckUserExistanceAsync(string username)
        {
            return await _userRepository.CheckUserExistanceAsync(username);
        }


        /// <summary>
        /// get user detail by username and password
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        public async Task<(GetUserDTO getUserDTO, string errorMessage)> GetUserAsync(UserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.Username))
            {
                return (getUserDTO: default, errorMessage: "Invalid username.");
            }

            if (string.IsNullOrWhiteSpace(userDTO.Password))
            {
                return (getUserDTO: default, errorMessage: "Invalid password.");
            }

            userDTO.Password = userDTO.Password.ToSHA256();

            var result = await _userRepository.GetUserAsync(userDTO.Username, userDTO.Password);

            var getUserDTO = string.IsNullOrWhiteSpace(result.errorMessage) ? _mapper.Map<GetUserDTO>(result.user) : default;

            return (getUserDTO: getUserDTO, errorMessage: result.errorMessage);

        }


    }

}