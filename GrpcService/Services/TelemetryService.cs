using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcService.Services
{
    public class TelemetryService : DataLog.DataLogBase
    {
        private readonly ILogger<TelemetryService> _logger;
        public TelemetryService(ILogger<TelemetryService> logger)
        {
            _logger = logger;
        }

        public override Task<IsAliveReply> IsAlive(IsAliveRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Responding to {Name}", request.Name);
            return Task.FromResult(new IsAliveReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<SuccessResponse> SubmitReading(ReadingRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received reading {request}", request);
            return Task.FromResult(new SuccessResponse { IsSuccess = true });
        }

        public override Task<DeveloperResponse> Developers(Empty request, ServerCallContext context)
        {
            _logger.LogInformation("Sending list of developers");
            var developerResponse = new DeveloperResponse();
            developerResponse.Developers.Add(new DeveloperName { Name = "John" });
            developerResponse.Developers.Add(new DeveloperName { Name = "Daniel" });
            developerResponse.Developers.Add(new DeveloperName { Name = "Emma" });
            developerResponse.Developers.Add(new DeveloperName { Name = "Matt" });
            return Task.FromResult<DeveloperResponse>(developerResponse);
        }
    }
}
