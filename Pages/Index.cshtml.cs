using Azure;
using Locust.NewFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Locust.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public string Email { get; private set; } = "";
        public string RoleLabel { get; private set; } = "";

        public PostViewModel[] Posts { get; private set; } = [];

        public void OnGet()
        {
            Email = User.FindFirstValue(ClaimTypes.Email)
                ?? User.Identity?.Name
                ?? "onbekend";

            // Role uit AUTH (claims), niet uit DB
            RoleLabel = User.IsInRole("beheerder") ? "beheerder" : "gebruiker";

            Posts = [];
            //[
            //    new PostViewModel(
            //    "Test Title No 1",
            //    "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            //    1,
            //    1,
            //    0
            //),
            //new PostViewModel(
            //    "Test Title No 2",
            //    "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            //    999999999,
            //    999999999,
            //    1
            //),
            //new PostViewModel(
            //    "Test Title No 3",
            //    "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            //    500,
            //    30,
            //    2
            //)
            //];
        }
        
    }
}