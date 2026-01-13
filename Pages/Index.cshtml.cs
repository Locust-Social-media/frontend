using Locust.NewFolder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;

namespace Locust.Pages
{
    public class IndexModel : PageModel
    {
        public PostViewModel[] Posts { get; private set; }
        public void OnGet()
        {
            Posts = [new PostViewModel("test", "bleh", 1, 1), new PostViewModel("test2", "bleh2", 1, 1) ];
        }
    }
}
