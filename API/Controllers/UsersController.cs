using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository, IMapper mapper, DContext context, IPhotoService photoService)
        {
            this._photoService = photoService;
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
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _userRepository.GetMemberByUserNameAsync(username);
            // var userToReturn = _mapper.Map<MemberDto>(user);
            return user;
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUserame();
            var user = await _userRepository.GetUserByUserNameAsync(username);
            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("error while updating user");
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
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> UploadPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserame());
            var result = await _photoService.UploadPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0)
                photo.IsMain = true;

            user.Photos.Add(photo);
            if (await _userRepository.SaveAllAsync())
                // return _mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            return BadRequest("something went wring while saving photo");
        }
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserame());
            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);
            if (photo.IsMain) return BadRequest("photo is already main photo");
            var currentMain = user.Photos.FirstOrDefault(photo => photo.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            var result = await _userRepository.SaveAllAsync();
            if (result) return NoContent();
            return BadRequest("something went wrong");
        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId){
            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserame());
             var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);
             if(photo==null) return BadRequest("no photo found with this id");
            if (photo.IsMain) return BadRequest("you can not delete main photo");
            if(photo.PublicId!=null){
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error!=null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if(await _userRepository.SaveAllAsync())
            return Ok();
             return BadRequest("something went wrong");
        }

    }
}