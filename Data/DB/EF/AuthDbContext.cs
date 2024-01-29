using _10_Authen_TrinhCV.Data.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace _10_Authen_TrinhCV.Data.DB.EF
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(GetConnection());
        }

        public string GetConnection()
        {

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("AuthDb");
            return connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "jJ7mqTi7XAF9ptMb7uYtkPUyCEE+6h4b+pykbiHPj8A=",
                    Email = "admin@identtiy.gmail.com",
                    Fullname = "Trinh Cao"
                }
            );
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Code = "Admin",
                    Description = "Administrative user"
                },
                new Role
                {
                    Id = 2,
                    Code = "Normal",
                    Description = "Normal user"
                }
            );
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole()
                {
                    Id = 1,
                    RoleId = 1,
                    UserId = 1
                });
        }
    }

}
