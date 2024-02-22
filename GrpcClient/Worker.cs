using Grpc.Net.Client;
using GrpcService;
using System.Net.WebSockets;

namespace GrpcClient
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Worker> _logger;

        public Worker(IConfiguration configuration, ILogger<Worker> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
        }
    }
}
