using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using TicketApi.Interfaces.Services;
using TicketApi.Models;

namespace TicketApi.Services;

public class ProverkaCheckaService : IProverkaCheckaService, IDisposable
{
    private const string ProverkaChekaUrl = "https://proverkacheka.com";
    private const string GetTicketDataUrl = "/api/v1/check/get";
    private readonly string _proverkaCheckaToken = Environment.GetEnvironmentVariable("PROVERKA_CHECKA_TOKEN");
    private readonly IRedisService _redisService;
    private readonly RestClient _restClient;
    public ProverkaCheckaService(IRedisService redisService)
    {
        if (string.IsNullOrEmpty(_proverkaCheckaToken))
            throw new ArgumentException(nameof(_proverkaCheckaToken));
        _redisService = redisService;
        var options = new RestClientOptions(ProverkaChekaUrl);
        _restClient = new RestClient(options, configureSerialization: s=> s.UseNewtonsoftJson());
    }
    
    public async Task<CheckResult> GetTicketDataFromQrString(string qrString)
    {
        if (!await _redisService.CanMakeRequestAsync(DateTime.Now))
        {
            return new CheckResult(ResultCodes.DailyRequestNumberExceeded);
        }
        var req = new RestRequest(GetTicketDataUrl);
        req.Method = Method.Post;
        req.AddParameter("token", _proverkaCheckaToken, ParameterType.GetOrPost);
        req.AddParameter("qrraw", qrString, ParameterType.GetOrPost);
        var response = await _restClient.ExecuteAsync(req);
        if (!response.IsSuccessStatusCode) 
            return new CheckResult(ResultCodes.SomethingWentWrong);
        await _redisService.IncreaseRequestCountAsync(DateTime.Now);
        return JsonConvert.DeserializeObject<CheckResult>(response.Content!);
    }

    public async Task<CheckResult> GetTicketDataFromQrData(QrData data)
    {
        /*
         *  token – Токен доступа
            fn - Номер фискального накопителя
            fd - Номер фискального документа
            fp - Фискальный признак документа
            t - Дата и время
            n - Тип операции (Приход/Возврат прихода/Расход/Возврат расхода)
            s - Сумма чека
            qr – Признак сканирования qr кода (0/1)
         * 
         */
        if (!await _redisService.CanMakeRequestAsync(DateTime.Now))
        {
            return new CheckResult(ResultCodes.DailyRequestNumberExceeded);
        }
        var req = new RestRequest(GetTicketDataUrl);
        req.Method = Method.Post;
        req.AddParameter("token", _proverkaCheckaToken, ParameterType.GetOrPost);
        req.AddParameter("fn", data.FiscalNumber, ParameterType.GetOrPost);
        req.AddParameter("fd", data.FiscalDocument, ParameterType.GetOrPost);
        req.AddParameter("fp", data.FiscalSign, ParameterType.GetOrPost);
        req.AddParameter("t", data.Date.ToString("yyyyMMddTHHmm"), ParameterType.GetOrPost);
        req.AddParameter("n", data.OperationType, ParameterType.GetOrPost);
        req.AddParameter("s", data.Sum, ParameterType.GetOrPost);
        req.AddParameter("qr", 1, ParameterType.GetOrPost);
        var response = await _restClient.ExecuteAsync(req);
        if (!response.IsSuccessStatusCode) 
            return new CheckResult(ResultCodes.SomethingWentWrong);
        await _redisService.IncreaseRequestCountAsync(DateTime.Now);
        return JsonConvert.DeserializeObject<CheckResult>(response.Content!);
    }

    public async Task<CheckResult> GetTicketDataFromQrUrl(string qrUrl)
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _restClient.Dispose();
    }
}