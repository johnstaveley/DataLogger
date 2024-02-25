using GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseGrpcWeb();
app.MapGrpcService<TelemetryService>().EnableGrpcWeb().RequireCors(builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909").RequireCors("AllowAll");
app.UseCors();
app.Run();
