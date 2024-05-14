using CVTool.Data;
using Microsoft.EntityFrameworkCore;

namespace CVTool.Tests.Integration
{
    public class DataContextSqlite : DataContext
    {
        public DataContextSqlite()
        {
        }

        public DataContextSqlite(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(TestWebApplicationFactory<Program>.ConnectionString);
        }

    }
}
