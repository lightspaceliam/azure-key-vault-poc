# Azure Key Vault POC

This POC is to showcase how we can distribute multiple application secrets required by one or more applications for one or more developers.

When there is just a couple of developers, it could be acceptable to store these secrets in .NET's User Secrets or an .env file however, as soon as the team grows, this becomes more challenging. Without implementing some sort of secrets management tool, the secret/s would be stored in plain text on each developers workstation.

By implementing Azure Key Vault all I require is the following configurations:

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

Depending on the composition of your code base, I would highly recommend you create a service to promote reuse and then make the request for individual secrets by key:

```c#

var azureKeyVaultService = host.Services.GetRequiredService<AzureKeyVaultService>();

Console.WriteLine($"POC Secret 1: {await azureKeyVaultService.GetSecretAsync("Key-One")}");

```