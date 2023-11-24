namespace TicketApi.Shared.Configuration;

public sealed class ContextOptions
{
    public string Url { get; set; }

    public string Port { get; set; }

    public string ClusterHosts { get; set; }

    public string DataBase { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string UrlSlave { get; set; }

    public string GetConnectionString(string host)
    {
        return
            $"Server={host};Port={Port};Database={DataBase};Uid={UserName};Pwd={Password};Trust Server Certificate=true;Load Table Composites= true;";
    }
}