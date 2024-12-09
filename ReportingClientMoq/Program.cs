using Grpc.Net.Client;
using ReportingServiceWrapper.Proto;

namespace ReportingClientMoq;

class Program
{
    private static ReportingServiceWrapper.Proto.ReportingServiceWrapper.ReportingServiceWrapperClient _client;
    
    static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("http://localhost:5217");
        _client = new ReportingServiceWrapper.Proto.ReportingServiceWrapper.ReportingServiceWrapperClient(channel);

        var tasks = new List<Task<Task<ResponseData>>>();

        for (var i = 0; i < 10000; i++)
        {
            tasks.Add(GetData());
        }

        var result = await tasks[0].Result;
        var result2 = await tasks[10].Result;
        Console.WriteLine(result);
        Console.WriteLine(result2);
    }

    private static async Task<Task<ResponseData>> GetData()
    {
        await Task.Delay(Random.Shared.Next(0, 1000));
        
        return _client.GetDataAsync(new RequestData() { Id = Guid.NewGuid().ToString() },
            cancellationToken: CancellationToken.None).ResponseAsync;
    }
}