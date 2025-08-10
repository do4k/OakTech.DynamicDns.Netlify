using System.Text.Json.Serialization;

namespace OakTech.DynamicDns.Netlify.Clients;

public class DnsForSiteResponse
{
    public string? Id { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public List<string> Errors { get; set; } = new();
    public List<string> SupportedRecordTypes { get; set; } = new();
    public string? UserId { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<DnsEntryResponse> Records { get; set; } = new();
    public List<string> DnsServers { get; set; } = new();
    public string? AccountId { get; set; } = null!;
    public string? SiteId { get; set; } = null!;
    public string? AccountSlug { get; set; } = null!;
    public string? AccountName { get; set; } = null!;
    public string? Domain { get; set; } = null!;
    public bool? Ipv6Enabled { get; set; }
    public bool? Dedicated { get; set; }
}