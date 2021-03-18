using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Authzilla
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)=> options.UseSqlite(_settings.Database.ConnectionString);
    }

    //public class SQLiteDbContext: DbContext
    //{
    //    public SQLiteDbContext(DbContextOptions<DbContext> options) : base(options) { }
    //}
    //public class PostgreSQLDbContext : DbContext
    //{
    //    public PostgreSQLDbContext(DbContextOptions<DbContext> options) : base(options) { }
    //}
    //public class MSSQLDbContext : DbContext
    //{
    //    public MSSQLDbContext(DbContextOptions<DbContext> options) : base(options) { }
    //}

}
