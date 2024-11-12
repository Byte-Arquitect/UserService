
using User_Service.Src.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddGrpc();
var app = builder.Build();


app.MapGrpcService<TestService>();
app.MapGrpcService<AuthService>();


app.Run();

