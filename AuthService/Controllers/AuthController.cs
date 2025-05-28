using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try 
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Content($"Error: {ex.Message}", "text/plain");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}", "text/plain");
            }
        }

        [HttpGet("validate")]
        [Authorize]
        public IActionResult Validate()
        {
            try
            {
                return Ok(new { status = "Valid", user = User.Identity.Name });
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}", "text/plain");
            }
        }
    }
}
