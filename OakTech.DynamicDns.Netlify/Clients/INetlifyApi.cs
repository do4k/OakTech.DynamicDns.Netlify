using Refit;

namespace OakTech.DynamicDns.Netlify.Clients;

public interface INetlifyApi
{
    [Delete("/dns_zones/{zoneId}/dns_records/{recordId}")]
    Task DeleteDnsRecordAsync(string zoneId, string recordId);

    [Post("/dns_zones/{zoneId}/dns_records")]
    Task<DnsEntryResponse> CreateDnsRecordsAsync(string zoneId, [Body] DnsEntryRequest request);
    
    [Get("/dns_zones")]
    Task<List<DnsRecordsForAccount>> GetDnsZonesAsync([Query("account_slug")] string accountSlug);
}