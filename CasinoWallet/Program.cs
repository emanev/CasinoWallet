using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CasinoWallet.Interfaces;
using CasinoWallet.Services;
using CasinoWallet.Configuration;
using Serilog;
using CasinoWallet.Domain;

namespace CasinoWallet
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(config);
            var settings = config.GetSection("GameSettings").Get<GameSettings>();
            GameSettingsValidator.Validate(settings);

            services.AddSingleton(settings);

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            });

            services.AddTransient<IWallet, Wallet>();
            services.AddTransient<IGame, Game>();
            services.AddSingleton<IGameService, GameService>();
            services.AddSingleton<IConsoleService, ConsoleService>();
            services.AddTransient<CommandHandler>();

            var provider = services.BuildServiceProvider();

            var handler = provider.GetRequiredService<CommandHandler>();
            var logger = provider.GetRequiredService<ILogger<Program>>();

            try
            {
                while (true)
                {
                    Console.WriteLine("Please, submit action:");
                    var input = Console.ReadLine()?.Trim().ToLower();

                    if (input == null)
                    {
                        continue;
                    }
                    if (!handler.Handle(input))
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}