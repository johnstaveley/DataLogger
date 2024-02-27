using Grpc.Core;
using Grpc.Net.Client;
using GrpcService;

namespace GrpcClient;

public class Worker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IGenerator _generator;
    private readonly ILogger<Worker> _logger;
    private string _token = "";
    private DateTime _tokenExpiry = DateTime.MinValue;

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
            var client = await GetGrpcClient();
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
        Thread.Sleep(5000); // Wait for the gRPC service to start
        var useStream = false;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = await GetGrpcClient();
                if (!IsAuthenticated())
                {
                    if (!await Authenticate(client))
                    {
                        _logger.LogError("Failed to authenticate");
                    }
                }
                var headers = new Metadata();
                headers.Add("Authorization", $"Bearer {_token}");
                SuccessResponse reply;
                if (useStream)
                {
                    var stream = client.SubmitStream(headers);
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
                    reply = await client.SubmitReadingAsync(readingRequest, headers);
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Response {reply} running at: {time}", reply.IsSuccess, DateTimeOffset.Now);
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }
            catch (RpcException ex)
            {
                _logger.LogError("Error calling gRPC service: {Message}", ex.Message);
            }
        }
    }
    private bool IsAuthenticated()
    {
        return !string.IsNullOrEmpty(_token) && _tokenExpiry > DateTime.UtcNow;
    }
    private async Task<bool> Authenticate(DataLog.DataLogClient client)
    {
        try
        {
            var tokenRequest = new TokenRequest
            {
                Username = _configuration.GetValue<string>("UserName"),
                Password = _configuration.GetValue<string>("Password")
            };
            var tokenResponse = await client.AuthenticateAsync(tokenRequest);
            if (tokenResponse.Success) {
                _token = tokenResponse.Token;
                _tokenExpiry = tokenResponse.Expiry.ToDateTime();
                return true;
            }
        } catch (RpcException ex)
        {
            _logger.LogError("Error authenticating: {Message}", ex.Message);
        }
        return false;
    }
    private async Task<DataLog.DataLogClient> GetGrpcClient()
    {
        var serviceUrl = _configuration.GetValue<string>("ServiceUrl");
        var channel = GrpcChannel.ForAddress(serviceUrl);
        return new DataLog.DataLogClient(channel);
    }
}
