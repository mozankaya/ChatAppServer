using ChatAppServer.WebAPI.Data;
using ChatAppServer.WebAPI.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatApp API v1");
    c.RoutePrefix = "swagger"; // /swagger
});

app.UseHttpsRedirection();

// CORS middleware
app.UseCors("ChatCorsPolicy");

app.UseAuthorization();

app.MapControllers();

// SignalR Hub
app.MapHub<ChatHub>("/chathub");

app.Run();
