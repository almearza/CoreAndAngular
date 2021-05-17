using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Entities;
namespace API.Controllers
{
   
    public class BuggyController : ApiBaseController
    {
        private readonly DContext _context;
        public BuggyController(DContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
         return await _context.Users.ToListAsync();   
        }
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret(){
         return "";   
        }
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound(){
         var user =  _context.Users.Find(-1);
         if(user==null)return NotFound();
         return user;
        }
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError(){
         return _context.Users.Find(-1).ToString();
        }
        [HttpGet("bad-request")]
        public ActionResult GetBadRequest(){
         return BadRequest("this was not request");
        }
    }
}