using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Discord_bot_pi.Database
{
    public class logs
    {
        [Key]
        public double GlobalCase { get; set; }
        public double ServerCase { get; set;}
        public long ServerId { get; set; }
        public string IpposterId { get; set; }
        public string Username { get; set; }
        public string Time { get; set; }
        public string MesasageContainingIp { get; set; }
        public string Punishment { get; set; }

    }
}