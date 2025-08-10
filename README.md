# OakTech Dynamic DNS for Netlify

A .NET 9 background service that automatically updates DNS records in Netlify when your public IP address changes. Perfect for self-hosted services that need to maintain up-to-date DNS records without a static IP address.

## Features

- 🔄 **Automatic IP Detection**: Monitors your public IP address using the ipify API
- 🌐 **Netlify DNS Integration**: Updates DNS records via Netlify's REST API
- 🐳 **Docker Support**: Containerized application ready for deployment
- ⚙️ **Environment Variable Configuration**: Override settings without rebuilding
- 📊 **Comprehensive Logging**: Detailed logging for monitoring and debugging
- 🔒 **Secure Token Handling**: Access tokens are masked in logs for security

## How It Works

1. The service polls the ipify API every minute to check your current public IP address
2. When an IP change is detected, it queries Netlify for DNS zones matching your configured domain
3. Updates all non-managed DNS records in the zone with the new IP address
4. Continues monitoring for future IP changes

## Prerequisites

- .NET 9.0 Runtime (for local development)
- Docker/Podman (for containerized deployment)
- Netlify account with DNS management enabled
- Netlify Personal Access Token

## Configuration

### Getting a Netlify Access Token

1. Log in to your Netlify account
2. Go to User Settings > Personal Access Tokens
3. Click "New access token"
4. Give it a descriptive name and create the token
5. Copy the token (starts with `nfp_`)

### Configuration Options

The application can be configured via `appsettings.json` or environment variables:

#### appsettings.json
```json
{
    "Ipify": {
        "Endpoint": "https://api.ipify.org"
    },
    "Netlify": {
        "Endpoint": "https://api.netlify.com/api/v1",
        "Domain": "your-domain.com",
        "AccessToken": "your-netlify-token"
    },
    "Logging": {
        "LogLevel": {
            "Default": "Debug"
        }
    }
}
```

#### Environment Variables
- `Ipify__Endpoint`: ipify API endpoint (default: https://api.ipify.org)
- `Netlify__Endpoint`: Netlify API endpoint (default: https://api.netlify.com/api/v1)
- `Netlify__Domain`: Your domain name (e.g., `example.com`)
- `Netlify__AccessToken`: Your Netlify Personal Access Token
- `Logging__LogLevel__Default`: Log level (Debug, Information, Warning, Error)

## Running the Application

### Docker/Podman (Recommended)

1. **Build the container:**
   ```bash
   # From the solution root directory
   docker build -f OakTech.DynamicDns.Netlify/Dockerfile -t oaktech-dynamic-dns .
   ```

2. **Run with environment variables:**
   ```bash
   docker run -e Netlify__AccessToken=your_token_here \
              -e Netlify__Domain=your-domain.com \
              oaktech-dynamic-dns
   ```

   Or with Podman:
   ```bash
   podman run -e Netlify__AccessToken=your_token_here \
              -e Netlify__Domain=your-domain.com \
              oaktech-dynamic-dns
   ```

### Local Development

1. **Clone and restore:**
   ```bash
   git clone <repository-url>
   cd OakTech.DynamicDns.Netlify
   dotnet restore
   ```

2. **Configure appsettings.json** or use user secrets:
   ```bash
   dotnet user-secrets set "Netlify:AccessToken" "your_token_here"
   dotnet user-secrets set "Netlify:Domain" "your-domain.com"
   ```

3. **Run the application:**
   ```bash
   cd OakTech.DynamicDns.Netlify
   dotnet run
   ```

## Project Structure

```
OakTech.DynamicDns.Netlify/
├── Clients/                    # API client interfaces and models
│   ├── IIpifyApi.cs           # ipify API client interface
│   ├── INetlifyApi.cs         # Netlify API client interface
│   ├── DnsEntryRequest.cs     # DNS record request model
│   ├── DnsEntryResponse.cs    # DNS record response model
│   ├── DnsForSiteResponse.cs  # DNS zone response model
│   └── IpifyResponse.cs       # IP address response model
├── Options/                   # Configuration option classes
│   ├── IpifyOptions.cs        # ipify API configuration
│   └── NetlifyOptions.cs      # Netlify API configuration
├── Services/                  # Business logic services
│   └── DomainManagementService.cs  # Main background service
├── Program.cs                 # Application entry point
├── appsettings.json          # Configuration file
└── Dockerfile                # Container build instructions
```

## Dependencies

- **Microsoft.Extensions.Hosting** (9.0.8) - Background service framework
- **Refit** (8.0.0) - HTTP API client library
- **Refit.HttpClientFactory** (8.0.0) - HttpClient integration

## Security Considerations

- 🔐 Access tokens are masked in log output (shows first 4 and last 4 characters only)
- 🚫 Never commit access tokens to source control
- 📝 Use environment variables or user secrets for sensitive configuration
- 🔄 Regularly rotate your Netlify access tokens

## Troubleshooting

### Common Issues

1. **"No managed DNS zone found"**
   - Ensure your domain is correctly configured in Netlify DNS
   - Verify the domain name matches exactly (case-sensitive)

2. **"Access Token changeme"**
   - Check that environment variables are passed correctly to the container
   - Ensure you're using double underscores (`__`) in environment variable names
   - Verify the container was rebuilt after adding `.AddEnvironmentVariables()`

3. **API Authentication Errors**
   - Verify your Netlify access token is valid and hasn't expired
   - Check that the token has the necessary permissions for DNS management

### Logs

The application provides detailed logging. Key log messages include:
- Service startup with masked access token
- IP address changes detection
- DNS record updates
- API errors and exceptions

## License

This project is licensed under the MIT License.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## Support

For issues and questions, please open an issue in the GitHub repository.
