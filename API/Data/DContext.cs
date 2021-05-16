using System.Diagnostics.CodeAnalysis;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DContext : DbContext
    {

        public DContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserLike> UserLikes { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserLike>().HasKey(k => new { k.SourceUserId, k.LikedUserId });

            builder.Entity<UserLike>()
            .HasOne(u => u.SourceUser)
            .WithMany(u => u.LikedUsers)
            .HasForeignKey(u => u.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
            .HasOne(u => u.LikedUser)
            .WithMany(u => u.LikedByUsers)
            .HasForeignKey(u => u.LikedUserId)
            .OnDelete(DeleteBehavior.Cascade);
            ///////////////////////////////////////////////////////////

            builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(u => u.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(u => u.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        }
    }
}