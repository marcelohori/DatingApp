﻿using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace API.Controllers
{
    public class AccountController(DataContext context, ITokenService itokenService, IMapper mapper) : BaseApiController
    {
        [HttpPost("register")] //account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.Username)) return BadRequest("Usuário já cadastrado");

            using var hmac = new HMACSHA512();
            var user = mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;
            
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = itokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

            
        }

        [HttpPost("login")]        
            public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
            {
                var user = await context.Users
                .Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());
                
                if(user == null) return Unauthorized("Usuário ou senha inválido");
                
                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

                for(int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Senha inválida");
                }

                return new UserDto
                {
                    Username = user.UserName,
                    KnownAs = user.KnownAs,
                    Token = itokenService.CreateToken(user),
                    Gender = user.Gender,
                    PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
                };
            }
        private async Task<bool> UserExists(string username)
        {
            return  await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }
    }
}