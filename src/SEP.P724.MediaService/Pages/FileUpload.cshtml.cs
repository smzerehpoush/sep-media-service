using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SEP.P724.MediaService.Pages
{
    public class FileUploadModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public FileUploadModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}