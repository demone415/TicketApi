namespace TicketApi.Shared.Swagger;

/// <summary>
/// Атрибут для указания названия группы операций в swagger
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SwaggerGroupAttribute : Attribute
{
    /// <summary>Имя группы</summary>
    public string GroupName { get; }

    /// <summary>.ctor</summary>
    /// <param name="groupName">Имя группы</param>
    public SwaggerGroupAttribute(string groupName)
    {
        GroupName = groupName;
    }
}