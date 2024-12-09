using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ReportingServiceWrapper;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<IReportingService, ReportingServiceMoq>();
        builder.Services.AddHostedService(s => s.GetRequiredService<IReportingService>());
        builder.Services.AddSingleton<IWorker, Worker>();
        builder.Services.AddHostedService(s => s.GetRequiredService<IWorker>());
        builder.Services.TryAddScoped<WorkerRpcService>();
        builder.Services.AddGrpc();

        var host = builder.Build();
        host.MapGrpcService<WorkerRpcService>();
        host.Run();
    }
}