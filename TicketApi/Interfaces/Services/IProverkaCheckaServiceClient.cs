using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Timeout;
using TicketApi.Models;
using Refit;
using TicketApi.Models.Requests;

namespace TicketApi.Interfaces.Services;

public interface IProverkaCheckaServiceClient
{
    [Post("/api/v1/check/get")]
    public Task<ApiResponse<CheckResult>> GetTicketDataFromQrString(
        [Body(BodySerializationMethod.UrlEncoded)] QrStringCheckRequest request, CancellationToken ct);

    [Post("/api/v1/check/get")]
    public Task<ApiResponse<CheckResult>> GetTicketDataFromQrData(
        [Body(BodySerializationMethod.UrlEncoded)] QrDataCheckRequest request, CancellationToken ct);

    [Post("/api/v1/check/get")]
    public Task<ApiResponse<CheckResult>> GetTicketDataFromQrUrl(
        [Body(BodySerializationMethod.UrlEncoded)] QrUrlCheckRequest request, CancellationToken ct);
}

public static partial class ServicesRegistration
{
    public static void AddProverkaCheckaServiceClient(this IServiceCollection services, Uri serviceUri)
    {
        var settings = new RefitSettings(new NewtonsoftJsonContentSerializer());
        services.AddRefitClient<IProverkaCheckaServiceClient>(settings)
            .ConfigureHttpClient(c => { c.BaseAddress = serviceUri; })
            .AddPolicyHandler(Policy
                .HandleResult<HttpResponseMessage>(x =>
                    x.StatusCode == HttpStatusCode.NotFound || (int)x.StatusCode >= 500)
                .RetryAsync(5)
                .WrapAsync(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(5000),
                    TimeoutStrategy.Optimistic)));
    }
}