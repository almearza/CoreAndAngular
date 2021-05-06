using System.Threading.Tasks;
using API.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.Dtos;
using API.Interfaces;
using System.Linq;

namespace API.Controllers
{
    public class AccountController : ApiBaseController
    {
        private readonly DContext _context;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        public AccountController(DContext context, ITokenService tokenService, IUserRepository userRepository)
        {
            this._userRepository = userRepository;
            _tokenService = tokenService;
            _context = context;

        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await IsRegistered(registerDto.Username.ToLower()))
                return BadRequest("Username already exsit !");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user =await _userRepository.GetUserByUserNameAsync(loginDto.UserName.ToLower());
            if (user == null) return Unauthorized("invalid username");
            var hmac = new HMACSHA512(user.PasswordSalt);
            var loginHashPass = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            //campare the two passwords:
            for (var i = 0; i < loginHashPass.Length; i++)
            {
                if (user.PasswordHash[i] != loginHashPass[i])
                    return Unauthorized("invalid password !");
            }
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos?.FirstOrDefault(Photo => Photo.IsMain)?.Url
            };

        }
        private async Task<bool> IsRegistered(string username)
        {
            return await _context.AppUsers.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}