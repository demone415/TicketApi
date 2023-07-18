using System.Data;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using TicketApi.Entities;
using TicketApi.Interfaces.Services;

namespace TicketApi.Services;

public class ClassifierService : IClassifierService, IDisposable
{
    private const string ClassierUrl = "http://classifier";
    private const string ClassifyUrl = "/classify";
    private readonly RestClient _restClient;
    
    public ClassifierService()
    {
        var options = new RestClientOptions(ClassierUrl);
        _restClient = new RestClient(options, configureSerialization: s=> s.UseNewtonsoftJson());
    }
    
    public async Task<TicketHeader> ClassifyTicket(TicketHeader header)
    {
        var req = new RestRequest(ClassifyUrl);
        req.AddParameter("header", header, ParameterType.RequestBody);
        var response = await _restClient.ExecutePostAsync<TicketHeader>(req);
        if (response.Data is null) throw new DataException("Cannot get categories from service");
        return response.Data;
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _restClient.Dispose();
    }
}