using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Timer.Models;

namespace Timer.Controllers

{
    [ApiController]
    public class ContentController : Controller
    {

        private readonly SiteContext _siteContext;
        private readonly UserManager<User> _userManager;

        public ContentController(SiteContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _siteContext = context;
        }

        [HttpGet("content/index")]
		public IActionResult Index()
        {
            return PartialView();
        }

		[HttpGet("content/register")]
		public IActionResult Register()
		{
			return PartialView();
		}

        [Authorize]
        [HttpGet("content/auth-navbar")]
        public async Task<IActionResult> AuthNavbar()
        {
           var user = _siteContext.Users.First(x => x.Id == int.Parse(User.FindFirst("Id").Value));
            return PartialView("~/Views/Content/NavBarAuthorize.cshtml", user);
        }

        [HttpGet("content/navbar")]
        public async Task<IActionResult> Navbar()
        {      
            return PartialView("~/Views/Content/NavBar.cshtml");
        }
    }
}
