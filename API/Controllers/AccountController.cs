using System.Threading.Tasks;
using API.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.Dtos;
using API.Interfaces;
using System.Linq;
using AutoMapper;

namespace API.Controllers
{
    public class AccountController : ApiBaseController
    {
        private readonly DContext _context;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public AccountController(DContext context, ITokenService tokenService,
        IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
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
            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownUs=user.KnownUs,
                Gender=user.Gender
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByUserNameAsync(loginDto.UserName.ToLower());
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
                PhotoUrl = user.Photos?.FirstOrDefault(Photo => Photo.IsMain)?.Url,
                KnownUs=user.KnownUs,
                Gender=user.Gender
            };

        }
        private async Task<bool> IsRegistered(string username)
        {
            return await _context.AppUsers.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}