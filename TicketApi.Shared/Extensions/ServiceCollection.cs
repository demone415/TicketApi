using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace TicketApi.Shared.Extensions;

public static class ServiceCollection
{
    /// <summary>
    /// Установка Json настроек
    /// </summary>
    /// <param name="services"></param>
    public static void SetupNewtonsoft(this IServiceCollection services)
    {
        services.AddMvc().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Culture = System.Globalization.CultureInfo.GetCultureInfo("ru-RU");
            options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.SerializerSettings.Converters.Add(new StringEnumConverter(new DefaultNamingStrategy()));
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        });
    }
}