using System.ComponentModel.DataAnnotations;

namespace Timer.Models.Requests;

public class UserRegistrationRequest
{
	[Required] 
	public string UserName { get; set; }
	[Required]
	public string Phone { get; set; }
	[Required][EmailAddress]
	public string Email { get; set; }
	[Required]
	public string Password { get; set; }
}
