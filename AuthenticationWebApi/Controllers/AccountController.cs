using AuthenticationWebApi.Data;
using BCrypt.Net;
using JwtAuthenticationManager;
using JwtAuthenticationManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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
            //Minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character
            Regex rx = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
           
            if (!rx.IsMatch(request.Password)) return BadRequest("Password must be at least 8 characters long and contain both uppercase and lowercase letters. Must contain at least one number an special character.");
            
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
