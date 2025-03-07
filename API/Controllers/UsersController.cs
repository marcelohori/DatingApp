using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using API.Data; // Certifique-se de importar o DataContext
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using API.DTOs;
using System.Security.Claims;

namespace API.Controllers // Adicione o namespace correto
{
  [Authorize]
    public class UsersController (IUserRepository userRepository, IMapper mapper): BaseApiController
    {
        
        [HttpGet]
        public async Task <ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await userRepository.GetMembersAsync(); 
            
            return Ok(users);
        }
        [Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username) 
        {
            var user = await userRepository.GetMemberAsync(username);

            if (user == null) return NotFound();

            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(username==null) return BadRequest("No username found in token");

            var user = await userRepository.GetUserByUserNameAsync(username);
            if(user==null) return BadRequest("Could not find user");
            
            mapper.Map(memberUpdateDto,user);
            if(await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update the user");
        }
    }
}
