using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<DeveloperResponse> Developers(Empty request, ServerCallContext context)
        {
            // return DeveloperREsponse via grpc


            var developerResponse = new DeveloperResponse();
            developerResponse.Developers.Add(new DeveloperName { Name = "John" });
            developerResponse.Developers.Add(new DeveloperName { Name = "Daniel" });
            developerResponse.Developers.Add(new DeveloperName { Name = "Emma" });
            developerResponse.Developers.Add(new DeveloperName { Name = "Matt" });
            return Task.FromResult<DeveloperResponse>(developerResponse);
        }
    }
}
