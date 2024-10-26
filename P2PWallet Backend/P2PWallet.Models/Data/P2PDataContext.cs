global using P2PWallet.Models;
global using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using P2PWallet.Models.Entities;
//using P2PWallet.Models.Data;

namespace P2PWallet.API.Data
{
    public class P2PDataContext : DbContext
    {
        public P2PDataContext(DbContextOptions<P2PDataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<UserAccount> UserAccounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Deposit> Deposits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<User>().HasKey(x => x.Id);

            //relationships
            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.User).WithMany(x => x.UserAccount).HasForeignKey(z => z.UserId);
            });
        }
    }
}
