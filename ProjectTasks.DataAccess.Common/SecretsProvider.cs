using System;
using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ProjectTasks.DataAccess.Common;

public class SecretsProvider
{
    private ILogger<SecretsProvider> _log;
    private SecretClient _keyVaultClient;

    public SecretsProvider(ILogger<SecretsProvider> logger, IConfiguration configuration, TokenCredential azureCredentials)
    {
        _log = logger;
        var secretClientOptions = new SecretClientOptions
        {
            Retry =
            {
                Delay= TimeSpan.FromSeconds(2),
                MaxDelay = TimeSpan.FromSeconds(16),
                MaxRetries = 5,
                Mode = RetryMode.Exponential
            }
        };
        _keyVaultClient = new SecretClient
        (
            new Uri(configuration.GetValue<string>("AppKeyVault:Endpoint")),
            azureCredentials,
            secretClientOptions
        );
    }

    public string Retrieve(string secretKey)
    {
        KeyVaultSecret secret = _keyVaultClient.GetSecret(secretKey);
        return secret.Value;
    }
}
