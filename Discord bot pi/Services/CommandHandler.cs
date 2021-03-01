
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord_bot_pi.Database;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.RegularExpressions;


namespace Discord_bot_pi.Services
{
    public class CommandHandler
    {
        
        // setup fields to be set later in the constructor
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly CsharpiEntities _db;
        private readonly IServiceProvider _services;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public CommandHandler(IServiceProvider services)
        {
            // juice up the fields with these services
            // since we passed the services in, we can use GetRequiredService to pass them into the fields set earlier
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _db = services.GetRequiredService<CsharpiEntities>();
            _logger = services.GetRequiredService<ILogger<CommandHandler>>();
            _services = services;
            // take action when we execute a command
            _commands.CommandExecuted += CommandExecutedAsync;
            // take action when we receive a message (so we can process it, and see if it is a valid command)
            _client.MessageReceived += MessageReceivedAsync;

        }
        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
        // this class is where the magic starts, and takes actions upon receiving messages
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {

            // ensures we don't process system/other bot messages
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
            }
            if (message.Channel is IPrivateChannel)
            {
                return;
            }
            var context = new SocketCommandContext(_client, message);
            var IsEnabled = GetEnabled((long)context.Guild.Id);
            bool Enabled = false;
            if (IsEnabled != null)
            {
                Enabled = IsEnabled.Enabled;
            }
            if (Enabled is true)    
            {
                String theEmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                   + "@"
                                   + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z";
                var message2 = rawMessage.Content;
                var match = Regex.Match(message2, @"\b(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\b");
                var match2 = Regex.Match(message2, @"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))");
                var dotdot = Regex.Match(message2, @"\::");
                var doxbin = Regex.Match(message2, @"https:\/\/doxbin\.org");
                var email = Regex.Match(message2, theEmailPattern);
               /* var phone = Regex.Match(message2, @"(\+[0-9]{2}|\+[0-9]{2}\(0\)|\(\+[0-9]{2}\)\(0\)|00[0-9]{2}|0)([0-9]{9}|[0-9\-\s]{9,18})");
                string phonenumber2 = "";
                if (phone.Success)
                {
                    string[] numbers = Regex.Split(message2, @"\D+");
                    foreach (var phonenumber in numbers)
                    {
                        int number;
                        if (int.TryParse(phonenumber, out number))
                        {
                            phonenumber2 = phonenumber2 + number;
                        }
                    }
                    Console.WriteLine(phonenumber2);
                }*/
               

                if (match.Success || match2.Success || doxbin.Success || email.Success /* || phone.Success */)
                {
                    if (dotdot.Success)
                    {
                       
                    }
                    else
                    {
                        string ip = rawMessage.Content;
                        var author = rawMessage.Author;
                        var chnl = message.Channel as SocketGuildChannel;
                        var Guild = chnl.Guild.Id;
                        var sendin = rawMessage.Channel;
                        var authorid = rawMessage.Author.Id;
                        var Time = rawMessage.CreatedAt;
                        var Punishment = Getpunishment((long)context.Guild.Id);
                        int punish = Punishment.punishment;
                        IGuildUser user = (IGuildUser)rawMessage.Author;
                        IGuild guild = user.Guild;
                        ISocketMessageChannel messageChannel = rawMessage.Channel;

                        if (punish == 1)
                        {
                            try
                            {
                                bool rExist = false;
                                ulong roleID = 0;
                                //Check if the role exist
                                foreach (var gRole in user.Guild.Roles)
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
                                    var embed = new EmbedBuilder()
                                         .WithDescription("I dont have the mute role. Run the command  [Muterole] to use this feature")
                                         .WithColor(Color.Red)
                                         .WithTitle($"No mute role");
                                    await messageChannel.TriggerTypingAsync();
                                    await messageChannel.SendMessageAsync(embed: embed.Build());
                                }
                                else
                                {
                                    //if it exist just add it to the user
                                    var role = user.Guild.GetRole(roleID);
                                    await user.AddRoleAsync(role);
                                    //Check if the role is added to every channel. Same as above
                                }
                            }
                            catch
                            {
                                var embed = new EmbedBuilder()
                                     .WithDescription("Something went wrong. Do i have the correct permissions?")
                                     .WithColor(Color.Red)
                                     .WithTitle($"Can't Mute");
                                await messageChannel.TriggerTypingAsync();
                                await messageChannel.SendMessageAsync(embed: embed.Build());
                            }

                        }
                        if (punish == 2)
                        {
                            try
                            {
                                int pruneDays = 0;
                                string Reason = "User posted an ip";
                                await user.BanAsync(pruneDays, Reason);
                            }
                            catch
                            {

                                var embed = new EmbedBuilder()
                                     .WithDescription("Something went wrong. Do i have the correct permissions?")
                                     .WithColor(Color.Red)
                                     .WithTitle($"Can't Ban");
                                await messageChannel.TriggerTypingAsync();
                                await messageChannel.SendMessageAsync(embed: embed.Build());
                            }
                        }

                        await rawMessage.DeleteAsync();
                        var warning = new EmbedBuilder()
                            .WithTitle("Naughty")
                            .WithDescription($"U can't post that here {author}")
                            .WithColor(Color.Red);
                        await messageChannel.SendMessageAsync(embed: warning.Build());

                        logs((SocketGuildUser)author, ip, Time, authorid, Guild, sendin, punish);
                    }
                }
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }
            // sets the argument position away from the prefix we set
            var argPos = 0;
            // get prefix from the configuration file
            char prefix = Char.Parse(_config["Prefix"]);
            var serverPrefix = GetPrefix((long)context.Guild.Id);
            if (serverPrefix != null)
            {
                prefix = serverPrefix.Prefix;

            }
            // determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasCharPrefix(prefix, ref argPos)))
            {
                return;
            }
            // execute command if one is found that matches
            await _commands.ExecuteAsync(context, argPos, _services);

        }
        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // if a command isn't found, log that info to console and exit this method
            if (!command.IsSpecified)
            {
                _logger.LogError($"Command failed to execute for [{context.User.Username}] <-> [{result.ErrorReason}]!");
                string vIn = _config["logs"];
                ulong vOut = Convert.ToUInt64(vIn);

                var logchannel = _client.GetChannel(vOut) as IMessageChannel;
               

                return;
            }
            // log success to the console and exit this method
            if (result.IsSuccess)
            {
                _logger.LogInformation($"Command [{command.Value.Name}] executed for [{context.User.Username}] on [{context.Guild.Name}]");
                string vIn = _config["logs"];
                ulong vOut = Convert.ToUInt64(vIn);


                return;

            }

            // failure scenario, let's let the user know
            await context.Channel.SendMessageAsync($"Sorry, {context.User.Username}... something went wrong -> [{result}]!");
        }
        private PrefixList GetPrefix(long serverId)
        {
            PrefixList prefix = null;

            using (var db = new CsharpiEntities())
            {
                prefix = db.PrefixList.AsNoTracking().Where(p => p.ServerId == serverId).FirstOrDefaultAsync().Result;

            }

            return prefix;
        }


        private async void logs(SocketGuildUser author, string ip, DateTimeOffset Time, ulong authorid, ulong Guild, ISocketMessageChannel sendin, int punish)
        {
            double ServercaseNummer;
            string punishment = "";
            if (punish == 0)
            {
                punishment = "none";
            }
            if (punish == 1)
            {
                punishment = "Muted";
            }
            if (punish == 2)
            {
                punishment = "Banned";
            }
            using (var db = new CsharpiEntities())
            {

                ulong vIn = Guild;
                long vOut = Convert.ToInt64(vIn);
                double GlobalcaseNummer;
                var Globalcase = GetglobalCase();
                if(Globalcase != null)
                {
                     GlobalcaseNummer = Globalcase.GlobalCase;
                    GlobalcaseNummer = GlobalcaseNummer + 1;
                }
                else
                {
                    GlobalcaseNummer = 0;
                }

                var Servercase = GetServerCase(Guild);
                if (Servercase != null)
                {
                    ServercaseNummer = Servercase.ServerCase;
                    ServercaseNummer = ServercaseNummer + 1;
                }
                else
                {
                    ServercaseNummer = 0;
                }

                db.Logs.Add(new logs
                {
                    GlobalCase = GlobalcaseNummer,
                    ServerCase = ServercaseNummer,
                    Time = Convert.ToString(Time),
                    Username = Convert.ToString(author),
                    ServerId = vOut,
                    IpposterId = Convert.ToString(authorid),
                    MesasageContainingIp = ip,
                    Punishment = punishment


                });
                await db.SaveChangesAsync();


            }
            DiscordLogs((SocketGuildUser)author, authorid, Time, sendin, Guild, ip, punishment, ServercaseNummer);
            return;
        }

        private logs GetServerCase(ulong guild)
        {
            using (var db = new CsharpiEntities())
            {
                ulong vIn = guild;
                long vOut = Convert.ToInt64(vIn);
                logs ServerCase = null;
                ServerCase = db.Logs.AsNoTracking().Where(p => p.ServerId == vOut).OrderBy(logs => logs.GlobalCase).LastOrDefaultAsync().Result;
                return ServerCase;
            }
        }

        private logs GetglobalCase()
        {
            using (var db = new CsharpiEntities())
            {
                logs Globalcase = null;
                Globalcase = db.Logs.AsNoTracking().OrderBy(logs => logs.GlobalCase).LastOrDefaultAsync().Result;
                return Globalcase;
            }

        }

        private Settings GetEnabled(long id)
        {
            using (var db = new CsharpiEntities())
            {
                Settings Enabled = null;
                Enabled = db.Settings.AsNoTracking().Where(p => p.ServerId == id).FirstOrDefaultAsync().Result;
                return Enabled;
            }
        }
        private async void DiscordLogs(SocketGuildUser author, ulong authorid, DateTimeOffset time, ISocketMessageChannel sendin, ulong guild, string ip, string punishment, double servercaseNummer)
        {

            var Logchannel = GetLogChannel((ulong)guild);
            if(Logchannel != null && Logchannel.Enabled == true)
            {

                ISocketMessageChannel messageChannel = null;
                messageChannel = author.Guild.GetChannel((ulong)Logchannel.LogsChannelId) as ISocketMessageChannel;
                var User = new EmbedFieldBuilder()
                    .WithName("User")
                    .WithValue(author)
                    .WithIsInline(true);
                var ID = new EmbedFieldBuilder()
                    .WithName("ID")
                    .WithValue(authorid)
                    .WithIsInline(true);
                var Time = new EmbedFieldBuilder()
                    .WithName("Time")
                    .WithValue(time)
                    .WithIsInline(true);
                var Punishment = new EmbedFieldBuilder()
                    .WithName("Punishment")
                    .WithValue(punishment)
                    .WithIsInline(false);
                var Message = new EmbedFieldBuilder()
                    .WithName("Message")
                    .WithValue(ip)
                    .WithIsInline(true);
                var embed = new EmbedBuilder()
                    .AddField(User)
                    .AddField(ID)
                    .AddField(Time)
                    .AddField(Punishment)
                    .AddField(Message)
                    .WithColor(Color.Red)
                    .WithTitle($"Case #{servercaseNummer}");
                await messageChannel.TriggerTypingAsync();
                await messageChannel.SendMessageAsync(embed: embed.Build());


            }
        }
        private Settings GetLogChannel(ulong guild)
        {
            using (var db = new CsharpiEntities())
            {
                ulong vIn = guild;
                long vOut = Convert.ToInt64(vIn);
                Settings LogChannelId = null;
                LogChannelId = db.Settings.AsNoTracking().Where(p => p.ServerId == vOut).FirstOrDefaultAsync().Result;
                return LogChannelId;
            }
        }
        private Settings Getpunishment(long id)
        {
            Settings punishment = null;
            using (var db = new CsharpiEntities())
            {
                punishment = db.Settings.AsNoTracking().Where(p => p.ServerId == id).FirstOrDefaultAsync().Result;

            }
            return punishment;
        }
        
    }
}
