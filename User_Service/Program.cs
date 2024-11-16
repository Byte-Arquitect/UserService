using User_Service.Src.Middleware;
using User_Service.Src.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<AuthService>(); // Registro de AuthService

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

