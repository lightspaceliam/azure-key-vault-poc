using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyVault_POC.Services
{
    public class AzureKeyVaultService(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly string _clientId = configuration["AzureKeyVault:ClientId"];
        private readonly string _clientSecret = configuration["AzureKeyVault:ClientSecret"];
        private readonly string _tenantId = configuration["AzureKeyVault:TenantId"];
        private readonly Uri _keyVaultUri = new Uri($"https://{configuration["AzureKeyVault:Uri"]}.vault.azure.net/");

        /// <summary>
        /// Gets an Azure Key Vault secret by the specified Key. Implements Client Credential Flow, not tied to an individual user's credentials.
        /// </summary>
        /// <param name="key">The specific secret you require access to</param>
        /// <returns>Nullable string</returns>
        public async Task<string?> GetSecretAsync(string key)
        {
            var credential = new ClientSecretCredential(_tenantId, _clientId, _clientSecret);
            var secretClient = new SecretClient(_keyVaultUri, credential);

            var keyVaultSecret = (await secretClient.GetSecretAsync(key)).Value;

            return keyVaultSecret?.Value;
        }
    }
}
