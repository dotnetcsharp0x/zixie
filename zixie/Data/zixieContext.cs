#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using zixie.Models;

namespace zixie.Data
{
    public class zixieContext : DbContext
    {
        public zixieContext (DbContextOptions<zixieContext> options)
            : base(options)
        {
        }

        public DbSet<zixie.Models.User> User { get; set; }
        public DbSet<zixie.Models.Shares> Shares { get; set; }
        public DbSet<zixie.Models.Watchlist> Watchlists { get; set; }
        public DbSet<zixie.Models.Prices> Prices { get; set; }
    }
}
