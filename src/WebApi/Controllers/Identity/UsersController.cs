using Application.Features.Identity.Users.Commands;
using Application.Features.Identity.Users.Queries;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
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

        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginCommandRequest request)
        {
            var response = await MediatorSender.Send(request);
            return Ok(response);
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

        [HttpGet]
        [MustHavePermission(AppFeature.Users, AppAction.Read)]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await MediatorSender.Send(new GetAllUsersQuery());
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


        [HttpPut("user-roles")]
        [MustHavePermission(AppFeature.Users, AppAction.Update)]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesRequest updateUserRoles)
        {
            var response = await MediatorSender
                .Send(new UpdateUserRolesCommand { UpdateUserRoles = updateUserRoles });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous] 
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code)
        {

            var response = await MediatorSender.Send(new ConfirmEmailCommand { UserId = userId, Code = code });

            return BadRequest(response);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(await ResponseWrapper.FailAsync("E-posta adresi gereklidir."));
            }

            var origin = GetOriginFromRequest();

            var command = new ForgotPasswordCommand
            {
                Email = request.Email,
                Origin = origin
            };

            var response = await MediatorSender.Send(command);

            return Ok(response);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var command = new ResetPasswordCommand
            {
                Email = request.Email,
                Password = request.Password,
                Token = request.Token,
                ConfirmPassword = request.ConfirmPassword
            };

            var response = await MediatorSender.Send(command);

            return Ok(response);
        }


        private string GetOriginFromRequest()
        {
            if (Request.Headers.ContainsKey("origin"))
            {
                return Request.Headers["origin"].ToString();
            }
            return $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
        }
    }
}
