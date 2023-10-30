using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using static IdentityModel.OidcConstants;
using Timer.Models;
using Timer.Models.Requests;
using Timer.Models.Responses;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Timer.Models.Configuration;
using Microsoft.Extensions.Options;

namespace webapi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
	private readonly ILogger<AuthController> _logger;
	private readonly UserManager<User> _userManager;
	private readonly JwtConfig _jwtConfig;
	private readonly SignInManager<User> _signInManager;
	public AuthController(ILogger<AuthController> logger, UserManager<User> userManager, SignInManager<User> signInManager, IOptionsMonitor<JwtConfig> options)
	{
		_logger = logger;
		_userManager = userManager;
		_jwtConfig = options.CurrentValue;
		_signInManager = signInManager;

    }

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(new AuthResponse
			{
				Success = false,
				Error = "Bad request",
				Token = String.Empty
			});
		}
		var existingUser = await _userManager.FindByEmailAsync(user.Email);
		if (existingUser == null)
		{
			return BadRequest(new AuthResponse
			{
				Success = false,
				Error = "User not found",
				Token = String.Empty
			});
		}

		var isValid = await _userManager.CheckPasswordAsync(existingUser, user.Password);

		if (!isValid)
		{
			return BadRequest(new AuthResponse
			{
				Success = false,
				Error = "Wrong password",
				Token = String.Empty
			});
		}

		var jwtToken = GenerateJwtToken(existingUser);
		
		var res = await _signInManager.PasswordSignInAsync(existingUser.UserName, user.Password, false, lockoutOnFailure: false);
		if (!res.Succeeded)
		{
			return BadRequest();
		}
		return Ok(new AuthResponse
		{
			Success = true,
			Error = String.Empty,
			Token = jwtToken
		});
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] UserRegistrationRequest register)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(new AuthResponse
			{
				Success = false,
				Error = "Bad request",
				Token = String.Empty
			});
		}

		var existingUser = await _userManager.FindByEmailAsync(register.Email);
		if (existingUser != null)
		{
			return BadRequest(new AuthResponse
			{
				Success = false,
				Error = "user already exists",
				Token = String.Empty
			});
		}

		var newUser = new User
		{
			Email = register.Email,
			UserName = register.UserName,
			PhoneNumber = register.Phone,
		};

		var isCreated = await _userManager.CreateAsync(newUser, register.Password);

		if (!isCreated.Succeeded)
		{
			return BadRequest(new AuthResponse
			{
				Success = false,
				Error = String.Join(" ", isCreated.Errors.Select(x => x.Description).ToList()),
				Token = String.Empty
			});
		}

		//await _userManager.AddToRoleAsync(newUser, Constants.UserRole);

		var jwtToken = GenerateJwtToken(newUser);
		return Ok(new AuthResponse
		{
			Success = true,
			Error = String.Empty,
			Token = jwtToken
		});

	}

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    private string GenerateJwtToken(User user)
	{
		var jwtHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
		var jwtDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[]
			{
				new Claim("Id", user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			}),
			Expires = DateTime.Now.AddDays(7),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature)
		};
		var token = jwtHandler.CreateToken(jwtDescriptor);
		var jwtToken = jwtHandler.WriteToken(token);
		return jwtToken;
	}
}
