using GrpcClient;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddTransient<IGenerator, Generator>();
var host = builder.Build();
host.Run();
