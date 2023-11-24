using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TicketApi.Shared.SimplifySetting;

/// <summary>
/// Интерфейся для регистрации контекста
/// </summary>
public interface IContextRegistration
{
    void RegistrationContext(IServiceCollection collection, IConfiguration configuration);
}