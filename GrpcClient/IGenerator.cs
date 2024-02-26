using Google.Protobuf.WellKnownTypes;
using GrpcService;

namespace GrpcClient;

public interface IGenerator
{
    ReadingRequest NewReading();
}

public class Generator : IGenerator
{
    public ReadingRequest NewReading()
    {
        return new ReadingRequest
        {
            Humidity = new Random().Next(0, 100),
            Temperature = new Random().Next(18, 40),
            Pressure = new Random().Next(100000, 101000),
            Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
        };
    }
}
