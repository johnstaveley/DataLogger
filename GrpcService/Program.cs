using GrpcService.Model;
using GrpcService.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<JwtTokenValidationService>();
builder.Services.AddAuthentication()
  .AddJwtBearer(cfg =>
  {
      cfg.TokenValidationParameters = new DataLogTokenValidationParameters(builder.Configuration);
  });
builder.Services.AddAuthorization();
builder.Services.AddCors();
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    // TODO: Add in auth provider here
    ;
builder.Services.AddGrpc(config => config.EnableDetailedErrors = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseGrpcWeb();
app.MapGrpcService<TelemetryService>().EnableGrpcWeb().RequireCors(builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909").RequireCors("AllowAll");
app.Run();
