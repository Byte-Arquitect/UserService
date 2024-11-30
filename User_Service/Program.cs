using Microsoft.EntityFrameworkCore;
using User_Service.Src.Middleware;
using User_Service.Src.Repositories;
using User_Service.Src.Repositories.Interfaces;
using User_Service.Src.Services;
using User_Service.Src.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuración de AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Repositorios y servicios
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMapperService, MapperService>();
builder.Services.AddScoped<AuthService>();

// Configuración de DbContext con PostgreSQL
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("userDB"));
});

// Configuración de gRPC con JSON Transcoding e interceptores
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionHandlingInterceptor>();
}).AddJsonTranscoding();
builder.Services.AddGrpcReflection();

// Swagger para documentación (opcional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mapear servicios gRPC
app.MapGrpcService<AuthController>();
app.MapGrpcService<TestService>();

// Reflexión de gRPC (solo desarrollo)
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.Run();
