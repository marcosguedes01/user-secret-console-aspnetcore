using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1
{
    //https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows
    //https://www.twilio.com/blog/2018/05/user-secrets-in-a-net-core-console-app.html

    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            Startup();
        }

        private static void Startup()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            // tell the builder to look for the appsettings.json file
            builder
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) || devEnvironmentVariable.ToLower() == "development";

            //only add secrets in development
            if (isDevelopment)
            {
                //builder.AddUserSecrets<Program>();
            }

            Configuration = builder.Build();

            IServiceCollection services = new ServiceCollection();

            //Map the implementations of your classes here ready for DI
            services
                .Configure<Movies>(Configuration.GetSection(nameof(Movies)))
                .AddOptions()
                //.AddLogging()
                .AddSingleton<ISecretMovies, SecretMovies>()
                .BuildServiceProvider();

            var serviceProvider = services.BuildServiceProvider();

            // Get the service you need - DI will handle any dependencies - in this case IOptions<SecretStuff>
            var revealer = serviceProvider.GetService<ISecretMovies>();

            revealer.Reveal();

            Console.ReadKey();
        }
    }
}
