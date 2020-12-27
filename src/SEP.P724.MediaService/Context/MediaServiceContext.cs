using Microsoft.EntityFrameworkCore;
using SEP.P724.MediaService.Model;

namespace SEP.P724.MediaService
{
    public class MediaServiceContext : DbContext
    {
        protected MediaServiceContext()
        {
        }

        public MediaServiceContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<MediaModel> Medias { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaModel>().HasKey(m => m.Id);
        }
    }
}