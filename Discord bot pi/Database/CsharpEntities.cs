using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Discord_bot_pi.Database
{
    public partial class CsharpiEntities : DbContext
    {
       
        public virtual DbSet<PrefixList> PrefixList { get; set; }
        public virtual DbSet<logs> Logs { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "csharpi.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            optionsBuilder.UseSqlite(connection);
        }
    }
}