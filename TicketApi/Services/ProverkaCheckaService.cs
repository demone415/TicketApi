using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using TicketApi.Interfaces.Services;
using TicketApi.Models;
using TicketApi.Models.Requests;
using TicketApi.Shared.SimplifySetting;

namespace TicketApi.Services;

public class ProverkaCheckaService : IProverkaCheckaService, IScopeRegistration
{
    private readonly string _proverkaCheckaToken;
    private readonly IRedisService _redisService;
    private readonly IProverkaCheckaServiceClient _serviceClient;
    private readonly ILogger<ProverkaCheckaService> _logger;

    public ProverkaCheckaService(
        IRedisService redisService,
        IProverkaCheckaServiceClient serviceClient,
        IConfiguration configuration,
        ILogger<ProverkaCheckaService> logger)
    {
        var section = configuration.GetSection("ProverkaChecka");
        _proverkaCheckaToken = section["Token"];

        if (string.IsNullOrEmpty(_proverkaCheckaToken))
            throw new ArgumentException(nameof(_proverkaCheckaToken));

        _redisService = redisService;
        _serviceClient = serviceClient;
        _logger = logger;
    }

    public async Task<CheckResult> GetTicketDataFromQrString(string qrString, CancellationToken ct)
    {
        if (!await _redisService.CanMakeRequestAsync(DateTime.Now))
            return new CheckResult(ResultCodes.DailyRequestNumberExceeded);

        var request = new QrStringCheckRequest(qrString, _proverkaCheckaToken);

        var response = await _serviceClient.GetTicketDataFromQrString(request, ct);
        if (response.Error != null)
        {
            _logger.LogError(response.Error, "");
            return new CheckResult(ResultCodes.SomethingWentWrong);
        }
        await _redisService.IncreaseRequestCountAsync(DateTime.Now);
        return response.Content;
    }

    public async Task<CheckResult> GetTicketDataFromQrData(QrData data, CancellationToken ct)
    {
        if (!await _redisService.CanMakeRequestAsync(DateTime.Now))
            return new CheckResult(ResultCodes.DailyRequestNumberExceeded);

        var request = new QrDataCheckRequest(data, _proverkaCheckaToken);

        var response = await _serviceClient.GetTicketDataFromQrData(request, ct);
        if (!response.IsSuccessStatusCode) return new CheckResult(ResultCodes.SomethingWentWrong);

        await _redisService.IncreaseRequestCountAsync(DateTime.Now);
        return response.Content;
    }

    public async Task<CheckResult> GetTicketDataFromQrUrl(string qrUrl, CancellationToken ct)
    {
        if (!await _redisService.CanMakeRequestAsync(DateTime.Now))
            return new CheckResult(ResultCodes.DailyRequestNumberExceeded);

        var request = new QrUrlCheckRequest(qrUrl, _proverkaCheckaToken);

        var response = await _serviceClient.GetTicketDataFromQrUrl(request, ct);
        if (!response.IsSuccessStatusCode) return new CheckResult(ResultCodes.SomethingWentWrong);

        await _redisService.IncreaseRequestCountAsync(DateTime.Now);
        return response.Content;
    }
}