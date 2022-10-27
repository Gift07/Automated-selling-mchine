using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using server.Dtos;
using server.Models;
using server.Common;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        public readonly DataContext _context;

        public AuthController(DataContext context)
        {
            _context = context;
        }
        [HttpPost("sign-in")]
        public async Task<ActionResult<Auth>> SignIn(Auth user)
        {
            try
            {
                var ExistingUser = await _context.Auth.FirstOrDefaultAsync(x => x.Email == user.Email);

                if (ExistingUser == null) return NotFound();
                if (Utils.Verify(user.Password, ExistingUser.Password))
                {
                    var access = Utils.GenerateJWTToken(ExistingUser.Id, true);

                    return Ok(access);
                }
                return BadRequest("passwords is wrong");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("sign-up")]
        public async Task<ActionResult<Auth>> SignUp(Auth user)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                string hashedPassword;
                Console.WriteLine(user);
                var ExistingUser = await _context.Auth
                .FirstOrDefaultAsync(
                    x => x.Email == user.Email
                    );
                if (ExistingUser != null) return Conflict();

                if (!string.IsNullOrEmpty(user.Password))
                {
                    hashedPassword = Utils.Encrypt(user.Password);
                    Console.WriteLine(hashedPassword);
                }
                else
                {
                    return BadRequest("Something went wrong");
                }

                Auth auth = new Auth()
                {
                    Id = Guid.NewGuid(),
                    CreateAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                auth.Email = user.Email;
                auth.Password = hashedPassword;
                auth.Username = user.Username;

                await _context.Auth.AddAsync(auth);
                await _context.SaveChangesAsync();

                return Ok(auth);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}