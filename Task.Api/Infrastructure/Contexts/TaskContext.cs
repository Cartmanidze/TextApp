using Microsoft.EntityFrameworkCore;
using Task.Api.Infrastructure.Models;

namespace Task.Api.Infrastructure.Contexts
{
    public sealed class TaskContext : DbContext
    {

        public DbSet<TaskFile> TextFiles { get; set; }

        public TaskContext()
        {
                
        }

        public TaskContext(DbContextOptions<TaskContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
