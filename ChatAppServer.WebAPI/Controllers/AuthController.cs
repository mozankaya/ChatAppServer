using ChatAppServer.WebAPI.Data;
using ChatAppServer.WebAPI.DTOs;
using ChatAppServer.WebAPI.Models;
using GenericFileService.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class AuthController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }




        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterDTO request, CancellationToken cancellationToken)
        {
            bool isNameExist = await _context.Users.AnyAsync(p => p.Name == request.Name, cancellationToken);

            if (isNameExist)
            {
                return BadRequest(new { Message = "This username has been used before!" });
            }

            string avatar = FileService.FileSaveToServer(request.File, "wwwroot/avatar/");
            User user = new()
            {
                Name = request.Name,
                Avatar = avatar
            };
            await _context.Users.AddAsync(user,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken); 


            return Ok(user);

        }

        [HttpGet]
        public async Task<IActionResult> Login(string name, CancellationToken cancellationToken)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
            if(user is null)
            {
                return BadRequest(new {Message ="User is not found."});
            }

            user.Status = "online";

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(user);
        }
    }
}
