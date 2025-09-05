namespace ChatAppServer.WebAPI.DTOs
{
    public sealed record RegisterDTO(
        string Name,
        IFormFile File);
}
