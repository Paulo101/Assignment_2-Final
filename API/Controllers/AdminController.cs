using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(UserManager<AppUser> userManager, IPhotoRepository photoRepository,
        IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound("Could not find user");

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins or moderators can see this");
        }
        [HttpGet("photos-for-approval/all")]
        public ActionResult GetPhotosForApproval()
        {
            return Ok(_unitOfWork.PhotoRepository.GetUnapprovedPhotos());
        }
        [Authorize(Policy = "ModeratePhotoRole,AdminPhotoRole")]
        [HttpPut("approve-photo/{id}")]
        public async Task<ActionResult> ApprovePhoto(PhotoForApprovalDto photoForApproval)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            user.Photos.SingleOrDefault(s => s.Id == photoForApproval.Id).isApproved = true;
            bool hasMain = false;
            foreach (var photo in user.Photos)
            {
                if (photo.IsMain)
                    hasMain = true;
            }
            if (!hasMain)
                user.Photos.SingleOrDefault(s => s.Id == photoForApproval.Id).IsMain = true;
            if (await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Failed to Approve Photo");
        }
        [Authorize(Policy = "ModeratePhotoRole,AdminPhotoRole")]
        [HttpPut("reject-photo/{id}")]
        public async Task<ActionResult> RejectPhoto(PhotoForApprovalDto photoForApproval)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            user.Photos.SingleOrDefault(s => s.Id == photoForApproval.Id).isApproved = false;
            if (await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Failed to Reject Photo");
        }
    }
}