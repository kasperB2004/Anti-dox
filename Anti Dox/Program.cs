using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Anti_Dox.Database;
using Anti_Dox.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.API;
using Discord.Addons.Interactive;
using Anti_Dox.dependencies;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog.Sinks.SystemConsole;

namespace Anti_Dox


{
    internal class Program
    {
        // setup our fields we assign later
        private readonly IConfiguration _config;

        private DiscordSocketClient _client;
        private static string _logLevel;



        private static void Main(string[] args = null)
        {
            if (args.Count() != 0)
            {
                _logLevel = args[0];
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/csharpi.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Console(theme: Serilogtheme.DiscordMatrix)
                .CreateLogger();

            Console.Clear();
            Console.Title = "Anti-Dox";
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {

            // create the configuration
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");

            // build the configuration and assign to _config
            _config = _builder.Build();
        }

        public async Task MainAsync()
        {
            //clear console
            Console.Clear();
            // call ConfigureServices to create the ServiceCollection/Provider for passing around the services
            using (var services = ConfigureServices())
            {
                // get the client and assign to client
                // you get the services via GetRequiredService<T>
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;

                // setup logging and the ready event
                services.GetRequiredService<LoggingService>();
                services.GetRequiredService<DatabaseService>();
                services.GetRequiredService<Info>();
                services.GetRequiredService<MessageCheck>();

                // this is where we get the Token value from the configuration file, and start the bot
                await client.LoginAsync(TokenType.Bot, _config["Token"]);
                await client.StartAsync();

                // we get the CommandHandler class here and call the InitializeAsync method to start things up for the CommandHandler service
                await services.GetRequiredService<CommandHandler>().InitializeAsync();


                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine("Connected as -> [{Username}] ", _client.CurrentUser.Username.ToString());


            return Task.CompletedTask;
        }

        // this method handles the ServiceCollection creation/configuration, and builds out the service provider we can call on later
        private ServiceProvider ConfigureServices()
        {
            // this returns a ServiceProvider that is used later to call for those services
            // we can add types we have access to here, hence adding the new using statement:
            // using csharpi.Services;
            // the Config we build is also added, which comes in handy for setting the command prefix!
            var services = new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(new CommandService(new CommandServiceConfig
                 {
                     DefaultRunMode = RunMode.Async,
                     CaseSensitiveCommands = false,
                     ThrowOnError = false
                }))
                .AddSingleton<CommandHandler>()
                .AddSingleton<LoggingService>()
                .AddSingleton<DatabaseService>()
                .AddSingleton<Info>()
                .AddSingleton<MessageCheck>()
                .AddSingleton<InteractiveService>()
                .AddDbContext<CsharpiEntities>()

                .AddLogging(configure => configure.AddSerilog());

            if (!string.IsNullOrEmpty(_logLevel))
            {
                switch (_logLevel.ToLower())
                {
                    case "info":
                        {
                            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
                            break;
                        }
                    case "error":
                        {
                            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                            break;
                        }
                    case "debug":
                        {
                            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
                            break;
                        }
                    default:
                        {
                            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                            break;
                        }
                }
            }
            else
            {
                services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
            }

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}