using ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Data
{
    public class LogDataContext : DbContext
    {
        public DbSet<LogDataModel> Logs { get; set; }

        public LogDataContext(DbContextOptions<LogDataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
