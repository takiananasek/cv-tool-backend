using CVTool.Data;
using Microsoft.EntityFrameworkCore;

namespace CVTool.IntegrationTests
{
    /// THIS IS STILL TO DO
    public class DemoContextSqlite : DataContext
    {
        public DemoContextSqlite()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(TestWebApplicationFactory<Program>.ConnectionString);
        }

        public DemoContextSqlite(DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}
