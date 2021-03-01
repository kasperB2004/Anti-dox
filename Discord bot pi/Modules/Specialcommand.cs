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

    }
}
