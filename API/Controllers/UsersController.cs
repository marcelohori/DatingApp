using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using API.Data; // Certifique-se de importar o DataContext
using API.Entities;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers // Adicione o namespace correto
{
  
    public class UsersController (DataContext context): BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task <ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await context.Users.ToListAsync(); // Use o contexto armazenado
            return users;
        }
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AppUser>> GetUser(int id) // Renomeado para evitar conflito
        {
            var user = await context.Users.FindAsync(id);

            if (user == null) return NotFound();

            return user;
        }
    }
}
