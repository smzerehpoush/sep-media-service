using Microsoft.EntityFrameworkCore;
using SEP.P724.MediaService.Model;

namespace SEP.P724.MediaService.Context
{
    public class MediaContext : DbContext
    {
        protected MediaContext()
        {
        }

        public MediaContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<MediaModel> Medias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaModel>().HasKey(m => m.Id);
        }
    }
}