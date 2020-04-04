using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pickup.Core.Models.V1.Request.Identity;
using Pickup.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Pickup.Api.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager)
        {
            this._userManager = userManager;
        }

        [HttpGet]
        [Route("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]ConfirmEmailRequest model)
        {

            if (model.UserId == null || model.Code == null)
            {
                return BadRequest(new string[] { "Error retrieving information!" });
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return BadRequest(new string[] { "Could not find user!" });
            if (user.EmailConfirmed == true)
                return BadRequest(new string[] { "Email has already been confirmed." });

            var result = await _userManager.ConfirmEmailAsync(user, model.Code);
            if (result.Succeeded)
                return View(result);

            return BadRequest(result.Errors.Select(x => x.Description));
        }
    }
}