using System.Collections.Generic;

namespace SEP.P724.MediaService.Model
{
    public class PagedResult<T> where T : class
    {
        public long TotalSize { get; set; }
        public IList<T>? Results { get; set; }
    }
}