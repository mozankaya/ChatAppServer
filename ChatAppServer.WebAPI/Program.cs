using ChatAppServer.WebAPI.Data;
using ChatAppServer.WebAPI.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();

builder.Services.AddOpenApi();

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("ChatCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular frontend address
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); //For signalR
    });
});

builder.Services.AddSignalR();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// CORS middleware
app.UseCors("ChatCorsPolicy");

app.UseAuthorization();

app.MapControllers();

// SignalR Hub
app.MapHub<ChatHub>("/chathub");

app.Run();
