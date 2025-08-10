using Refit;

namespace OakTech.DynamicDns.Netlify.Clients;

public interface INetlifyApi
{
    [Get("/sites/{siteId}/dns")]
    Task<DnsForSiteResponse> GetDnsRecordsForSiteAsync(string siteId, [Header("Authorization")] string bearerToken);

    [Delete("/dns_zones/{zoneId}/dns_records/{recordId}")]
    Task DeleteDnsRecordAsync(string zoneId, string recordId, [Header("Authorization")] string bearerToken);

    [Post("/dns_zones/{zoneId}/dns_records")]
    Task<DnsEntryResponse> CreateDnsRecordsAsync(string zoneId, [Body] DnsEntryRequest request, [Header("Authorization")] string bearerToken);
    
    [Get("/dns_zones/{zoneId}/dns_records")]
    Task<List<DnsEntryResponse>> GetDnsEntriesAsync(string zoneId, [Header("Authorization")] string bearerToken);
    
    [Get("/dns_zones")]
    Task<List<DnsForSiteResponse>> GetDnsZonesAsync([Query("account_slug")] string accountSlug, [Header("Authorization")] string bearerToken);
}