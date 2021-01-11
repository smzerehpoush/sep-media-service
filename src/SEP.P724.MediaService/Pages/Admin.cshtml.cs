using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SEP.P724.MediaService.Pages
{
    public class AdminModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public AdminModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}