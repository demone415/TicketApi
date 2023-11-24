using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TicketApi.Shared.Configuration;

namespace TicketApi.Shared.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly ServiceOptions _serviceOptions;

    /// <summary>.ctor</summary>
    public ConfigureSwaggerOptions(
        IApiVersionDescriptionProvider provider,
        IOptionsMonitor<ServiceOptions> serviceOptionsMonitor)
    {
        _provider = provider;
        _serviceOptions = serviceOptionsMonitor.CurrentValue;
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var versionDescription in _provider.ApiVersionDescriptions)
        {
            var groupName = versionDescription.GroupName;
            var info = new OpenApiInfo();
            var interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
            interpolatedStringHandler.AppendFormatted(_serviceOptions.Name);
            interpolatedStringHandler.AppendLiteral(" ");
            interpolatedStringHandler.AppendFormatted(versionDescription.ApiVersion);
            info.Title = interpolatedStringHandler.ToStringAndClear();
            info.Version = versionDescription.ApiVersion.ToString();
            info.Description = _serviceOptions.Description;
            options.SwaggerDoc(groupName, info);
        }
    }
}