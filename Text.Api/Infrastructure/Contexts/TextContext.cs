using Microsoft.EntityFrameworkCore;
using Text.Api.Infrastructure.Models;

namespace Text.Api.Infrastructure.Contexts
{
    public sealed class TextContext : DbContext
    {

        public DbSet<TextFile> TextFiles { get; set; }

        public TextContext()
        {
                
        }

        public TextContext(DbContextOptions<TextContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
