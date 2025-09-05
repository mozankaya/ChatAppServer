using ChatAppServer.WebAPI.Data;
using ChatAppServer.WebAPI.DTOs;
using ChatAppServer.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class ChatsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ChatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetChats(Guid userId, Guid toUserId, CancellationToken cancellationToken)
        {
            List<Chat> chats = await _context.Chats.Where(p =>
            (p.UserId == userId && p.ToUserId == toUserId) ||
            (p.UserId == toUserId && p.ToUserId == userId))
            .OrderBy(p => p.Date)
            .ToListAsync(cancellationToken);
            

            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageDTO request ,CancellationToken cancellationToken)
        {
            Chat chat = new()
            {
                UserId = request.UserId,
                ToUserId = request.ToUserId,
                Message = request.Message,
                Date = DateTime.Now
            };

            await _context.Chats.AddAsync(chat,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(chat);
        }
    }
}
