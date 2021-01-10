using System.Collections.Generic;

namespace SEP.P724.MediaService.Contract.DTO
{
    public class PagedResponse<T> where T : class
    {
        public long TotalCount { get; set; }
        public IList<T>? Results { get; set; }
    }
}