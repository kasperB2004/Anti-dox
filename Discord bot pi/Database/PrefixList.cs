using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace Discord_bot_pi.Database
{
    public class PrefixList
    {
        [Key]
        public long ServerId { get; set; }
        public string ServerName { get; set; }
        public char Prefix { get; set; }
        public long SetById { get; set; }

    }
}
