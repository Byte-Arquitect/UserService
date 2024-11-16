using Microsoft.EntityFrameworkCore;
using User_Service.Src.Middleware;
using User_Service.Src.Repositories;
using User_Service.Src.Repositories.Interfaces;
using User_Service.Src.Services;
using User_Service.Src.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMapperService, MapperService>();


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("userDB"));
});



builder.Services.AddScoped<AuthService>(); 

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionHandlingInterceptor>(); // Registrar el interceptor
});
builder.Services.AddGrpcReflection();

var app = builder.Build();


app.MapGrpcService<AuthController>();
app.MapGrpcService<TestService>();


if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}


app.Run();

