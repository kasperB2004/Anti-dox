using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using RestSharp;
using System.Text;
using Discord_bot_pi.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using System.Reflection;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Discord_bot_pi.Database;
using System.Net.Http.Headers;
using Discord.Net;
using Microsoft.EntityFrameworkCore;

namespace Discord_bot_pi.Services
{
    public class DatabaseService
    {

        // declare the fields used later in this class
        private readonly ILogger _logger;
        private DiscordSocketClient _client;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;


        public DatabaseService(IServiceProvider services)
        {
            // get the services we need via DI, and assign the fields declared above to them
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            _logger = services.GetRequiredService<ILogger<LoggingService>>();
            _config = services.GetRequiredService<IConfiguration>();
            var client = services.GetRequiredService<DiscordSocketClient>();
            _client = client;

            // hook into these events with the methods provided below
            _discord.Ready += OnReadyAsync;
            _discord.JoinedGuild += OnJoinAsync;

        }
        //log that the module is loaded
        public Task OnReadyAsync()
        {
            _logger.LogInformation($"DatabaseService Module loaded");
            return Task.CompletedTask;

        }
        public async Task<Task> OnJoinAsync(SocketGuild guild)
        {

            /*const string ClientSecret = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0IjoxLCJpZCI6Ijc5MTk2NDY0MjIzNDQ2NjMxNiIsImlhdCI6MTYxNDYyNDE0OX0.S7YA0cCywPtJGKNNpwcXA6azZh-O2Rrh7uQl3OfkaIA";

            var numGuilds = _client.Guilds.ToList(); ;
            int totalUsers = 0;
            foreach (var guilds in numGuilds)
            {
                var users = await  _client.Rest.GetUserAsync
            }
            var client = new RestClient("https://discordbotlist.com/api/v1/bots/791964642234466316/stats?id=791964642234466316");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", ClientSecret);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Cookie", "__cfduid=d166f01e6e4f0e5742b98c1f5c6def1001614624469");
            request.AddParameter("guilds", _discord.Guilds.Count);
            request.AddParameter("users", "");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);*/


            //make database on join
            using (var db = new CsharpiEntities())
            {
                var currentSettings = db.Settings.AsQueryable().Where(p => p.ServerId == (long)guild.Id).FirstOrDefault();
                if (currentSettings != null)
                {
                    return Task.CompletedTask;
                }
                else
                {
                    db.Settings.Add(new Settings
                    {
                        LogsChannelId = 0,
                        ServerId = (long)guild.Id,
                        ServerName = guild.Name,
                        Enabled = false,
                        SetById = (long)0,
                        Discordlogs = true,
                        punishment = 1

                    });
                }
                await db.SaveChangesAsync();
            }
            //return task is completed
            return Task.CompletedTask;
        }



    }
}
