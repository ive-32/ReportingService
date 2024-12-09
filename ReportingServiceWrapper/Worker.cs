using System.Collections.Concurrent;
using ReportingServiceWrapper.Dtos;

namespace ReportingServiceWrapper;

public interface IWorker : IHostedService 
{
    Task<ResponseDto> GetData(RequestDto request);
}

public class Worker : BackgroundService, IWorker
{
    private readonly ILogger<Worker> _logger;

    private ConcurrentDictionary<Guid, TaskCompletionSource<ResponseDto>> _responses = new();
    private readonly IReportingService _reportingService;

    public Worker(ILogger<Worker> logger, IReportingService reportingService)
    {
        _logger = logger;
        _reportingService = reportingService;
    }

    public async Task<ResponseDto> GetData(RequestDto request)
    {
        _logger.LogInformation("GetData, {CurrentThreadId}, {id}", Environment.CurrentManagedThreadId, request.Id);
        var responseTask = new TaskCompletionSource<ResponseDto>();
        _responses.TryAdd(request.Id, responseTask);
        await _reportingService.SendRequest(request);
        
        var result = await responseTask.Task;
        _logger.LogInformation("Response received, responses count {Count}", _responses.Count);
        return result;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker is started, {CurrentThreadId}", Environment.CurrentManagedThreadId);
        while (!stoppingToken.IsCancellationRequested)
        {
            ResponseDto? response = new ResponseDto();
            while (response != null && !_responses.IsEmpty)
            {
                response = await _reportingService.GetResponse();
                
                if (response is null) 
                    continue;
                
                _logger.LogInformation("Get response, {CurrentThreadId} {Id}", Environment.CurrentManagedThreadId,
                    response.Id);
                
                if (_responses.TryRemove(response.Id, out var tcs))
                {
                    tcs.SetResult(response);
                }
            }
            
            await Task.Delay(64, stoppingToken);
        }
    }
}