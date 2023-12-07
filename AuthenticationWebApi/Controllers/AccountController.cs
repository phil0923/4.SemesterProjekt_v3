using AuthenticationWebApi.Data;
using BCrypt.Net;
using JwtAuthenticationManager;
using JwtAuthenticationManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly AuthDbContext _context;

        public AccountController(JwtTokenHandler jwtTokenHandler, AuthDbContext context)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _context = context;
        }

        [HttpPost]
        public ActionResult<AuthenticationResponse?> Authenticate([FromBody] AuthenticationRequest authenticationRequest)
        {
            var user = _context.Users.Where(x => x.Username == authenticationRequest.Username).First();

            if (user is null) return BadRequest("User not found");

            if (!BCrypt.Net.BCrypt.Verify(authenticationRequest.Password, user.PasswordHash)) return BadRequest("User not found");

            var authenticationResponse = _jwtTokenHandler.GenerateJwtToken(authenticationRequest);
            if (authenticationResponse is null) return Unauthorized();
            return authenticationResponse;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Regiser(UserAccountDto request)
        {
            var user = new UserAccount();
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }
    }
}
