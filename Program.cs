// See https://aka.ms/new-console-template for more information

/*
 * Refs: 
 * https://www.c-sharpcorner.com/blogs/fetching-secrets-from-key-vault-in-net-console-app
 * A bit of Copilot help 
 */
using KeyVault_POC.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // I can read both, environment variables and user secrets without enabling these two:
        config.AddEnvironmentVariables();
        config.AddUserSecrets<Program>();

        var environment = context.HostingEnvironment;
        Console.WriteLine($"Environment: {environment.EnvironmentName}");
        if (environment.IsDevelopment())
        {
            Console.WriteLine("For when you run from the IDE - Development environment");
            var projectPath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName).FullName;
            config.SetBasePath(projectPath);
        }
        else
        {
            Console.WriteLine("For when you run from the CLI - Not development environment");
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

Console.WriteLine("\nAzure Key Vault - secrets\n");
Console.WriteLine($"POC Secret 1: {await azureKeyVaultService.GetSecretAsync("Key-One")}");
Console.WriteLine($"POC Secret 2: {await azureKeyVaultService.GetSecretAsync("Key-Two")}");

Console.WriteLine("\nHello, Azure Key Vault POC!");
Console.ReadKey();