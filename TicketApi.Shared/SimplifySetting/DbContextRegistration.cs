using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;

namespace TicketApi.Shared.SimplifySetting;

/// <summary>
/// Расширение для регистрираяции DB контекста
/// </summary>
public static class DbContextRegistration
{
    /// <summary>
    /// Регистрация DB контекст
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="typeOfRegistration"></param>
    /// <param name="configuration"></param>
    public static void RegisterContext(this IServiceCollection serviceCollection, Type typeOfRegistration,
        IConfiguration configuration)
    {
        var assembly = Assembly.GetAssembly(typeOfRegistration);

        if (assembly == null)
            return;

        foreach (var context in AppDomain.CurrentDomain.GetAssemblies()
                     .Where(x => x.FullName != null)
                     .SelectMany(s => s.GetTypes())
                     .Where(p => typeOfRegistration.IsAssignableFrom(p) && p.IsClass))
        {
            var contextInstance = context.CreateInstance();
            var methodOfRegistration = contextInstance.GetType().GetMethod("RegistrationContext",
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder,
                new[] { typeof(IServiceCollection), typeof(IConfiguration) }, null);

            methodOfRegistration?.Invoke(contextInstance,
                new object[] { serviceCollection, configuration });
        }
    }
}