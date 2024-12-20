using GrpcService.Model;
using GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<JwtTokenValidationService>();
builder.Services.AddAuthentication()
  .AddJwtBearer(cfg =>
  {
      cfg.TokenValidationParameters = new DataLogTokenValidationParameters(builder.Configuration);
  });
builder.Services.AddAuthorization();
builder.Services.AddCors(cfg =>
{
    cfg.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// TODO: Might want to add in an actual authorization provider here
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
builder.Services.AddGrpc(config => config.EnableDetailedErrors = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseGrpcWeb();
app.MapGrpcService<TelemetryService>().EnableGrpcWeb().RequireCors("AllowAll");
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909").RequireCors("AllowAll");
app.Run();
