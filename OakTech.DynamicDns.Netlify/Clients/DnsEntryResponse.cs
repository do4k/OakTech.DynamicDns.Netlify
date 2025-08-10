namespace OakTech.DynamicDns.Netlify.Clients;

public class DnsEntryResponse
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    public required string Hostname { get; set; }
    public required string Value { get; set; }
    public int Ttl { get; set; }
    public int? Priority { get; set; }
    public string? DnsZoneId { get; set; }
    public string? SiteId { get; set; }
    public int? Flag { get; set; }
    public string? Tag { get; set; }
    public bool Managed { get; set; }
}