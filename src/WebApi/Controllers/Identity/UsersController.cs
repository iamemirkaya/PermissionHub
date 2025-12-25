using Application.Features.Identity.Users.Commands;
using Application.Features.Identity.Users.Queries;
using Common.Authorization;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    public class UsersController : MyBaseController<UsersController>
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest userRegistration)
        {
            var response = await MediatorSender
                .Send(new UserRegistrationCommand { UserRegistration = userRegistration });

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("{userId}")]
        [MustHavePermission(AppFeature.Users, AppAction.Read)]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var response = await MediatorSender.Send(new GetUserByIdQuery { UserId = userId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut]
        [MustHavePermission(AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> UpdateUserDetails([FromBody] UpdateUserRequest updateUser)
        {
            var response = await MediatorSender.Send(new UpdateUserCommand { UpdateUser = updateUser });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("change-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordRequest changePassword)
        {
            var response = await MediatorSender
                .Send(new ChangeUserPasswordCommand { ChangePassword = changePassword });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPut("change-status")]
        [MustHavePermission(AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> ChangeUserStatus([FromBody] ChangeUserStatusRequest changeUserStatus)
        {
            var response = await MediatorSender
                .Send(new ChangeUserStatusCommand { ChangeUserStatus = changeUserStatus });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("roles/{userId}")]
        [MustHavePermission(AppFeature.Roles, AppAction.Read)]
        public async Task<IActionResult> GetRoles(string userId)
        {
            var response = await MediatorSender.Send(new GetRolesQuery { UserId = userId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
    }
}
