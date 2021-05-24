using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : ApiBaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly DContext _context;
        private readonly IPhotoService _photoService;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        public UsersController(IUnitOfWork unitOfWork,
        IMapper mapper, DContext context, IPhotoService photoService,
        UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
            this._photoService = photoService;
            _context = context;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserPrams userPrams)
        {
            var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
            userPrams.CurrentUsername = User.GetUsername();
            if (string.IsNullOrEmpty(userPrams.Gender))
                userPrams.Gender = gender;

            var users = await _unitOfWork.UserRepository.GetMembersAsync(userPrams);
            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            Response.AddPaginationHeader(users.CurrentPage, users.TotalPages, users.TotalCount, users.PageSize);
            return Ok(users);
        }
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _unitOfWork.UserRepository.GetMemberByUserNameAsync(username);
            // var userToReturn = _mapper.Map<MemberDto>(user);
            return user;
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            _mapper.Map(memberUpdateDto, user);
            _unitOfWork.UserRepository.Update(user);
            if (await _unitOfWork.Complete()) return NoContent();
            return BadRequest("error while updating user");
        }
        [AllowAnonymous]
        [HttpPost("Seed")]
        public async Task<ActionResult<List<AppUser>>> Seed(List<AppUser> users)
        {
            var roles = new List<AppRole>(){
                new AppRole{Name="Member"},
                new AppRole{Name="Admin"},
                new AppRole{Name="Moderator"}
        };
            foreach (var role in roles)
            {
                await _roleManager.CreateAsync(role);
            }
            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                await _userManager.CreateAsync(user, "P@$$w0rd");
                await _userManager.AddToRoleAsync(user, "Member");
            }
            var admin = new AppUser
            {
                UserName = "admin"
            };
            await _userManager.CreateAsync(admin, "P@$$w0rd");
            await _userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
            return users;
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> UploadPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());
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
            if (await _unitOfWork.Complete())
                // return _mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            return BadRequest("something went wring while saving photo");
        }
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);
            if (photo.IsMain) return BadRequest("photo is already main photo");
            var currentMain = user.Photos.FirstOrDefault(photo => photo.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            var result = await _unitOfWork.Complete();
            if (result) return NoContent();
            return BadRequest("something went wrong");
        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);
            if (photo == null) return BadRequest("no photo found with this id");
            if (photo.IsMain) return BadRequest("you can not delete main photo");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await _unitOfWork.Complete())
                return Ok();
            return BadRequest("something went wrong");
        }

    }
}