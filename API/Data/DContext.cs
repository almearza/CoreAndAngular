using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DContext : DbContext
    {

        public DContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
    }
}