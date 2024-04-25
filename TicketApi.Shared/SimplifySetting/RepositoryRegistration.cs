using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TicketApi.Shared.SimplifySetting;

/// <summary>
/// Расширение для регистрации зависимостей репозиториев по интерфейсу
/// </summary>
public static class RepositoryRegistration
{
    /// <summary>
    /// Регистрация всех репозиториев
    /// </summary>
    /// <param name="cpServiceCollection"></param>
    /// <param name="configuration"></param>
    public static void AddRepositories(this IServiceCollection cpServiceCollection,
        IConfiguration configuration)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var context in assemblies
                     .Where(x => x.FullName != null && x.FullName.Contains("TicketApi"))
                     .SelectMany(s => s.GetTypes())
                     .Where(p => p.FullName != null && typeof(IRegisterRepository).IsAssignableFrom(p) && p.IsClass
                                 && !p.FullName.Contains("RepositoryBase")))
        {
            var interfaceType = assemblies
                .Where(x => x.FullName != null && x.FullName.Contains("TicketApi"))
                .SelectMany(s => s.GetTypes()).FirstOrDefault(p =>
                    p.FullName != null && p.IsAssignableFrom(context) && p.IsClass &&
                    !p.FullName.Contains("RepositoryBase"))
                ?.GetInterfaces().FirstOrDefault(x => x.FullName?.Contains(context.Name) ?? false);

            if (interfaceType != null)
            {
                cpServiceCollection.AddScoped(interfaceType, context);
                Console.WriteLine($"Added repository {context.Name} implementation of {interfaceType.Name}");
            }
            else
            {
                cpServiceCollection.AddScoped(context);
                Console.WriteLine($"Added repository {context.Name} without implementation interface");
            }
        }
    }
}