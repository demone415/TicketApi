using System.Data;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using TicketApi.Entities;
using TicketApi.Interfaces.Services;
using TicketApi.Shared.SimplifySetting;

namespace TicketApi.Services;

public class CategorizerServiceClient : ICategorizerServiceClient, IScopeRegistration, IDisposable
{
    //private const string ClassierUrl = "http://ticket-classifier.default.svc.cluster.local";
    private const string ServiceUrl = "http://localhost:5101";
    private const string CategorizeUrl = "/categorize";
    private readonly RestClient _restClient;
    private readonly ILogger<CategorizerServiceClient> _logger;

    public CategorizerServiceClient(ILogger<CategorizerServiceClient> logger)
    {
        var options = new RestClientOptions(ServiceUrl);
        _restClient = new RestClient(options, configureSerialization: s => s.UseNewtonsoftJson());
        _logger = logger;
    }

    public async Task<TicketHeader> CategorizeTicketAsync(TicketHeader header, CancellationToken ct)
    {
        var req = new RestRequest(CategorizeUrl);
        req.AddParameter("header", header, ParameterType.GetOrPost);
        var response = await _restClient.ExecutePostAsync<TicketHeader>(req, ct);

        if (response.Data is not null) return response.Data;

        _logger.LogError("Cannot get categories from service: {ex}", response.ErrorException);
        return header;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _restClient.Dispose();
    }
}