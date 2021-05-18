using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            this._userManager = userManager;

        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
            .Include(u => u.UserRoles)
            .ThenInclude(u => u.Role)
            .Select(u => new
            {
                u.Id,
                username = u.UserName,
                roles = u.UserRoles.Select(u => u.Role.Name).ToList()
            })
            .ToListAsync();
            return Ok(users);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound("there is new user with this username");
            var selectedRoles = roles.Split(',').ToArray();
            var userRoles = await _userManager.GetRolesAsync(user);
            var addingResult = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!addingResult.Succeeded)
                return BadRequest("unable to add roles");
            var removingOldRoles = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!removingOldRoles.Succeeded)
                return BadRequest("error while editing roles");
            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosToModerate()
        {
            return Ok("only moderate and admin can access this");
        }
    }
}