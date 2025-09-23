using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Domain.Entities;

namespace TravelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = new User
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    // Assign role (default to Customer if not specified)
                    var role = string.IsNullOrEmpty(request.Role) ? "Customer" : request.Role;
                    await _userManager.AddToRoleAsync(user, role);

                    // Sign in the user automatically after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    _logger.LogInformation("User created a new account with password.");

                    return Ok(new
                    {
                        message = "Registration successful",
                        user = new
                        {
                            user.Id,
                            user.FirstName,
                            user.LastName,
                            user.Email,
                            Role = role
                        }
                    });
                }

                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return BadRequest(new { message = "An error occurred during registration" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    request.Email,
                    request.Password,
                    request.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // Get the logged-in user info
                    var user = await _userManager.FindByEmailAsync(request.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    return Ok(new
                    {
                        message = "Login successful",
                        user = new
                        {
                            user.Id,
                            user.FirstName,
                            user.LastName,
                            user.Email,
                            Roles = roles
                        }
                    });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return BadRequest(new { message = "Account locked out" });
                }
                else
                {
                    _logger.LogWarning("Invalid login attempt.");
                    return Unauthorized(new { message = "Invalid login attempt" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return BadRequest(new { message = "An error occurred during login" });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");
                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return BadRequest(new { message = "An error occurred during logout" });
            }
        }

        [HttpGet("userinfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    Roles = roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user info");
                return BadRequest(new { message = "An error occurred" });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var result = await _userManager.ChangePasswordAsync(
                    user, request.CurrentPassword, request.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User changed their password successfully.");
                    return Ok(new { message = "Password changed successfully" });
                }

                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return BadRequest(new { message = "An error occurred" });
            }
        }
    }

    public class RegisterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "Customer";
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; } = false;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}

