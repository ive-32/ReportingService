using Grpc.Core;
using ReportingServiceWrapper.Dtos;
using ReportingServiceWrapper.Proto;

namespace ReportingServiceWrapper;

public class WorkerRpcService : ReportingServiceWrapper.Proto.ReportingServiceWrapper.ReportingServiceWrapperBase
{
    private readonly IWorker _worker;

    public WorkerRpcService(IWorker worker)
    {
        _worker = worker;
    }
    
    public override async Task<ResponseData> GetData(RequestData request, ServerCallContext context)
    {
        if (Guid.TryParse(request.Id, out var id))
        {
            var result = await _worker.GetData(new RequestDto() { Id = id });

            return new ResponseData
            {
                Id = result.Id.ToString(),
                Response = result.Response
            };
        }
        else
        {
            return new ResponseData
            {
                Id = request.Id,
                Response = "Error wrong guid"
            };
        }
    }
}