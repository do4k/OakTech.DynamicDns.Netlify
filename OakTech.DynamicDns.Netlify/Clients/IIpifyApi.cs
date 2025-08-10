using Refit;

namespace OakTech.DynamicDns.Netlify.Clients;

public interface IIpifyApi
{
    [Get("/?format=json")]
    Task<IpifyResponse> GetIpAsync();
}