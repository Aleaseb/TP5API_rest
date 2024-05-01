using Microsoft.EntityFrameworkCore;

namespace ChepoAPI
{
    public class PostgreDbContext : DbContext
    {
        public PostgreDbContext(DbContextOptions<PostgreDbContext> options)
        : base(options)
        {
        }

        public DbSet<UsersData> users { get; set; }
        public DbSet<UsersDataAuth> usersAuths { get; set; }
        public DbSet<SuccessData> success { get; set; }
        public DbSet<RankData> rank { get; set; }
        public DbSet<PlayerStatsData> player_stats { get; set; }
        public DbSet<ServerData> servers { get; set; }
        public DbSet<PlayerStateData> player_state { get; set; }
    }
}
