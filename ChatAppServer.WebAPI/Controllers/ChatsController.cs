using ChatAppServer.WebAPI.Data;
using ChatAppServer.WebAPI.DTOs;
using ChatAppServer.WebAPI.Hubs;
using ChatAppServer.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class ChatsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatsController(ApplicationDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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

            string connectionId = ChatHub.Users.First(p => p.Value == chat.ToUserId).Key;

            await _hubContext.Clients.Client(connectionId).SendAsync("Message", chat);

            return Ok(chat);
        }
    }
}
