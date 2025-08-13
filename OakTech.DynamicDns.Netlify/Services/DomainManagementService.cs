using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OakTech.DynamicDns.Netlify.Clients;
using OakTech.DynamicDns.Netlify.Options;

namespace OakTech.DynamicDns.Netlify.Services;

public class DomainManagementService(
    IIpifyApi ipifyApi,
    INetlifyApi netlifyApi,
    IOptions<NetlifyOptions> netlifyOptions,
    ILogger<DomainManagementService> logger)
    : BackgroundService
{
    private string _cachedIp = string.Empty;
    private readonly NetlifyOptions _netlifyOptions = netlifyOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting Domain Management Service...");
        while (stoppingToken.IsCancellationRequested is false)
        {
            try
            {
                var ipResponse = await ipifyApi.GetIpAsync();
                if (ipResponse.Ip == _cachedIp)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }
                
                logger.LogInformation("Detected IP change: {OldIp} -> {NewIp}", _cachedIp, ipResponse.Ip);
                var dnsZones = await netlifyApi.GetDnsZonesAsync(_netlifyOptions.AccountSlug);
                var managedZone = dnsZones.SingleOrDefault(x => x.Name == _netlifyOptions.Domain);
                if (managedZone == null)
                {
                    logger.LogWarning("No managed DNS zone found for domain {Domain}", _netlifyOptions.Domain);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }

                foreach (var record in managedZone.Records.Where(x => x.Managed is false))
                {
                    logger.LogInformation("Updating DNS record: {Record} with new IP address {IpAddress}", record.Hostname, ipResponse.Ip);
                    await netlifyApi.DeleteDnsRecordAsync(record.DnsZoneId, record.Id);
                    var newEntry = new DnsEntryRequest
                    {
                        Type = record.Type,
                        Hostname = record.Hostname,
                        Value = ipResponse.Ip,
                    };
                    var result = await netlifyApi.CreateDnsRecordsAsync(record.DnsZoneId, newEntry);
                    logger.LogInformation("Created DNS entry: {Entry}", result.Hostname);
                }
                
                _cachedIp = ipResponse.Ip;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while updating DNS entry");
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}