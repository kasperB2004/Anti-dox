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
using Anti_Dox.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Reflection;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Anti_Dox.Database;
using System.Net.Http.Headers;
using Discord.Net;
using Anti_Dox.Services;
using System.Timers;
using System.Diagnostics;

namespace Anti_Dox.Services
{
    public class Info
    {
        static public string Ram { get; set; }
        static public string Cpu { get; set; }
        static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        private Timer aTimer;
        // declare the fields used later in this class
        private readonly ILogger _logger;
        private DiscordSocketClient _client;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;


        public Info(IServiceProvider services)
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
        }
        public Task OnReadyAsync()
        {
            //set timer
            SetTimer();
            //say its ready
            _logger.LogInformation($"Info Module loaded");
            //task completed
            return Task.CompletedTask;


        }
        //set timer
        private void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(10000);
            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += OnTimedEventAsync;
            //auto reset
            aTimer.AutoReset = true;
            //enable it
            aTimer.Enabled = true;
        }
        public async void OnTimedEventAsync(Object source, ElapsedEventArgs e)
        {
            //get cpu amount
            var cpu = await GetCpuUsageForProcess();
            //ram as proc
            Process proc = Process.GetCurrentProcess();
            //ram as bit's
            Int64 UsingRam = proc.PrivateMemorySize64;
            //ram with correct suffix
            var ram = SizeSuffix(UsingRam);
            //convert cpu to string

            Cpu = Convert.ToString(cpu);
            Ram = ram;
            //get installed cpu amount
            var gcMemoryInfo = GC.GetGCMemoryInfo();
            var installedMemory = gcMemoryInfo.TotalAvailableMemoryBytes;
            var physicalMemory = (double)installedMemory * 0.50 ;
            //check if the cpu or ram is above 50% before sending a email.
            if(cpu >= 50 || UsingRam >=physicalMemory )
            {
                //from adress which the email is sent from
                var fromAddress = new MailAddress(_config["FromEmail"], "Bot");
                //to what adress should i send the email
                var toAddress = new MailAddress(_config["ToEmail"], "Kasper");
                //password for email
                string fromPassword = _config["EmailPassword"];
                //the email subject
                const string subject = "ERROR high ram or cpu usage";
                //message
                string body = $"The ram usage is {ram} and the cpu usage is {cpu}%";
                //set up the email client
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 20000
                };
                //makin the message
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                //sending email
                {
                    smtp.Send(message);
                }
            }


        }
        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag)
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
        private async Task<double> GetCpuUsageForProcess()
        {
            //cuurret time
            var startTime = DateTime.UtcNow;
            //get startcpu usage
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            //waitttttttttttttt
            await Task.Delay(500);
            //get end time
            var endTime = DateTime.UtcNow;
            //get end cpu usage
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            //get cpuused in ms
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            //total time passed
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            //get total cpu usage
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return Math.Round((cpuUsageTotal * 100), 2);
        }
    }
}
