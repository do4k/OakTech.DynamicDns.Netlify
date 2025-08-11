using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OakTech.DynamicDns.Netlify.Clients;
using OakTech.DynamicDns.Netlify.Options;

namespace OakTech.DynamicDns.Netlify.Services;

public class DomainManagementService : BackgroundService
{
    private readonly IIpifyApi _ipifyApi;
    private readonly INetlifyApi _netlifyApi;
    private readonly ILogger<DomainManagementService> _logger;
    private string _cachedIp = string.Empty;
    private readonly NetlifyOptions _netlifyOptions;
    private readonly string _token;

    public DomainManagementService(
        IIpifyApi ipifyApi, 
        INetlifyApi netlifyApi, 
        IOptions<NetlifyOptions> netlifyOptions, 
        ILogger<DomainManagementService> logger)
    {
        _ipifyApi = ipifyApi;
        _netlifyApi = netlifyApi;
        _netlifyOptions = netlifyOptions.Value;
        _logger = logger;
        _token = $"Bearer {_netlifyOptions.AccessToken}";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Domain Management Service...");
        while (stoppingToken.IsCancellationRequested is false)
        {
            var ipResponse = await _ipifyApi.GetIpAsync();
            if (ipResponse.Ip == _cachedIp)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                continue;
            }
            
            _logger.LogInformation("Detected IP change: {OldIp} -> {NewIp}", _cachedIp, ipResponse.Ip);
            try
            {
                var dnsZones = await _netlifyApi.GetDnsZonesAsync(_netlifyOptions.AccountSlug, _token);
                var managedZone = dnsZones.SingleOrDefault(x => x.Name == _netlifyOptions.Domain);
                if (managedZone == null)
                {
                    _logger.LogWarning("No managed DNS zone found for domain {Domain}", _netlifyOptions.Domain);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }

                foreach (var record in managedZone.Records.Where(x => x.Managed is false))
                {
                    _logger.LogInformation("Updating DNS record: {Record} with new IP address {IpAddress}", record, ipResponse.Ip);
                    await _netlifyApi.DeleteDnsRecordAsync(record.DnsZoneId, record.Id, _token);
                    var newEntry = new DnsEntryRequest
                    {
                        Type = record.Type,
                        Hostname = record.Hostname,
                        Value = ipResponse.Ip,
                    };
                    var result = await _netlifyApi.CreateDnsRecordsAsync(record.DnsZoneId, newEntry, _token);
                    _logger.LogInformation("Created DNS entry: {Entry}", result);
                }
                
                _cachedIp = ipResponse.Ip;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating DNS entry");
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}