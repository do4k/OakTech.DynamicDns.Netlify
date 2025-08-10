namespace OakTech.DynamicDns.Netlify.Clients;

public class DnsEntryRequest
{
    public required string Type { get; set; }
    public required string Hostname { get; set; }
    public required string Value { get; set; }
    public int? Ttl { get; set; }
    public int? Priority { get; set; }
    public int? Weight { get; set; }
    public int? Port { get; set; }
    public int? Flag { get; set; }
    public string? Tag { get; set; }
}