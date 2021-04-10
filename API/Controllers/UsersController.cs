using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
   
    public class UsersController : ApiBaseController
    {
        private readonly DContext _context;
        public UsersController(DContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
         return await _context.AppUsers.ToListAsync();   
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id){
         return await _context.AppUsers.FindAsync(id);   
        }
    }
}