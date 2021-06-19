using Discord;
using Discord.Commands;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Anti_Dox.Database;
using Discord.WebSocket;
using RestSharp;
using Discord.Net;
using Anti_Dox.CustomPreattributes;
using System.Timers;
namespace Anti_Dox.Modules
{
    public class OwnerCommands : ModuleBase
    {

        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;
        private int shutdownseconds;
        private int Restartseconds;
        private Timer aTimer;
        private Timer aTimer2;

        public OwnerCommands(IServiceProvider services)
        {
            _config = services.GetRequiredService<IConfiguration>();
            _services = services;
            aTimer = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += OnTimedEventAsync;
            //auto reset
            aTimer.AutoReset = true;
            aTimer2 = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer.
            aTimer2.Elapsed += ATimer2_Elapsed; ;
            //auto reset
            aTimer2.AutoReset = true;
        }

        private void ATimer2_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(Restartseconds == 5)
            {
                // Starts a new instance of the program itself
                System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.FriendlyName);
                // Closes the current process
                Environment.Exit(0);
            }
            Restartseconds++;
        }

        private void OnTimedEventAsync(object sender, ElapsedEventArgs e)
        {

            if (shutdownseconds == 5)
            {
                Environment.Exit(0);
            }
            shutdownseconds++;
        }

        [Command("numservers")]
        [RequireTeamOwner]
        public async Task GetNumGuilds()
        {
                var numGuilds = await Context.Client.GetGuildsAsync();
                await ReplyAsync($"I am connected to {numGuilds.Count()} guilds!");
        }
        [Command("NumUsers")]
        [RequireTeamOwner]
        public async Task Stats()
        {

                var numGuilds = await Context.Client.GetGuildsAsync();
                int totalUsers = 0;
                foreach (var guild in numGuilds)
                {
                    var members = await guild.GetUsersAsync();
                    totalUsers += members.Count;
                }
                var client = new RestClient("https://discordbotlist.com/api/v1/bots/791964642234466316/stats?id=791964642234466316");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", _config["ClientSecret"]);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Cookie", "__cfduid=d166f01e6e4f0e5742b98c1f5c6def1001614624469");
                request.AddParameter("guilds", numGuilds.Count);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                await ReplyAsync($"I am connected to {numGuilds.Count()} guilds! and {totalUsers} Users use me !");
        }
        [Command("Restart")]
        [RequireTeamOwner]
        public async Task restart()
        {
            MessageReference msg = new MessageReference(messageId: Context.Message.Id);
            AllowedMentions allowed = new AllowedMentions(AllowedMentionTypes.None);
            await ReplyAsync("**Restarting in 5 seconds**", allowedMentions: allowed, messageReference: msg).ConfigureAwait(false);
            aTimer2.Start();
        }
        [Command("shutdown")]
        [RequireTeamOwner]
        public async Task shutdown()
        {
            MessageReference msg = new MessageReference(messageId: Context.Message.Id);
            AllowedMentions allowed = new AllowedMentions(AllowedMentionTypes.None);
            await ReplyAsync("**Shutting Down**", allowedMentions: allowed, messageReference: msg).ConfigureAwait(false);
            aTimer.Start();
        }
    }
}
