namespace OakTech.DynamicDns.Netlify.Options;

public class NetlifyOptions
{
    public required string Endpoint { get; set; }
    public required string Domain { get; set; }
    public required string AccessToken { get; set; }
    public required string AccountSlug { get; set; }
}