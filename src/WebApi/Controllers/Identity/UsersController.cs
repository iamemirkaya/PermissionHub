using Application.Features.Identity.Users.Commands;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
