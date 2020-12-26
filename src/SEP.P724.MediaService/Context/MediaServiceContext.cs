using Microsoft.EntityFrameworkCore;

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
    }
}