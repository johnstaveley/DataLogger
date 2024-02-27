using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace GrpcService.Services
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TelemetryService : DataLog.DataLogBase
    {
        private readonly JwtTokenValidationService _tokenValidationService;
        private readonly ILogger<TelemetryService> _logger;
        public TelemetryService(JwtTokenValidationService tokenValidationService, ILogger<TelemetryService> logger)
        {
            _logger = logger;
            _tokenValidationService = tokenValidationService;
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

        public async override Task SubmitStream(IAsyncStreamReader<ReadingRequest> requestStream, 
            IServerStreamWriter<SuccessResponse> responseStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var currentReading = requestStream.Current;
                _logger.LogInformation("Received reading {request}", currentReading);
                await responseStream.WriteAsync(new SuccessResponse { IsSuccess = true });
            }
        }

        [AllowAnonymous]
        public override async Task<TokenResponse> Authenticate(TokenRequest request, ServerCallContext context)
        {
            var credentials =  new CredentialModel
            {
                UserName = request.UserName,
                Password = request.Password
            };
            var result = await _tokenValidationService.GenerateTokenModelAsync(credentials);
            if (result.Success)
            {
                return new TokenResponse { Success = true, Token = result.Token, Expiry = Timestamp.FromDateTime(result.Expiration) };
            }
            else
            {
                return new TokenResponse { Success = false, Token = "" };
            }
        }
    }
}
