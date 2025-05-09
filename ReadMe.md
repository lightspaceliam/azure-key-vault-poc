# Azure Key Vault POC

This POC is to showcase how we can conveniently and securely distribute multiple application secrets required by one or more applications for one or more developers.

When the team is small it is clearly more convenient to mitigate this use case to: Environment variables. User Secrets or even a .env file however, best practice is to implement some sort of secure distribution service such as Azure Key Vault.

Covered in this POC is the well documented and easy to find **Default Credential** flow and the potentially not so easy to find **Client Credentials - App Registration** flow. Keeping in mind, the purpose of this POC is to demonstrate access for local development but also facilitate further configuration outside local development.  

## Client Credentials - App Registration

1. Register an App in App Registrations
2. Create a Key Vault
3. Add secrets
4. Register the App in the key vault
5. Add configuration to the subscribing application/s

By doing so we have mitigated access to the key vault to the registered app. Azure still provides additional access controls but thats out of scope for this POC.

Required configurations:

- Uri - Key Vault name
- TenantId Azure tenant id
- ClientId registered application in Azure's App registrations
- ClientSecret  App registrations / {registered-app} / Manage / Certificate & secrets

This will provide access to one or many secrets without the need to distribute via plain text. To add to this, if required, we can simply updated the `ClientSecret` if access needs to be updated.

All that is required is to configure these values in your configuration file:

```json
{
  "AzureKeyVault": {
    "Uri": "in-user-secrets-for-this-poc",
    "TenantId": "in-user-secrets-for-this-poc",
    "ClientId": "in-user-secrets-for-this-poc",
    "ClientSecret": "in-user-secrets-for-this-poc"
  }
}
```

Keys are only accessible to users who have access to the Key Vault.

Depending on the composition of your code base, I would highly recommend you create a service to promote reuse and then make the request for individual secrets by key:

```c#

var azureKeyVaultService = host.Services.GetRequiredService<AzureKeyVaultService>();

Console.WriteLine($"POC Secret 1: {await azureKeyVaultService.GetSecretAsync("Key-One")}");
Console.WriteLine($"POC Secret 2: {await azureKeyVaultService.GetSecretAsync("Key-Two")}");

```

## Default Azure Credential Flow

1. Create the Azure Key Vault
2. Add secrets
3. Add configuration to the subscribing application/s
4. Install and run az CLI
5. Provide access to all developers
    - Azure Portal Access
    - Key Vault Access

**Azure CLI:**

```bash
# 1. Install az cli in PowerShell
winget install --exact --id Microsoft.AzureCLI

# 2. Log in
az login

# 3. On Windows, there will be a popup window for you to login to the Portal. I have not tried on macOS

# 4. After a successful login, your this application will work as expected 
```

## Conclusion

Both strategies provide an excellent way to securely distribute application secrets. Here is a list of concepts to consider:

**Default Azure Credential Flow**
1. Requires the developer to have more granular access to both the Azure Portal and the targeted Key Vault.
2. Provides more granular security especially for offboarding access
3. For local development, requires the developer to install and run the `az CLI` client
4. Requires only the Key Vault name to be stored in the source code 

**Client Credentials - App Registration Flow**
1. Requires more configuration to be stored in the source code:
    - Key Vault name
    - Registered App ClientId and ClientSecret
    - TenantId
2. No access to the portal or the key vault
3. Simplified developer experience as they do not need to run the `az CLI` client for authentication

## References

- [Tutorial: Use a managed identity to connect Key Vault to an Azure web app in .NET](https://learn.microsoft.com/en-us/azure/key-vault/general/tutorial-net-create-vault-azure-web-app?tabs=azure-cli)
- [Install Azure CLI on Windows](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows?pivots=winget)
- [Fetching Secrets From Key Vault In .NET Console App](https://www.c-sharpcorner.com/blogs/fetching-secrets-from-key-vault-in-net-console-app)