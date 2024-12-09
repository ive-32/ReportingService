using ReportingServiceWrapper.Dtos;

namespace ReportingServiceWrapper;

public interface IReportingService : IHostedService
{
    Task SendRequest(RequestDto request);
    Task<ResponseDto?> GetResponse();
}