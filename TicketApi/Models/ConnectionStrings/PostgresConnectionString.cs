namespace TicketApi.Models.ConnectionStrings;

public class PostgresConnectionString
{
    public string Value { get; set; }

    public PostgresConnectionString(string value)
    {
        Value = value;
    }
}