using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Discord_bot_pi.Database
{
    public class Settings
    {
        [Key]
        public long ServerId { get; set; }
        public string ServerName { get; set; }
        public ulong LogsChannelId { get; set; }
        public long SetById { get; set; }
        public bool Enabled { get; set; }
        public int punishment { get; set; }
        public bool Discordlogs { get; set; }

    }
}