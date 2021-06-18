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
using Anti_Dox.CustomPreattributes;
using Discord.Net;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Anti_Dox.Services;


namespace Anti_Dox.Modules
{

    public class Commands : ModuleBase<SocketCommandContext>
    {


        private readonly EmbedBuilder _embed;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;
        public Commands(IServiceProvider services)
        {

            _config = services.GetRequiredService<IConfiguration>();
            _services = services;
            _embed = new EmbedBuilder();

        }
        //ping command
        [Command("ping", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task PingAsync()
        {
           //build embed get user name latency and send
            _embed.WithTitle($"Info for {Context.User.Username}");
            _embed.WithDescription($"{Context.Client.Latency} ms");
            _embed.WithColor(new Color(255, 255, 255));
            MessageReference msg = new MessageReference(messageId: Context.Message.Id);
            AllowedMentions allowed = new AllowedMentions(AllowedMentionTypes.None);
            await ReplyAsync("", false,_embed.Build(),null, allowed, msg).ConfigureAwait(false);



        }
        //set the bot status
        [Command("status")]
        [RequireTeamOwner]
        public async Task status([Remainder] string status = null)
        {
              //get the status string and set it.
                await Context.Client.SetGameAsync(status);
                //new embed builder and send what the bot status has been set to
                var embed = new EmbedBuilder()
                               .WithColor(Color.Green)
                               .WithTitle($"Status")
                               .WithDescription($"The bot status has been set to `playing {status}`");
                MessageReference msg = new MessageReference(messageId: Context.Message.Id);
                AllowedMentions allowed = new AllowedMentions(AllowedMentionTypes.None);
                await ReplyAsync("", false, _embed.Build(), null, allowed, msg).ConfigureAwait(false);

        }
        //set the bots prefix
        [Command("setprefix", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task setprefix(char prefix)
        {
            //say to use the CsharpiEntities database
            using (var db = new CsharpiEntities())
            {
                var currentPrefix = db.PrefixList.AsQueryable().Where(p => p.ServerId == (long)Context.Guild.Id).FirstOrDefault();
                if (currentPrefix != null)
                {
                    currentPrefix.Prefix = prefix;
                    currentPrefix.SetById = (long)Context.User.Id;
                }
                else
                {
                    db.PrefixList.Add(new PrefixList
                    {
                        ServerId = (long)Context.Guild.Id,
                        ServerName = Context.Guild.Name,
                        Prefix = prefix,
                        SetById = (long)Context.User.Id
                    });
                }
                await db.SaveChangesAsync();
            }
            await ReplyAsync($"Prefix for [**{Context.Guild.Name}**] changed to [**{prefix}**]");
        }

        [Command("Setchannel", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Channel(ITextChannel channel)
        {
            bool check = false;
            foreach(var Channel in Context.Guild.Channels)
            {
                var ChannelId = Channel.Id;
                if (channel.Id == ChannelId)
                {
                    check = true;
                }
            }
            if(check == true)
            {
                using (var db = new CsharpiEntities())
                {
                    var currentSettings = db.Settings.AsQueryable().Where(p => p.ServerId == (long)Context.Guild.Id).FirstOrDefault();
                    if (currentSettings != null)
                    {
                        currentSettings.LogsChannelId = (ulong)channel.Id;
                        currentSettings.SetById = (long)Context.User.Id;
                    }
                    else
                    {
                        db.Settings.Add(new Settings
                        {
                            LogsChannelId = channel.Id,
                            ServerId = (long)Context.Guild.Id,
                            ServerName = Context.Guild.Name,
                            Enabled = false,
                            SetById = (long)Context.User.Id,
                            Discordlogs = true,
                            punishment = 1

                        });
                    }
                    await db.SaveChangesAsync();
                }

                await ReplyAsync($"LogChannel for [**{Context.Guild.Name}**] Is now [**{channel}**]");
            }
            else
            {
                var embed = new EmbedBuilder()
                  .WithDescription("The channel provided is not in this guild")
                  .WithColor(Color.Red)
                  .WithTitle($"Error");
                MessageReference msg = new MessageReference(messageId: Context.Message.Id);
                AllowedMentions allowed = new AllowedMentions(AllowedMentionTypes.None);
                await ReplyAsync("", false, embed.Build(), null, allowed, msg).ConfigureAwait(false);
            }

        }
        [Command("punishment", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task punishment(string option)
        {
            int punishment = 1;
            if(option == "none")
            {
                punishment = 0;
                punish(punishment);
                await ReplyAsync($"Punishment for [**{Context.Guild.Name}**] Is now set to [**{option}**]");
            }
            else if(option == "mute")
            {
                punishment = 1;
                punish(punishment);
                await ReplyAsync($"Punishment for [**{Context.Guild.Name}**] Is now set to [**{option}**]");
            }
            else if(option == "ban")
            {
                punishment = 2;
                punish(punishment);
                await ReplyAsync($"Punishment for [**{Context.Guild.Name}**] Is now set to [**{option}**]");
            }
            else
            {
              await ReplyAsync("Wrong option choose between **none, mute, ban**");

            }

        }

        private async void punish(int punishment)
        {
            using (var db = new CsharpiEntities())
            {
                var currentSettings = db.Settings.AsQueryable().Where(p => p.ServerId == (long)Context.Guild.Id).FirstOrDefault();
                if (currentSettings != null)
                {
                    currentSettings.punishment = punishment;
                    currentSettings.SetById = (long)Context.User.Id;
                }
                else
                {
                    db.Settings.Add(new Settings
                    {
                        punishment = punishment,
                        ServerId = (long)Context.Guild.Id,
                        ServerName = Context.Guild.Name,
                        Enabled = false,
                        SetById = (long)Context.User.Id,
                        Discordlogs = false

                    }) ;
                }
                await db.SaveChangesAsync();
            }
        }
        [Command("unmute")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "You don't have the permission to unmute members!")]
        public async Task unmute(IGuildUser user)
        {
            var roleexists = false;
            ulong roleID = 0;
            foreach (var gRole in Context.Guild.Roles)
            {
                if (gRole.Name.Equals("MuteRole"))
                {
                    roleID = gRole.Id;
                    var role = Context.Guild.GetRole(roleID);
                    await user.RemoveRoleAsync(role);
                    roleexists = true;


                }
            }
            if (roleexists == false)
            {
                await Context.Message.Channel.SendMessageAsync("*ERROR* : Mute role doesn't exist");
            }

        }

        [Command("Case", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Case(int Case)
        {
            using (var db = new CsharpiEntities())
            {
                ulong vIn = Context.Guild.Id;
                long vOut = Convert.ToInt64(vIn);
                var logs = db.Logs.AsNoTracking().Where(p => p.ServerId == vOut).Where(n => n.ServerCase == Case).FirstOrDefaultAsync().Result;
                var User = new EmbedFieldBuilder()
                  .WithName("User")
                  .WithValue(logs.Username)
                  .WithIsInline(true);
                var ID = new EmbedFieldBuilder()
                    .WithName("ID")
                    .WithValue(logs.IpposterId)
                    .WithIsInline(true);
                var Time = new EmbedFieldBuilder()
                    .WithName("Time")
                    .WithValue(logs.Time)
                    .WithIsInline(true);
                var Punishment = new EmbedFieldBuilder()
                    .WithName("Punishment")
                    .WithValue(logs.Punishment)
                    .WithIsInline(false);
                var Message = new EmbedFieldBuilder()
                    .WithName("Message")
                    .WithValue(logs.MesasageContainingIp)
                    .WithIsInline(true);
                var embed = new EmbedBuilder()
                    .AddField(User)
                    .AddField(ID)
                    .AddField(Time)
                    .AddField(Punishment)
                    .AddField(Message)
                    .WithColor(Color.Red)
                    .WithTitle($"Case #{logs.ServerCase}");
                MessageReference msg = new MessageReference(messageId: Context.Message.Id);
                AllowedMentions allowed = new AllowedMentions(AllowedMentionTypes.None);
                await ReplyAsync("", false, embed.Build(), null, allowed, msg).ConfigureAwait(false);
            }
        }
        //makes the mute role
        [Command("muterole")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task muterole()
        {
            bool rExist = false;
            ulong roleID = 0;
            //Check if the role exist
            foreach (var gRole in Context.Guild.Roles)
            {
                if (gRole.Name.Equals("MuteRole"))
                {
                    rExist = true;
                    roleID = gRole.Id;
                }
                else
                {
                    continue;
                }
            }
            if (!rExist)
            {
                var color = (new Color(47, 49, 54));
                //if the roles doesnt exist u create it and set the perms of the channels
                var mRole = await Context.Guild.CreateRoleAsync(
                 "MuteRole", Discord.GuildPermissions.None, color/*what ever color*/, false, null);
                try
                {

                    foreach (var channel in Context.Guild.Channels)
                    {
                        await channel.AddPermissionOverwriteAsync(mRole,
                        OverwritePermissions.DenyAll(channel).Modify(
                        viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow)
                        );



                    }
                    var embed = new EmbedBuilder()
                                  .WithDescription("I Have made the Muterole Muted")
                                  .WithColor(Color.Red)
                                  .WithTitle($"mute role");
                    MessageReference msg = new MessageReference(messageId: Context.Message.Id);
                    AllowedMentions allowed = new AllowedMentions(AllowedMentionTypes.None);
                    await ReplyAsync("", false, embed.Build(), null, allowed, msg).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //handel error if occures
                    var embed = new EmbedBuilder()
                                  .WithDescription("Something Went wrong do i have the correct permissions?")
                                  .WithColor(Color.Red)
                                  .WithTitle($"mute role");

                    await ReplyAsync(embed: embed.Build());
                }
            }
            else
            {
                var embed = new EmbedBuilder()
                                  .WithDescription("I Already have the mute role.")
                                  .WithColor(Color.Red)
                                  .WithTitle($"mute role");

                await ReplyAsync(embed: embed.Build());
            }

        }
        [Command("ListCases", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task ListCases()
        {
            using (var db = new CsharpiEntities())
            {
                var sb = new StringBuilder();
                var embed = new EmbedBuilder();
                ulong vIn = Context.Guild.Id;
                long vOut = Convert.ToInt64(vIn);
                var logs = db.Logs.AsNoTracking().Where(p => p.ServerId == vOut).ToListAsync().Result;
                double count = 0;
                foreach (var ServerCase in logs)
                {

                    sb.AppendLine($"Case #{count}.");
                    count = count + 1;
                }
                embed.Title = "List of cases";
                embed.Description = sb.ToString();

                // send embed reply
                MessageReference msg = new MessageReference(messageId: Context.Message.Id);
                AllowedMentions allowed = new AllowedMentions(AllowedMentionTypes.None);
                await ReplyAsync("", false, embed.Build(), null, allowed, msg).ConfigureAwait(false);
            }
        }
        [Command("Help", RunMode = RunMode.Async)]
        [Alias("h")]
        public async Task Help()
        {
            var Help = new EmbedFieldBuilder()
                  .WithName("Help")
                  .WithValue("Help  = running this command brings up the help menu")
                .WithIsInline(false);
            var Ping = new EmbedFieldBuilder()
                  .WithName("Ping")
                  .WithValue("Ping = check if the bot is online")
                 .WithIsInline(false); ;
            var SetPrefix = new EmbedFieldBuilder()
                  .WithName("Setprefix")
                  .WithValue("Setprefix (prefix) = change the bot's prefix")
                .WithIsInline(false);
            var Enable = new EmbedFieldBuilder()
                  .WithName("Enable")
                  .WithValue("Enable = enable bot's ip deletion module")
                 .WithIsInline(false);
             var Disable = new EmbedFieldBuilder()
                  .WithName("Disable")
                  .WithValue("Disable = Disable bot's ip deletion module")
                 .WithIsInline(false);
            var setchannel = new EmbedFieldBuilder()
                  .WithName("Setchannel")
                  .WithValue("Setchannel (channel) = set a channel where the logs appear")
                 .WithIsInline(false);
            var Punishment = new EmbedFieldBuilder()
                  .WithName("Punishment")
                  .WithValue("Punishment (none/mute/ban) = set how the bot needs to punish somebody when they post an ip")
                  .WithIsInline(false);
            var Unmute = new EmbedFieldBuilder()
                  .WithName("unmute")
                  .WithValue("Unmute (user) = unmute a user that has been muted by the bot")
                  .WithIsInline(false);
            var Case = new EmbedFieldBuilder()
                   .WithName("Case")
                   .WithValue("Case (casenummer) = show's a old case")
                   .WithIsInline(false);
            var Listcases = new EmbedFieldBuilder()
                   .WithName("Listcases")
                   .WithValue("Listcases = show a list of all cases that happend in the guild")
                   .WithIsInline(false);
            var Muterole = new EmbedFieldBuilder()
                   .WithName("Muterole")
                   .WithValue("Muterole = make the mute role that the bot uses")
                   .WithIsInline(false);
            var DisableDiscordlogs = new EmbedFieldBuilder()
                   .WithName("DisableDiscordlogs")
                   .WithValue("DisableDiscordlogs = disable posting a log message if somebody posts a ip they will still be stored in the database")
                   .WithIsInline(false);
            var embed = new EmbedBuilder()
                    .AddField(Help)
                    .AddField(Ping)
                    .AddField(SetPrefix)
                    .AddField(Enable)
                    .AddField(setchannel)
                    .AddField(Punishment)
                    .AddField(Unmute)
                    .AddField(Case)
                    .AddField(Listcases)
                    .AddField(Muterole)
                    .AddField(DisableDiscordlogs)
                    .WithColor(Color.Green)
                    .WithTitle($"Help menu");
            MessageReference msg = new MessageReference(messageId: Context.Message.Id);
            AllowedMentions allowed = new AllowedMentions(AllowedMentionTypes.None);
            await ReplyAsync("", false, embed.Build(), null, allowed, msg).ConfigureAwait(false);



        }
        [Command("Disablediscordlogs", RunMode = RunMode.Async)]
        [Summary("Disables DiscordLogs")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Disablediscordlogs()
        {
            using (var db = new CsharpiEntities())
            {
                var currentSettings = db.Settings.AsQueryable().Where(p => p.ServerId == (long)Context.Guild.Id).FirstOrDefault();
                if (currentSettings != null)
                {
                    currentSettings.Discordlogs = false;
                    currentSettings.SetById = (long)Context.User.Id;
                }
                else
                {
                    db.Settings.Add(new Settings
                    {
                        punishment = 2,
                        ServerId = (long)Context.Guild.Id,
                        ServerName = Context.Guild.Name,
                        Enabled = false,
                        SetById = (long)Context.User.Id,
                        Discordlogs = false

                    });
                }
                await db.SaveChangesAsync();
                await ReplyAsync($"LogChannel for [**{Context.Guild.Name}**] Is now [**Disabled**]");
            }
        }
        [Command("Enable", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Enable()
        {
            using (var db = new CsharpiEntities())
            {
                var currentSettings = db.Settings.AsQueryable().Where(p => p.ServerId == (long)Context.Guild.Id).FirstOrDefault();
                if (currentSettings != null)
                {
                    currentSettings.Enabled = true;
                    currentSettings.SetById = (long)Context.User.Id;
                }
                else
                {
                    db.Settings.Add(new Settings
                    {
                        Enabled = true,
                        ServerId = (long)Context.Guild.Id,
                        ServerName = Context.Guild.Name,
                        SetById = (long)Context.User.Id,
                        punishment = 1,
                        Discordlogs = false
                    });
                }
                await db.SaveChangesAsync();
            }

            await ReplyAsync($"Anti-Dox for [**{Context.Guild.Name}**] Is now [**enabled**]");
        }
        [Command("Disable", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Disable()
        {
            using (var db = new CsharpiEntities())
            {
                var currentSettings = db.Settings.AsQueryable().Where(p => p.ServerId == (long)Context.Guild.Id).FirstOrDefault();
                if (currentSettings != null)
                {
                    currentSettings.Enabled = false;
                    currentSettings.SetById = (long)Context.User.Id;
                }
                else
                {
                    db.Settings.Add(new Settings
                    {
                        Enabled = false,
                        ServerId = (long)Context.Guild.Id,
                        ServerName = Context.Guild.Name,
                        SetById = (long)Context.User.Id,
                        punishment = 1,
                        Discordlogs = false
                    });
                }
                await db.SaveChangesAsync();
            }

            await ReplyAsync($"Anti-Dox for [**{Context.Guild.Name}**] Is now [**Disabled**]");
        }
        [Command("stats")]
        public async Task stats()
        {

            if (Context.User.Id is 652248874873782272)
            {
                string RAM = Info.Ram;
                string CPU = Info.Cpu;


                await ReplyAsync($"Ram usage is {RAM} and the cpu usage is {CPU}");
            }
            else
            {
                await ReplyAsync("This message can only be run by the owner of the bot");
            }

        }
    }
}