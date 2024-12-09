## ReportingServiceWrapper example

### Задача.
Сервис отчетов. 
Два метода, первый посылает запрос на отчет 
Второй получает первый готовый отчет 
(отчеты возвращаются в произвольном порядке по мере готовности). 

```c#
public interface IReportingService : IHostedService
{
    Task SendRequest(RequestDto request);
    Task<ResponseDto?> GetResponse();
}
```

требуется сделать обертку, 
которая будет в одном запросе отправлять запрос и дожидаться ответа

### ReportServiceWrapper 
grpc сервис который исполняет этот функционал<br>
метод GetData отправляет запрос и дожидается ответа  

```protobuf
service ReportingServiceWrapper {
  rpc GetData (RequestData) returns (ResponseData);
}

message RequestData {
  string Id = 1;
}

message ResponseData {
  string Id = 1;
  string Response = 2;
}
```

1. ReportingServiceMoq.cs - мок самого сервиа отчетов, генерирует несколько случайных ответов и тот который ожидается
2. WorkerRpcService.cs - rpc сервис
3. Worker.cs - воркер который опрашивает ReportingService и сохраняет очередь ответов


### ReportingServiceWrapper.Proto
прото для ReportServiceWrapper
собирается в nuget для переиспользования

### ReportingClientMoq
эмулятор клиента - отправляет 10000 запросов на сервер, 
отображает первый и десятый ответ