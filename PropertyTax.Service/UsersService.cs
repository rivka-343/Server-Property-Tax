﻿using PropertyTax.Core.Repositories;
using PropertyTax.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Umbraco.Core.Persistence.Repositories;
using PropertyTax.Core.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using PropertyTax.Core.DTO;
using PropertyTax.DTO;

namespace PropertyTax.Servise
{
    public class UsersService: IUsersService
    {
        
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UsersService(IUsersRepository usersRepository, IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _usersRepository.GetUsers();
        }
        public async Task<IEnumerable<ResidentDto>> GetResidents()
        {
            return await _usersRepository.GetResidents();
        }
        public async Task<User> GetUserById(int id)
        {
            return await _usersRepository.GetUserById(id);
        }

        public async Task<User> Register(User user, string password)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, password); // שימוש ב-user ו-password
            return await _usersRepository.AddUser(user);
        }

        public async Task<User> UpdateUser(int id, UserDto updatedUser)
        {
            User user = await GetUserById(id);

            // עדכון רק של שדות שנשלחו
            if (!string.IsNullOrEmpty(updatedUser.Username))
                user.Username = updatedUser.Username;

            if (!string.IsNullOrEmpty(updatedUser.Password))
                user.PasswordHash = updatedUser.Password;

            if (!string.IsNullOrEmpty(updatedUser.IdNumber))
                user.IdNumber = updatedUser.IdNumber;

            if (!string.IsNullOrEmpty(updatedUser.Role))
                user.Role = updatedUser.Role;

            return await _usersRepository.UpdateUser(user);
        }

        public async Task<bool> DeleteUser(int id)
        {
            return await _usersRepository.DeleteUser(id);
        }

        public async Task<User> GetUserByUsername(string name)
        {
            return await _usersRepository.GetUserByUsername(name);
        }

       
    }
}
