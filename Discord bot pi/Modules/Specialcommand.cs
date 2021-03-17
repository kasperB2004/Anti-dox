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
using Discord_bot_pi.Database;
using Discord.WebSocket;
using RestSharp;
using Discord.Net;
namespace Discord_bot_pi.Modules
{
    public class SpecialCommand : ModuleBase
    {

        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;
        public SpecialCommand(IServiceProvider services)
        {
            _config = services.GetRequiredService<IConfiguration>();
            _services = services;
        }
        [Command("numservers")]
        public async Task GetNumGuilds()
        {

            if (Context.User.Id is 652248874873782272)
            {
                var numGuilds = await Context.Client.GetGuildsAsync();
                await ReplyAsync($"I am connected to {numGuilds.Count()} guilds!");
            }
            else
            {
                await ReplyAsync("This message can only be run by the owner of the bot");
            }

        }
        [Command("Notimplemented")]
        public async Task Stats()
        {
            if (Context.User.Id is 652248874873782272)
            {

                const string ClientSecret = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0IjoxLCJpZCI6Ijc5MTk2NDY0MjIzNDQ2NjMxNiIsImlhdCI6MTYxNDYyNDE0OX0.S7YA0cCywPtJGKNNpwcXA6azZh-O2Rrh7uQl3OfkaIA";

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
                request.AddHeader("Authorization", ClientSecret);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Cookie", "__cfduid=d166f01e6e4f0e5742b98c1f5c6def1001614624469");
                request.AddParameter("guilds", numGuilds.Count);
                request.AddParameter("users", totalUsers);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                await ReplyAsync($"I am connected to {numGuilds.Count()} guilds! and {totalUsers} Users use me !");
            }
            else
            {
                await ReplyAsync("This message can only be run by the owner of the bot");
            }
        }

    }
}
