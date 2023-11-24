using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace TicketApi.Shared.SimplifySetting;

public static class InterfaceRegistration
{
    /// <summary>
    /// Регистрация всех класов соотвествующих типовому интерфесу
    /// </summary>
    /// <param name="cpServiceCollection"></param>
    /// <param name="interfaceRegistration"></param>
    public static void RegisterByInterface(this IServiceCollection cpServiceCollection, Type interfaceRegistration)
    {
        var assemblies = GetSolutionAssemblies();
        foreach (var context in assemblies
                     .Where(x => x.FullName != null && x.FullName.Contains("TicketApi"))
                     .SelectMany(s => s.GetTypes())
                     .Where(p => interfaceRegistration.IsAssignableFrom(p) && p.IsClass))
        {
            var interfaceType = assemblies
                .Where(x => x.FullName != null && x.FullName.Contains("TicketApi"))
                .SelectMany(s => s.GetTypes()).FirstOrDefault(p =>
                    p.IsAssignableFrom(context) && p.IsClass &&
                    !p.FullName.Contains("RepositoryBase"))
                ?.GetInterfaces().FirstOrDefault(x => x.FullName != null);

            if (interfaceType != null)
            {
                cpServiceCollection.AddScoped(interfaceType, context);
                Log.Information("Added service {ContextName} implementation of {InterfaceTypeName} to Scoped",
                    context.Name, interfaceType.Name);
            }
            else
            {
                cpServiceCollection.AddScoped(context);
                Log.Information("Added service {ContextName} without implementation interface to Scoped", context.Name);
            }
        }
    }

    public static Assembly[] GetSolutionAssemblies()
    {
        var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)));
        return assemblies.ToArray();
    }
}