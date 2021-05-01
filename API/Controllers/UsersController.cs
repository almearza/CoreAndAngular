using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
[Authorize]
    public class UsersController : ApiBaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly DContext _context;
        public UsersController(IUserRepository userRepository, IMapper mapper, DContext context)
        {
            _context = context;
            this._mapper = mapper;
            this._userRepository = userRepository;

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(users);
        }
        // [Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _userRepository.GetMemberByUserNameAsync(username);
            // var userToReturn = _mapper.Map<MemberDto>(user);
            return user;
        }
        // [HttpPost("Seed")]
        // public async Task<ActionResult<List<AppUser>>> Seed(List<AppUser> users)
        // {
        //     foreach (var user in users)
        //     {
        //         var hmac = new HMACSHA512();
        //         user.UserName=user.UserName.ToLower();
        //         user.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes("P@$$w0rd"));
        //         user.PasswordSalt=hmac.Key;
        //         _context.AppUsers.Add(user);
        //     }
        //     await _context.SaveChangesAsync();
        //     return users;
        // }


    }
}