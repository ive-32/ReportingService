using System.Collections.Concurrent;
using ReportingServiceWrapper.Dtos;

namespace ReportingServiceWrapper;

public class ReportingServiceMoq : BackgroundService, IReportingService
{
    private ConcurrentQueue<RequestDto> _requests = new ConcurrentQueue<RequestDto>();
    private ConcurrentQueue<ResponseDto> _responses = new ConcurrentQueue<ResponseDto>();
    private Random _random = new Random();
    
    public Task SendRequest(RequestDto request)
    {
        _requests.Enqueue(request);
        return Task.CompletedTask;
    }

    public Task<ResponseDto?> GetResponse()
    {
        return _responses.TryDequeue(out var responseDto) 
            ? Task.FromResult((ResponseDto?) responseDto) 
            : Task.FromResult((ResponseDto?) null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_requests.TryDequeue(out var requestDto))
            {
                var random = _random.Next(0, 100); 
                if (random < 10)
                {
                    for (var i = 0; i < random; i++)
                    {
                        _responses.Enqueue(new ResponseDto()
                        {
                            Id = Guid.NewGuid(),
                            Response = "Response by random"
                        });
                    }                    
                }
                
                _responses.Enqueue(new ResponseDto()
                {
                    Id = requestDto.Id,
                    Response = "Response by request"
                });
            }
            await Task.Delay(32, stoppingToken);
        }
    }
}