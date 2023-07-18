namespace TicketApi.Models.ConnectionStrings;

public static class ConnectionStrings
{
    public static RedisConnectionModel Redis { get; set; }
    public static RabbitConnectionModel Rabbit { get; set; }
    public static PostgresConnectionString Postgres { get; set; }
}