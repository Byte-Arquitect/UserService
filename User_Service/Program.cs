using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Messages;
using User_Service.Src.Controllers;
using User_Service.Src.Extensions;
using User_Service.Src.Middleware;
using User_Service.Src.Producers;
using User_Service.Src.Repositories;
using User_Service.Src.Repositories.Interfaces;
using User_Service.Src.Services;
using User_Service.Src.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuración de AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Repositorios y servicios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMapperService, MapperService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ApiGatewayService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RegisterEvent>();
builder.Services.AddScoped<UpdatePasswordEvent>();

builder.Services.AddHttpClient();

// Configuración de DbContext con PostgreSQL
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("userDB"));
});

// Configuración de gRPC con JSON Transcoding e interceptores
builder
    .Services.AddGrpc(options =>
    {
        options.Interceptors.Add<ExceptionHandlingInterceptor>();
    })
    .AddJsonTranscoding();
builder.Services.AddGrpcReflection();

// Swagger para documentación (opcional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq(
        (context, cfg) =>
        {
            cfg.Host(
                "localhost",
                "/",
                h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                }
            );

            cfg.Send<RegisterUserMessage>(config =>
            {
                config.UseRoutingKeyFormatter(context => "register-user-queue");
            });

            cfg.Send<UpdatePasswordMessage>(config =>
            {
                config.UseRoutingKeyFormatter(context => "update-password-queue");
            });
        }
    );
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();
    context.Database.EnsureCreated();
}

// Configurar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mapear servicios gRPC
app.MapGrpcService<AuthController>();
app.MapGrpcService<UserController>();
app.MapGrpcService<TestService>();

// Reflexión de gRPC (solo desarrollo)
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

AppSeedService.SeedDatabase(app);

app.Run();
