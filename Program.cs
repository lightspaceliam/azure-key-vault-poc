using KeyVault_POC.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddEnvironmentVariables();
        config.AddUserSecrets<Program>();

        var environment = context.HostingEnvironment;

        if (environment.IsDevelopment())
        {
            var projectPath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName).FullName;
            config.SetBasePath(projectPath);
        }
        else
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
        }

        //  Pick up configuration settings.
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging();

        services.AddTransient<AzureKeyVaultService>();
    })
    .Build();

var azureKeyVaultService = host.Services.GetRequiredService<AzureKeyVaultService>();
var configuration = host.Services.GetRequiredService<IConfiguration>();

Console.WriteLine("\nAzure Key Vault - secrets\n");
Console.WriteLine($"POC Secret 1: {await azureKeyVaultService.GetSecretAsync("Key-One")}");
Console.WriteLine($"POC Secret 2: {await azureKeyVaultService.GetSecretAsync("Key-Two")}");

//  Key is obscured as this is a public repo. 
Console.WriteLine($"POC Secret 3: {await azureKeyVaultService.GetSecretDefaultAzureCredentialsAsync(configuration["DefaultAzyreCredentialAzureKeyVault:Key"])}");

Console.WriteLine("\nHello, Azure Key Vault POC!");
Console.ReadKey();