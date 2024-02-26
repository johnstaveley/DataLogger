using Grpc.Net.Client;
using GrpcService;

namespace GrpcClient;

public class Worker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IGenerator _generator;
    private readonly ILogger<Worker> _logger;

    public Worker(IConfiguration configuration, IGenerator generator, ILogger<Worker> logger)
    {
        _configuration = configuration;
        _generator = generator;
        _logger = logger;
    }

    /*protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Thread.Sleep(5000);
        while (!stoppingToken.IsCancellationRequested)
        {
            var serviceUrl = _configuration.GetValue<string>("ServiceUrl");
            var channel = GrpcChannel.ForAddress(serviceUrl);
            var client = new DataLog.DataLogClient(channel);
            var name = Guid.NewGuid().ToString();
            var reply = await client.IsAliveAsync(new IsAliveRequest { Name = name });

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Response {reply} running at: {time}", reply.Message, DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }*/

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Thread.Sleep(5000);
        var useStream = true;
        while (!stoppingToken.IsCancellationRequested)
        {
            var serviceUrl = _configuration.GetValue<string>("ServiceUrl");
            var channel = GrpcChannel.ForAddress(serviceUrl);
            var client = new DataLog.DataLogClient(channel);
            var stream = client.SubmitStream();
            SuccessResponse reply;
            if (useStream)
            {
                for (int i = 0; i < 10; i++)
                {
                    var readingRequest = _generator.NewReading();
                    await stream.RequestStream.WriteAsync(readingRequest);
                    await Task.Delay(400);
                }
                await stream.RequestStream.CompleteAsync();
                while (await stream.ResponseStream.MoveNext(new CancellationToken()))
                {
                    reply = stream.ResponseStream.Current;
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Response {reply} running at: {time}", reply.IsSuccess, DateTimeOffset.Now);
                    }
                }
            }
            else
            {
                var readingRequest = _generator.NewReading();
                reply = await client.SubmitReadingAsync(readingRequest);
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Response {reply} running at: {time}", reply.IsSuccess, DateTimeOffset.Now);
                }
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
